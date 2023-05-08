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

#endregion

namespace Revit_Add_in_Bootcamp_21
{
    [Transaction(TransactionMode.Manual)]
    public class Week_2_Notes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // 1. pick elements and filter them into list -
            // THIS IS REQUIRED TO ENABLE SELECTION OF ELEMENTS IN CURRENT STATE OF REVIT
            UIDocument uidoc = uiapp.ActiveUIDocument;
            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select Elements");

            TaskDialog.Show ("Test", "I selected " + pickList.Count.ToString() + " elements");

            // 2. FILTER selected ELEMENTS for OUR SELECTION - IE: curves
            List<CurveElement> AllCurves = new List<CurveElement>();
            foreach (Element elem in pickList)
            {
                if (elem is CurveElement)
                {
                    // cast generic element to a more specific element type for our list 
                    AllCurves.Add(elem as CurveElement);
                }
            }

            // 2b.  Filter selected elements for model curves
            List<CurveElement> modelCurves = new List<CurveElement>();
            foreach (Element elem in pickList)
            {
                if (elem is CurveElement)
                {
                    CurveElement curveElem = elem as CurveElement;
                    // Alternative - CurveElement curveElem = (CurveElement) elem;

                    if (curveElem.CurveElementType == CurveElementType.ModelCurve)
                    {
                        modelCurves.Add(curveElem);
                    }
                    
                }
            }

            // ITERATE through SELECTED Curve elements list
            // 3. curve data
            foreach (CurveElement currentCurve in modelCurves)
            {
                Curve curve = currentCurve.GeometryCurve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                GraphicsStyle curStyle = currentCurve.LineStyle as GraphicsStyle;

                Debug.Print(curStyle.Name); 
                //this outputs a value into the output window of the visual studio debug window
            }

            // 5. create transaction with "using statement"
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create Revit Elements");
                // 4. create wall methods 
                Level newLevel = Level.Create(doc, 20);
                Curve curCurve1 = modelCurves[0].GeometryCurve;
               
                // method 1 (doc, curve, element id: level id, structural boolean)
                Wall.Create(doc, curCurve1, newLevel.Id, false);

                // method 2
                // per Revit API need: (doc, curve, elementid : walltypeid,
                // elementid: level id, height, offset, t/f flip boolean, structural boolean)
                // need list of wall types
                FilteredElementCollector wallTypes = new FilteredElementCollector(doc);
                wallTypes.OfClass(typeof(WallType));
                               
                Curve curCurve2 = modelCurves[1].GeometryCurve;
                WallType myWallType = GetWallTypeByName(doc, "Exterior - Brick on CMU");
                Wall.Create(doc, curCurve2, wallTypes.FirstElementId(), newLevel.Id, 20, 0, false, false);


                // 15. switch statements
                int numberValue = 5;
                string numAsString = "";

                switch (numberValue)
                {
                    case 1:
                        numAsString = "One";
                        break;
                    case 2:
                        numAsString = "Two";
                        break;
                    case 3:
                        numAsString = "Three";
                        break;
                    case 4:
                        numAsString = "Four";
                        break;
                    case 5:
                        numAsString = "Five";
                        break;

                    default:
                        numAsString = "Zero";
                        break;
                }

                // 16. advanced switch statements
                Curve curve5 = modelCurves[1].GeometryCurve;
                GraphicsStyle curve5GS = modelCurves[1].LineStyle as GraphicsStyle;

                WallType WallType1 = GetWallTypeByName(doc, "Storefront");
                WallType wallType2 = GetWallTypeByName(doc, "Exterior - Brick on CMU");

                switch (curve5GS.Name)
                {
                    case "<Thin Lines>":
                        Wall.Create(doc, curve5, WallType1.Id, newLevel.Id, 20, 0, false, false);
                        break;
                    case "<Wide Lines>":
                        Wall.Create(doc, curve5, wallType2.Id, newLevel.Id, 20, 0, false, false);
                        break;

                    default:
                        Wall.Create(doc, curve5, newLevel.Id, false);
                        break;
                }

                // creating MEP elements!!!!

                // creating Pipe Method 1 - (doc, elementId systemTypeID,
                // ElementId pipeTypeId, ElementId levelId, XYZ startPoint, XYZ endPoint)

                // System Type:
                FilteredElementCollector systemCollector = new FilteredElementCollector(doc);
                systemCollector.OfClass(typeof(MEPSystemType));

                // get pipe system type
                MEPSystemType pipeSystemType = null;
                foreach (MEPSystemType curType in systemCollector)
                {
                    if (curType.Name == "Domestic Hot Water")
                    {
                        pipeSystemType = curType;
                        break;
                    }
                }

                // get pipe type
                FilteredElementCollector collector1 = new FilteredElementCollector(doc);
                collector1.OfClass(typeof(PipeType));

                // create pipe
                Curve curCurve3 = modelCurves[2].GeometryCurve;
                Pipe newPipe = Pipe.Create(doc, pipeSystemType.Id, collector1.FirstElementId(), 
                    newLevel.Id, curCurve3.GetEndPoint(0), curCurve3.GetEndPoint(1));

                // get duct system type
                MEPSystemType ductSystemType = null;
                foreach (MEPSystemType curType in systemCollector)
                {
                    if (curType.Name == "Supply Air")
                    {
                        ductSystemType = curType;
                        break;
                    }
                }

                // get duct type
                FilteredElementCollector collector2 = new FilteredElementCollector(doc);
                collector2.OfClass(typeof(DuctType));

                // create duct
                Curve curCurve4 = modelCurves[3].GeometryCurve;
                Duct newDuct = Duct.Create(doc, ductSystemType.Id, collector2.FirstElementId(), newLevel.Id, curCurve4.GetEndPoint(0), curCurve4.GetEndPoint(1));


                // 13. use our new methods
                string testString = MyFirstMethod();
                MySecondMethod();
                string testString2 = MyThirdMethod("Hello World!");

                t.Commit();
            }

                    

            return Result.Succeeded;
        }

        internal string MyFirstMethod()
        {
            return "This is my first method!";
        }

        internal void MySecondMethod()
        {
            Debug.Print("This is my second method!");
        }

        internal string MyThirdMethod(string input)
        {
            return "This is my third method: " + input;
        }

        internal WallType GetWallTypeByName(Document doc, string typeName)
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

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
