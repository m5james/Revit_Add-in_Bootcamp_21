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
    public class Week_2_CSV_Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // create a variable for CSV file - File Path
            // use a string for file path
            string levelPath = "C:\\Users\\MitchJames\\Documents\\Revit Bootcamp\\RAB_Bonus_Levels.csv";

            // create a list of string arrays for CSV data
            // designate ARRAY with [] seen below
            List<string[]> levelData = new List<string[]>();

            // read text files data
            string[] levelArray = System.IO.File.ReadAllLines(levelPath);

            // loop through file data and put into a list
            foreach (string levelString in levelArray)
            {
                //use single ' to specify looking for single character
                string[] rowArray = levelString.Split(',');
                //place each row array into the list
                levelData.Add(rowArray);
            }

            // remove header row from data
            // first row of excel, according to C# is row 0 even tho is numbered 1 in excel
            levelData.RemoveAt(0);

            // create a transaction
            Transaction t = new Transaction(doc);
            t.Start("Create Levels");

            // loop through level data
            int counter = 0;
            foreach (string[] currentLevelData in levelData)
            {
                // create height variables
                double heightFeet = 0;
                double heightMeters = 0;

                // get height and convert from string to double
                // Boolean Try parse lets us know IF the value can be converted from string to double
                // [1] = "Index 1" = 1st Column of Data, [2] = "Index 2" = 2nd Column, etc...
                // the OUT command pushing value into variables specified above
                bool convertFeet = double.TryParse(currentLevelData[1], out heightFeet);
                bool convertMeters = double.TryParse(currentLevelData[2], out heightMeters);
                // if working in metric, would need to convert here because Revit API is in imperial feet
                double heightMetersConvert = heightMeters * 3.28084;
                double heightMetersConvert2 = UnitUtils.ConvertToInternalUnits(heightMeters, UnitTypeId.Meters);
                // ConvertToInternalUnits will automatically convert meters to internal units for Revit = imp. feet

                // create level and rename
                Level currentLevel = Level.Create(doc, heightFeet);
                currentLevel.Name = currentLevelData[0];

                // increment the counter
                counter++;

            }

            t.Commit();
            t.Dispose();

            // tell user what happened
            TaskDialog.Show("Complete", "Created " + counter.ToString() + " levels.");

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
