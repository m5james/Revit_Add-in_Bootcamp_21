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
    public class Week_1_Notes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // WEEK 1 BOOTCAMP SKILLS
            // create a comment using a double forward slash
            // comments do not compile
            // so you can leave yourself notes in your code

            // Creating Variables
            // DataType VariableName = Value; <- always end the line with a semicolon!

            //  Variable Declarations
            //  Text: string
            string text1 = "this is my text";
            string text2 = "this is my next text";
            // able to combine strings
            string text3 = text1 + text2;
            string text4 = text3 + " " + text2 + "abcd";

            //  Creating Number Variables
            //  Numbers:
            //      int = whole numbers, byte = integers between 0 and 255, float = decimal numbers rounded to 7 digits
            //      double = decimal numbers accurate to about 15-16 digits USE THIS BY DEFAULT IF UNSURE
            //      decimal = 28-29 digit precision but smaller range than float or double
            int number1 = 10;
            double number2 = 20.5;

            //  Other Variables:
            //      char = "character" used to store single Unicode characters such as 'A', '%', '@'
            //      bool = "boolean" can only hold two values: true and false

            // can do some math
            double number3 = number1 + number2;
            double number4 = number3 - number2;
            double number5 = number4 / 100;
            double number6 = number5 * number4;
            double number7 = (number6 + number5) / number4;

            // convert meters to feet
            double meters = 4;
            double metersToFeet = meters * 3.28084;
            // convert mm to feet
            double mm = 3500;
            double mmToFeet = mm / 304.8;
            double mmToFeet2 = (mm / 1000) * 3.28084;

            // find the remainder when dividing (ie. modulo or mod)
            double remainder1 = 100 % 10; // equals 0 (100 divided by 10 = 10)
            double remainder2 = 100 % 9; // equals 1 (100 divided by 9 = 11 with a remainder of 1)

            // increment a number
            number6++; //increments +1
            number6--; //increments -1
            number6 += 10; //increments +10

            // boolean operators - conditional logic - used to compare things
            // == equals
            // != not equal
            // > greater than
            // < less than
            // <= or >= less than or equal too/ greater than or equal to

            // check a value and perform a single action if true
            if (number6 > 10)
            {
                // do something if true
            }

            // IF ELSE statement
            if (number5 == 100)
            {
                // do something if true
            }
            else
            {
                // do something else if true
            }

            // IF ELSE IF ELSE
            if (number5 == 100)
            {
                // do something if true
            }
            else if (number6 == 8)
            {
                // do something else if true
            }
            else
            {
                // do a third thing if false
            }

            //  compound conditional statements
            //  look for two things (or more) using &&
            if (number6 > 10 && number5 == 100)
            {
                //do something if both are true
            }

            // look for either thing using ||
            if (number6 > 10 || number5 == 100)
            {
                // do something if either is true
            }

            // LISTS
            // creating a list  - use <"x"> to specify what you are making a list of
            List<string> list1 = new List<string>();

            // add items to list
            list1.Add(text1);
            list1.Add(text2);
            list1.Add("this is some text");

            // create a list and add items to it - method 2
            List<int> List2 = new List<int> { 1, 2, 3, 4, 5 };

            // LOOPS

            // FOR EACH LOOP:
            int letterCounter = 0;
            foreach (string currentString in list1)
            {
                // do something with currentString
                // IE: letterCounter = letterCounter + currentString.Length;
                letterCounter += currentString.Length;
            }

            // FOR LOOP
            int numberCount = 0;
            int counter = 100;
            for (int i = 0; i <= counter; i++)
            {
                // do something 
                numberCount += i;
            }

            // Output a dialog box ("First Quote is Title", after comma is content of box)
            TaskDialog.Show("Number Counter", "The number count is " + numberCount.ToString());

            //  REVIT API SPECIFIC SECTION
            //  see top of code - Data Type & Doc set to specify Revit & Current Working Document (Current model being worked on)

            // create a Transaction to lock the model
            Transaction t = new Transaction(doc);
            t.Start("Doing something in Revit");

            // make a change in the Revit model

            // create a floor level - show in revit API (www.revitapidocs.com)
            // elevation value is in decimal feet regardless of model's units
            double elevation = 100;
            Level newLevel = Level.Create(doc, elevation);
            newLevel.Name = "My New Level";

            // create a floor plan view 
            // but first we need to get a floor plan View Family Type
            // by creating a FILTERED ELEMENT COLLECTOR
            FilteredElementCollector collector1 = new FilteredElementCollector(doc);
            collector1.OfClass(typeof(ViewFamilyType));

            // first we create an empty variable
            ViewFamilyType floorPlanVFT = null; 
            // now we loop through the collector
            foreach (ViewFamilyType curVFT in collector1)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    floorPlanVFT = curVFT;
                    break;
                }
            }

            // create a view by specifying the document, view family type, and level
            ViewPlan newPlan = ViewPlan.Create(doc, floorPlanVFT.Id, newLevel.Id);
            newPlan.Name = "My New Floor Plan";

            // create ceiling plan view family type
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
            newCeilingPlan.Name = "My New Ceiling Plan";


            // CREATING SHEETS
            // get titleblock first
            FilteredElementCollector collector2 = new FilteredElementCollector(doc);
            collector2.OfCategory(BuiltInCategory.OST_TitleBlocks);
            // all built in categories begin with OST 
            // creating sheet
            ViewSheet newSheet = ViewSheet.Create(doc, collector2.FirstElementId());
            newSheet.Name = "My New Sheet";
            newSheet.SheetNumber = "A101";

            // ADDING VIEW TO SHEET AKA ADDING VIEWPORT
            // first create a point
            XYZ insPoint = new XYZ(1, 0.5, 0);

            Viewport newViewport = Viewport.Create(doc, newSheet.Id, newPlan.Id, insPoint);

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
