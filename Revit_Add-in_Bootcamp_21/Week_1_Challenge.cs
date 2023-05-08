#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace Revit_Add_in_Bootcamp_21
{
    [Transaction(TransactionMode.Manual)]
    public class Week_1_Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here
                        
            //Variable Declarations
            //   Numbers:
            //      int = whole numbers, byte = integers between 0 and 255, float = decimal numbers rounded to 7 digits
            //      double = decimal numbers accurate to about 15-16 digits USE THIS BY DEFAULT IF UNSURE
            //      decimal = 28-29 digit precision but smaller range than float or double       
            //   Others:
            //      char = "character" used to store single Unicode characters such as 'A', '%', '@'
            //      bool = "boolean" can only hold two values: true and false
            
            int numFloors = 250;
            int currentElev = 0; 
            int floorHeight = 15;

            
            // get titleblock
            FilteredElementCollector tbCollector = new FilteredElementCollector(doc);
            tbCollector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            ElementId tblockId = tbCollector.FirstElementId();
            
            // get view family types
            FilteredElementCollector vftCollector = new FilteredElementCollector(doc);
            vftCollector.OfClass(typeof(ViewFamilyType));

            ViewFamilyType fpVFT = null;
            ViewFamilyType cpVFT = null;

            foreach(ViewFamilyType curVFT in vftCollector)
            {
                if(curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    fpVFT = curVFT;
                }
                else if(curVFT.ViewFamily == ViewFamily.CeilingPlan)
                {
                    cpVFT = curVFT;
                }
            }

            Transaction t = new Transaction(doc);
            t.Start("FizzBuzzing");

            // create counters so that you can present user with data on created elements
            int fizzbuzzCounter = 0;
            int fizzCounter = 0;
            int buzzCounter = 0;

            //loop
            for (int i = 1; i <= numFloors; i++)
            {
                // 3. create level
                Level newLevel = Level.Create(doc, currentElev);
                newLevel.Name = "LEVEL " + i.ToString();
                currentElev += floorHeight;

                // 4. check for FIZZBUZZ
                if (i % 3 == 0 && i % 5 == 0)
                {
                    // FIZZBUZZ = create a sheet
                    ViewSheet newSheet = ViewSheet.Create(doc, tblockId);
                    newSheet.SheetNumber = "RAB-" + i.ToString();
                    newSheet.Name = "FIZZBUZZ-" + i.ToString();

                    // BONUS 
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, fpVFT.Id, newLevel.Id);
                    newFloorPlan.Name = "FIZZBUZZ-" + i.ToString();

                    XYZ insPoint = new XYZ(1.5, 1, 0);
                    Viewport newVP = Viewport.Create(doc, newSheet.Id, newFloorPlan.Id, insPoint);

                    fizzbuzzCounter++;

                }
                else if (i % 3 == 0)
                {
                    // FIZZ = create a floor plan
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, fpVFT.Id, newLevel.Id);
                    newFloorPlan.Name = "FIZZ-" + i.ToString();
                    fizzCounter++;
                }
                else if (i % 5 == 0)
                {
                    // BUZZ = create a ceiling plan
                    ViewPlan newCeilingPlan = ViewPlan.Create(doc, cpVFT.Id, newLevel.Id);
                    newCeilingPlan.Name = "BUZZ-" + i.ToString();
                    buzzCounter++;
                }
            }


            t.Commit();
            t.Dispose();

            // alert user to counts
            TaskDialog.Show("Complete", $"Created {numFloors} levels. Created {fizzbuzzCounter} FIZZBUZZ sheets. Created {fizzCounter} FIZZ floor plans. Created {buzzCounter}"
                + " BUZZ ceiling plans"); ;

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
