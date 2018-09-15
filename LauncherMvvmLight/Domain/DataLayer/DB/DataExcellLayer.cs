using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;


namespace LauncherMvvmLight.Domain.DataLayer.DB
{
    public class DataExcellLayer
    {


        public DataTable dt = new DataTable();
        DataTable dtCsv = new DataTable();
        public IEnumerable<DataRow> DevicesIE ;
        public List<DataRow> DevicesList;
        public List<String[]> DevicesCsvList;

        public DataExcellLayer()
        {
            //loadData();
        }

        public DataTable GetDeviceDataTable()
        {
            return dtCsv;
        }

        public bool LoadTableData(string sheetName, string path)
        {
            //return (ReadExcelFile(sheetName, path));
            return (ReadCsvFile(sheetName, path));
        }

      


        //public List<DataRow> ReadCsvFile(string sheetName, string path)
        public bool ReadCsvFile(string sheetName, string path)
        {
            // open the file "data.csv" which is a CSV file with headers
            using (CsvReader csv = new CsvReader(
                   new StreamReader("excDB.csv"), true))
            {
                // missing fields will not throw an exception,
                // but will instead be treated as if there was a null value
                //csv.DefaultParseErrorAction = ParseErrorAction.RaiseEvent;
                //csv.ParseError += new ParseErrorEventHandler(csv_ParseError);

                // to replace by "" instead, then use the following action:
                csv.MissingFieldAction = MissingFieldAction.ReplaceByEmpty;
                //csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;

                int fieldCount = csv.FieldCount;

                string[] headers = csv.GetFieldHeaders();

                for (int j = 0; j < fieldCount; j++)
                {
                    dtCsv.Columns.Add(headers[j]); //add headers  
                }


                //DevicesCsvList = csv.ToList();
                while (csv.ReadNextRecord())
                {
                    //for (int i = 0; i < fieldCount; i++)
                    //    Debug.WriteLine("Send to debug output." + string.Format("{0} = {1};",
                    //                  headers[i], csv[i]));

                    DataRow dr = dtCsv.NewRow();
                    for (int k = 0; k < fieldCount; k++)
                    {
                        dr[k] = csv[k].ToString();
                    }
                    dtCsv.Rows.Add(dr); //add other rows
                }

            }

            return true;

        }

      



    }
}
