using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;

namespace ReadDataFromCSVFile
{
    static class Program
    {
        static void Main()
        {
            string csv_file_path = @"C:\XXX\yyy.csv";
            DataTable csvData = GetDataTabletFromCSVFile(csv_file_path);
            Console.WriteLine("Rows count:" + csvData.Rows.Count);
            Console.ReadLine();
            InsertDataIntoSQLServerUsingSQLBulkCopy(csvData);
            Console.ReadLine();
        }
        private static DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return csvData;
        }

        private static void InsertDataIntoSQLServerUsingSQLBulkCopy(DataTable csvFileData)
        {
            using (SqlConnection dbConnection = new SqlConnection("Server=localhost;Database=Any_db;Trusted_Connection=True;"))
            {
                dbConnection.Open();
                using (var blukCopy = new SqlBulkCopy(dbConnection))
                {
                    blukCopy.DestinationTableName = "TableName";

                    foreach (var column in csvFileData.Columns)
                        blukCopy.ColumnMappings.Add(column.ToString(), column.ToString());

                    blukCopy.WriteToServer(csvFileData);
                }
            }
        }
    }
}