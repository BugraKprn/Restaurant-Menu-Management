using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OfficeOpenXml;
using System.Data;

namespace System
{
    public class CsvHelper
    {
        public static CsvTable GetCsvTable(string filename, CsvOptions options)
        {
            CsvTable result = new CsvTable();
            string[] lines = File.ReadAllLines(filename, Encoding.Default);

            return GetCsvTable(lines, options);
        }
            
        public static CsvTable GetCsvTable(string[] lines, CsvOptions options)
        {
            CsvTable result = new CsvTable();
            bool first = true;
            foreach (string line in lines)
            {
                string[] values = line.Split(options.Separator);

                if (first)
                {
                    first = false;
                    if (options.FirstLineIsTitle)
                    {
                        for (int i = 0; i < values.Length; i++)
                            result.Columns[values[i]] = i;
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < values.Length; i++)
                            result.Columns["Column" + i] = i;
                    }
                }

                result.Rows.Add(values);
            }

            return result;
        }

        public static CsvTable GetCsvTableFromExcel(string filename, CsvOptions options)
        {
            CsvTable result = new CsvTable();
            FileInfo fi = new FileInfo(filename);
            ExcelPackage package = new ExcelPackage(fi);
            ExcelWorksheet sheet = package.Workbook.Worksheets[options.ExcelSayfaIndex];
            if (sheet == null)
            {
                throw new Exception(filename + "\nDosyasında Aktarılabilecek Uygun Kayıt Yoktur.");
            }
            else
            {
                if (sheet.Dimension == null)
                {
                    throw new Exception(filename + "\nDosyasında " + options.ExcelSayfaIndex + ".Sayfada Aktarılabilecek Uygun Kayıt Yoktur.");
                }

                int startRowIndex = sheet.Dimension.Start.Row;
                int endRowIndex = sheet.Dimension.End.Row;
                var valueList = new List<string>();
                for (int rowIndex = startRowIndex; rowIndex <= endRowIndex; rowIndex++)
                {
                    valueList.Clear();

                    if (rowIndex == startRowIndex)
                    {
                        if (options.FirstLineIsTitle)
                        {
                            /*if (sheet.Cells[rowIndex, 1].Value != null)  ilk sütun boş olabilir o yüzden diğer sütunlara bakmak gerekir*/
                            {
                                for (int i = 1; i <= sheet.Dimension.End.Column; i++)
                                {
                                    if (sheet.Cells[rowIndex, i].Value == null) continue;

                                    string columnName = sheet.Cells[rowIndex, i].Value.ToString();
                                    if (result.Columns.ContainsKey(columnName))
                                        throw new Exception("opps! İçeri aldığınız dosyada  [" + columnName + "] isminde iki kolon var. Kolon isimleri tekil olmalıdır.");

                                    result.Columns.Add(columnName, i - 1);
                                }
                            }
                            continue;
                        }
                        else
                        {
                            for (int i = 1; i <= sheet.Dimension.End.Column; i++)
                            {
                                string columnName = "Column" + i.ToString();
                                if (result.Columns.ContainsKey(columnName))
                                    throw new Exception("opps! İçeri aldığınız dosyada  [" + columnName + "] isminde iki kolon var. Kolon isimleri tekil olmalıdır.");

                                result.Columns.Add(columnName, i - 1);
                            }
                        }
                    }

                    for (int i = 1; i <= sheet.Dimension.End.Column; i++)
                    {
                        object obj = sheet.Cells[rowIndex, i].Value;
                        valueList.Add(obj == null ? "" : obj.ToString());
                    }

                    result.Rows.Add(valueList.ToArray());
                }
            }

            return result;
        }

        public static CsvTable GetCsvTable2(string filename, CsvOptions options)
        {
            CsvTable result = new CsvTable();
            string[] lines = File.ReadAllLines(filename, Encoding.Default);

            return GetCsvTable2(lines, options);
        }

