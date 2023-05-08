#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;

#endregion

namespace Revit_Add_in_Bootcamp_21
{
    [Transaction(TransactionMode.Manual)]
    public class Week_2_Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // MODULE 02 - Challenge - Can you find the hidden message?

            // Create an add-in that will:
            // Generate Revit elements from model lines based on the line style name.
            // Add-in must promt you to select elements. 
            // It should then filter the elements for model curves
            // then loop through them to create Revit elements based on line styles (Provided on website)

            // Generate List of All Line Types in Model & Separate into Categories
            // prompt selection
            UIDocument uidoc = uiapp.ActiveUIDocument;
            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select elements");


            // create list of modelCurves
            List<CurveElement> modelCurves = new List<CurveElement>();
            
            
            // loop through selection & filter for model curves.
            foreach (Element elem in pickList)
            {
                if (elem is CurveElement)
                {
                    CurveElement curveElem = elem as CurveElement;
                    modelCurves.Add(curveElem);                    
                }
            }
            
            TaskDialog.Show("Selection", $"Selected {pickList.Count} elements");
                    

            WallType wallType1 = GetWallTypeByName(doc, "Storefront");
            WallType wallType2 = GetWallTypeByName(doc, "Generic - 8\"");

            MEPSystemType ductSystemType = GetMEPSystemTypeByName(doc, "Supply Air");
            DuctType ductType = GetDuctTypeByName(doc, "Default");

            MEPSystemType pipeSystemType = GetMEPSystemTypeByName(doc, "Domestic Hot Water");
            PipeType pipeType = GetPipeTypeByName(doc, "Default");

            Parameter levelParam = doc.ActiveView.LookupParameter("Associated Level");
            Level currentLevel = GetLevelByName(doc, levelParam.AsString());

            List<ElementId> linesToHide = new List<ElementId>();

            // get level from active view
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Creating Elements by Linestyle");                               
                foreach (CurveElement currentCurve in modelCurves)
                {
                    Curve curve = currentCurve.GeometryCurve;
                                    
                                    
                    //  try
                    //  {
                    //     XYZ startPoint = curve.GetEndPoint(0);
                    //     XYZ endPoint = curve.GetEndPoint(1);
                    //  }
                    //  catch (Exception ex)
                    //  {
                    //      TaskDialog.Show("Error", ex.);
                    //  }

                    GraphicsStyle curStyle = currentCurve.LineStyle as GraphicsStyle;
                    Debug.Print(curStyle.Name);

                    switch (curStyle.Name)
                    {
                        case "A-GLAZ":
                            Wall currentWall = Wall.Create(doc, curve, wallType1.Id, currentLevel.Id, 20, 0, false, false);
                            break;
                        case "A-WALL":
                            Wall currentWall2 = Wall.Create(doc, curve, wallType2.Id, currentLevel.Id, 20, 0, false, false);
                            break;
                        case "M-DUCT":
                            Duct currentDuct = Duct.Create(doc, ductSystemType.Id, ductType.Id, currentLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1));
                            break;
                        case "P-PIPE":
                            Pipe currentPipe = Pipe.Create(doc, pipeSystemType.Id, pipeType.Id, currentLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1));
                            break;

                        default:
                            linesToHide.Add(currentCurve.Id);
                            break;
                    }
                }
                doc.ActiveView.HideElements(linesToHide);
                t.Commit();
            }
            return Result.Succeeded;
        }

        private WallType GetWallTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(WallType));

            foreach (WallType curType in collector)
            {
                if (curType.Name == typeName)
                {
                    return curType;
                }            
            }
            return null;
            // EXAMPLE: 
            // WallType WallType1 = GetWallTypeByName(doc, "Storefront");
            // WallType wallType2 = GetWallTypeByName(doc, "Exterior - Brick on CMU");
        }

        private Level GetLevelByName(Document doc, string levelName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Level));

            foreach (Level curLevel in collector)
            {
                if (curLevel.Name == levelName)
                {
                    return curLevel;
                }
            }
            return null;
        }
              
        private MEPSystemType GetMEPSystemTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(MEPSystemType));

            foreach (MEPSystemType curType in collector)
            {
                if (curType.Name == typeName)
                {
                    return curType;
                }                
            }
            return null;
        }
        
        private DuctType GetDuctTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(DuctType));

            foreach (DuctType curType in collector)
            {
                if (curType.Name == typeName)
                {
                    return curType;
                }                
            }
            return null;
        }
        
        private PipeType GetPipeTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));

            foreach (PipeType curType in collector)
            {
                if (curType.Name == typeName)
                {
                    return curType;
                }                
            }
            return null;
        }        
        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
