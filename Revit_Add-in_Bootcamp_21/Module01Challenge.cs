#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
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
    public class Module01Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here

            //Declarations
            int number = 250;
            int startingElevation = 0; 
            int floorHeight = 15;

            Transaction t = new Transaction(doc);
            t.Start("FizzBuzzing");

            //loop
            for (int i = 1; i <= number; i++)
            {
                //create level for each
                Level newLevel = Level.Create(doc, startingElevation);
                newLevel.Name = "Level_" + i;
                                
                //increment current elevation by floor height
                startingElevation += floorHeight;

                // if number divisible by 3, create floor plan and name it "FIZZ_#"
                if (i % 3 == 0)
                {
                    //create a floor plan
                    FilteredElementCollector collector1 = new FilteredElementCollector(doc);
                    collector1.OfClass(typeof(ViewFamilyType));

                    ViewFamilyType floorPlanVFT = null;
                    foreach (ViewFamilyType curVFT in collector1)
                    {
                        if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                        {
                            floorPlanVFT = curVFT;
                            break;
                        }
                    }

                    // create view by specifying document, view family type, and level
                    ViewPlan newPlan = ViewPlan.Create(doc, floorPlanVFT.Id, newLevel.Id);
                    newPlan.Name = "Fizz_" + i;

                }


                // if the number is divisible by 5, create a ceiling plan and name it "BUZZ_#"
                if(i % 5 == 0)
                {
                    //get a ceiling view family type
                    FilteredElementCollector collector1 = new FilteredElementCollector(doc);
                    collector1.OfClass(typeof(ViewFamilyType));

                    ViewFamilyType ceilingPlanVFT = null;
                    foreach (ViewFamilyType curVFT in collector1)
                    {
                        if (curVFT.ViewFamily == ViewFamily.CeilingPlan)
                        {
                            ceilingPlanVFT = curVFT;
                            break;
                        }
                    }

                    // create a ceiling plan using the ceiling plan view family type
                    ViewPlan newCeilingPlan = ViewPlan.Create(doc, ceilingPlanVFT.Id, newLevel.Id);
                    newCeilingPlan.Name = "BUZZ_" + i;

                }

                // if the number is divisible by both 3 and 5, create a sheet and name it "FIZZBUZZ_#"
                if (i % 3 == 0 && i % 5 == 0)
                {
                    //Create a sheet - requires title block
                    // Get title block by creating Filtered Element Collector
                    FilteredElementCollector collector2 = new FilteredElementCollector(doc);
                    collector2.OfCategory(BuiltInCategory.OST_TitleBlocks);
                    // create a sheet
                    ViewSheet newSheet = ViewSheet.Create(doc, collector2.FirstElementId());
                    newSheet.Name = "FIZZBUZZ_" + i;
                    newSheet.SheetNumber = "FB_" + i;
                    // create floor plan for each FIZZBUZZ
                    FilteredElementCollector collector1 = new FilteredElementCollector(doc);
                    collector1.OfClass(typeof(ViewFamilyType));

                    ViewFamilyType floorPlanVFT = null;
                    foreach (ViewFamilyType curVFT in collector1)
                    {
                        if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                        {
                            floorPlanVFT = curVFT;
                            break;
                        }
                    }

                    // create view by specifying document, view family type, and level
                    ViewPlan newPlan = ViewPlan.Create(doc, floorPlanVFT.Id, newLevel.Id);
                    newPlan.Name = "FIZZBUZZ_" + i;

                    // add a view to the sheet by creating viewport element
                    // first need to create a point
                    XYZ insPoint = new XYZ(1, 0.5, 0);

                    // Create Viewport
                    Viewport newViewport = Viewport.Create(doc, newSheet.Id, newPlan.Id, insPoint);

                }



            }                    
            
            t.Commit();
            t.Dispose();

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