        public static CsvTable GetCsvTable2(string[] lines, CsvOptions options)
        {
            CsvTable result = new CsvTable();
            bool first = true;
            foreach (string line in lines)
            {
                string[] values = line.Split(options.Separator);

                if (first)
                {
                    first = false;
                    if (options.FirstLineIsTitle)
                    {
                        for (int i = 0; i < values.Length; i++)
                            result.Columns[values[i]] = i;
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < values.Length; i++)
                            result.Columns["Column" + i] = i;
                    }
                }

                result.Rows2.Add(values);
            }

            return result;
        }

        public static CsvTable GetCsvTableFromExcel2(string filename, CsvOptions options)
        {
            CsvTable result = new CsvTable();
            FileInfo fi = new FileInfo(filename);
            ExcelPackage package = new ExcelPackage(fi);
            ExcelWorksheet sheet = package.Workbook.Worksheets[options.ExcelSayfaIndex];
            if (sheet == null)
            {
                throw new Exception(filename + "\nDosyasında Aktarılabilecek Uygun Kayıt Yoktur.");
            }
            else
            {
                if (sheet.Dimension == null)
                {
                    throw new Exception(filename + "\nDosyasında " + options.ExcelSayfaIndex + ".Sayfada Aktarılabilecek Uygun Kayıt Yoktur.");
                }

                int startRowIndex = sheet.Dimension.Start.Row;
                int endRowIndex = sheet.Dimension.End.Row;
                var valueList = new List<object>();
                for (int rowIndex = startRowIndex; rowIndex <= endRowIndex; rowIndex++)
                {
                    valueList.Clear();

                    if (rowIndex == startRowIndex)
                    {
                        if (options.FirstLineIsTitle)
                        {
                            /*if (sheet.Cells[rowIndex, 1].Value != null)  ilk sütun boş olabilir o yüzden diğer sütunlara bakmak gerekir*/
                            {
                                for (int i = 1; i <= sheet.Dimension.End.Column; i++)
                                {
                                    if (sheet.Cells[rowIndex, i].Value == null) continue;

                                    string columnName = sheet.Cells[rowIndex, i].Value.ToString();
                                    if (result.Columns.ContainsKey(columnName))
                                        throw new Exception("opps! İçeri aldığınız dosyada  [" + columnName + "] isminde iki kolon var. Kolon isimleri tekil olmalıdır.");

                                    result.Columns.Add(columnName, i - 1);
                                }
                            }
                            continue;
                        }
                        else
                        {
                            for (int i = 1; i <= sheet.Dimension.End.Column; i++)
                            {
                                string columnName = "Column" + i.ToString();
                                if (result.Columns.ContainsKey(columnName))
                                    throw new Exception("opps! İçeri aldığınız dosyada  [" + columnName + "] isminde iki kolon var. Kolon isimleri tekil olmalıdır.");

                                result.Columns.Add(columnName, i - 1);
                            }
                        }
                    }

                    for (int i = 1; i <= sheet.Dimension.End.Column; i++)
                    {
                        valueList.Add(sheet.Cells[rowIndex, i].Value);
                    }

                    result.Rows2.Add(valueList.ToArray());
                }
            }

            return result;
        }
    }

    public class CsvTable
    {
        public string TableName { get; set; }
        public Dictionary<string, int> Columns = new Dictionary<string, int>();
        public List<string[]> Rows = new List<string[]>();
        public List<object[]> Rows2 = new List<object[]>();

        public DataTable ToTable() {
            DataTable dtTable = new DataTable(TableName);
            foreach (var item in Columns)
            {
                dtTable.Columns.Add(item.Key);
            }

            foreach (var row in Rows)
            {
                DataRow drIns=dtTable.NewRow();
                foreach (var item in Columns)
                {
                    drIns[item.Key ] = row[Columns[item.Key]];
                }
                dtTable.Rows.Add(drIns);
            }
            return dtTable;
        }
    }

    public class CsvOptions
    {
        public bool FirstLineIsTitle { get; set; }
        public char Separator { get; set; }
        public string DateFormat { get; set; }
        public char NumericPoint { get; set; }
        public int ExcelSayfaIndex { get; set; }
        public CsvOptions()
        {
            FirstLineIsTitle = true;
            Separator = '\t';
            DateFormat = "dd.MM.yyyy";
            NumericPoint = ',';
            ExcelSayfaIndex = 1;
        }
    }
}
