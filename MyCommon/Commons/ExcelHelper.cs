using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;

namespace MyProject.NOPI {
    /// <summary>
    /// Excel操作类
    /// </summary>
    public class ExcelHelper {
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="Model">集合</param>
        /// <param name="Lst">自定义表头</param>
        /// <param name="SheetName">Sheet名称 默认：sheet1</param>
        public static Stream ExportIEnumerableToExcel<T>(IEnumerable<T> Model, string SheetName = "Sheet1") {
            DataTable dt = new DataTable();
            foreach (System.Reflection.PropertyInfo p in typeof(T).GetProperties()) {
                dt.Columns.Add(p.Name);
            }

            DataRow dr = null;

            //表体
            foreach (var item in Model) {
                dr = dt.NewRow();
                dt.Rows.Add(dr);
                foreach (System.Reflection.PropertyInfo p in item.GetType().GetProperties()) {
                    object val = p.GetValue(item, null);
                    dr[p.Name] = val;
                }
            }

            return ExportDataTableToExcel(dt, SheetName);
        }

        /// <summary>    
        /// 由DataSet导出Excel   
        /// </summary>    
        /// <param name="SourceDs">要导出数据的DataTable</param>   
        /// <param name="SheetName">工作表名称</param>   
        /// <returns>Excel工作表</returns>    
        private static Stream ExportDataSetToExcel(DataSet SourceDs, string SheetName) {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            string[] SheetNames = SheetName.Split(',');
            for (int i = 0; i < SheetNames.Length; i++) {
                ISheet sheet = workbook.CreateSheet(SheetNames[i]);
                IRow headerRow = sheet.CreateRow(0);            // handling header.          
                foreach (DataColumn column in SourceDs.Tables[i].Columns)
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);// handling value.
                int rowIndex = 1;
                foreach (DataRow row in SourceDs.Tables[i].Rows) {
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in SourceDs.Tables[i].Columns) {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }
                    rowIndex++;
                }
            }
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            workbook = null;
            return ms;
        }


        /// <summary>   
        /// 由DataSet导出Excel  
        /// </summary>    
        /// <param name="SourceDs">要导出数据的DataTable</param>  
        /// <param name="FileName">指定Excel工作表名称</param>   
        /// <param name="SheetName">指定Excel工作簿名称</param>  
        /// <returns>Excel工作表</returns>    
        public static void ExportDataSetToExcel(DataSet SourceDs, string FileName, string SheetName) {
            MemoryStream ms = ExportDataSetToExcel(SourceDs, SheetName) as MemoryStream;
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;FileName=" + FileName);
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            HttpContext.Current.Response.End();
            ms.Close();
            ms = null;
        }


        /// <summary>    
        /// 由DataTable导出Excel  
        /// </summary>  
        /// <param name="DT">要导出数据的DataTable</param>  
        /// <param name="SheetName">指定Excel工作簿名称</param>  
        /// <returns>Excel工作表</returns>   
        public static Stream ExportDataTableToExcel(DataTable DT, string SheetName = "Sheet1") {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            ISheet sheet = workbook.CreateSheet(SheetName);
            IRow headerRow = sheet.CreateRow(0);

            // handling header.     
            foreach (DataColumn column in DT.Columns) {
                ICell cell = headerRow.CreateCell(column.Ordinal);
                cell.SetCellValue(column.ColumnName);

                ICellStyle css = workbook.CreateCellStyle();
                cell.CellStyle = css;
                css.BorderBottom = BorderStyle.THIN;
                css.BorderLeft = BorderStyle.THIN;
                css.BorderRight = BorderStyle.THIN;
                css.BorderTop = BorderStyle.THIN;


                IFont font = workbook.CreateFont();
                css.SetFont(font);
                font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            }
            // handling value.     
            int rowIndex = 1;
            foreach (DataRow row in DT.Rows) {
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in DT.Columns) {
                    ICell cell = dataRow.CreateCell(column.Ordinal);
                    cell.SetCellValue(row[column].ToString());

                    ICellStyle css = workbook.CreateCellStyle();
                    cell.CellStyle = css;
                    css.BorderBottom = BorderStyle.THIN;
                    css.BorderLeft = BorderStyle.THIN;
                    css.BorderRight = BorderStyle.THIN;
                    css.BorderTop = BorderStyle.THIN;
                }
                rowIndex++;
            }
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            sheet = null;
            headerRow = null;
            workbook = null;
            return ms;
        }


        /// <summary>  
        /// 由DataTable导出Excel  
        /// </summary>   
        /// <param name="DT">要导出数据的DataTable</param> 
        /// <param name="FileName">指定Excel工作表名称</param>   
        /// <param name="SheetName">指定Excel工作簿名称</param>  
        /// <returns>Excel工作表</returns>   
        public static void ExportDataTableToExcel(DataTable DT, string FileName, string SheetName = "Sheet1") {
            MemoryStream ms = ExportDataTableToExcel(DT, SheetName) as MemoryStream;
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;FileName=" + FileName);
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            HttpContext.Current.Response.End();
            ms.Close();
            ms = null;
        }


        /// <summary>    
        /// 由Excel导入DataTable    
        /// </summary>    
        /// <param name="ExcelFileStream">Excel文件流</param>    
        /// <param name="SheetName">Excel工作表名称</param>    
        /// <param name="HeaderRowIndex">Excel表头行索引</param>   
        /// <returns>DataTable</returns>  
        public static DataTable ImportDataTableFromExcel(Stream ExcelFileStream, string SheetName, int HeaderRowIndex) {
            HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            ISheet sheet = workbook.GetSheet(SheetName);
            DataTable table = new DataTable();
            IRow headerRow = sheet.GetRow(HeaderRowIndex);
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++) {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                    dataRow[j] = row.GetCell(j).ToString();
            }
            ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }


        /// <summary>    
        /// 由Excel导入DataTable   
        /// </summary>    
        /// <param name="ExcelFilePath">Excel文件路径，为物理路径。</param>  
        /// <param name="SheetName">Excel工作表名称</param>
        /// <param name="HeaderRowIndex">Excel表头行索引</param>  
        /// <returns>DataTable</returns>    
        public static DataTable ImportDataTableFromExcel(string ExcelFilePath, string SheetName, int HeaderRowIndex) {
            using (FileStream stream = System.IO.File.OpenRead(ExcelFilePath)) {
                return ImportDataTableFromExcel(stream, SheetName, HeaderRowIndex);
            }
        }


        /// <summary>    
        /// 由Excel导入DataTable   
        /// </summary>   
        /// <param name="ExcelFileStream">Excel文件流</param>    
        /// <param name="SheetIndex">Excel工作表索引</param>   
        /// <param name="HeaderRowIndex">Excel表头行索引</param>  
        /// <returns>DataTable</returns>    
        public static DataTable ImportDataTableFromExcel(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex) {
            HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            ISheet sheet = workbook.GetSheetAt(SheetIndex);
            DataTable table = new DataTable();
            IRow headerRow = sheet.GetRow(HeaderRowIndex);
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++) {
                if (headerRow.GetCell(i) == null || headerRow.GetCell(i).StringCellValue.Trim() == "") {
                    // 如果遇到第一个空列，则不再继续向后读取                
                    cellCount = i + 1;
                    break;
                }
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) {
                IRow row = sheet.GetRow(i);
                if (row == null || row.GetCell(0) == null || row.GetCell(0).ToString().Trim() == "") {
                    // 如果遇到第一个空行，则不再继续向后读取      
                    break;
                }
                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++) {
                    dataRow[j] = row.GetCell(j);
                }
                table.Rows.Add(dataRow);
            }
            ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }


        /// <summary>   
        /// 由Excel导入DataTable 
        /// </summary>   
        /// <param name="ExcelFilePath">Excel文件路径，为物理路径。</param>  
        /// <param name="SheetIndex">Excel工作表索引</param>  
        /// <param name="HeaderRowIndex">Excel表头行索引</param> 
        /// <returns>DataTable</returns>  
        public static DataTable ImportDataTableFromExcel(string ExcelFilePath, int SheetIndex, int HeaderRowIndex) {
            using (FileStream stream = System.IO.File.OpenRead(ExcelFilePath)) {
                return ImportDataTableFromExcel(stream, SheetIndex, HeaderRowIndex);
            }
        }


        /// <summary>    
        /// 由Excel导入DataSet，如果有多个工作表，则导入多个DataTable 
        /// </summary>   
        /// <param name="ExcelFileStream">Excel文件流</param>   
        /// <param name="HeaderRowIndex">Excel表头行索引</param> 
        /// <returns>DataSet</returns>  
        public static DataSet ImportDataSetFromExcel(Stream ExcelFileStream, int HeaderRowIndex) {
            DataSet ds = new DataSet();
            HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            for (int a = 0, b = workbook.NumberOfSheets; a < b; a++) {
                ISheet sheet = workbook.GetSheetAt(a);
                DataTable table = new DataTable();
                IRow headerRow = sheet.GetRow(HeaderRowIndex);
                int cellCount = headerRow.LastCellNum;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++) {
                    if (headerRow.GetCell(i) == null || headerRow.GetCell(i).StringCellValue.Trim() == "") {
                        // 如果遇到第一个空列，则不再继续向后读取                   
                        cellCount = i + 1;
                        break;
                    }
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) {
                    IRow row = sheet.GetRow(i);
                    if (row == null || row.GetCell(0) == null || row.GetCell(0).ToString().Trim() == "") {
                        // 如果遇到第一个空行，则不再继续向后读取      
                        break;
                    }
                    DataRow dataRow = table.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++) {
                        if (row.GetCell(j) != null) {
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                    }
                    table.Rows.Add(dataRow);
                }
                ds.Tables.Add(table);
            }
            ExcelFileStream.Close();
            workbook = null;
            return ds;
        }


        /// <summary>   
        /// 由Excel导入DataSet，如果有多个工作表，则导入多个DataTable   
        /// </summary>   
        /// <param name="ExcelFilePath">Excel文件路径，为物理路径。</param>  
        /// <param name="HeaderRowIndex">Excel表头行索引</param>   
        /// <returns>DataSet</returns>  
        public static DataSet ImportDataSetFromExcel(string ExcelFilePath, int HeaderRowIndex) {
            using (FileStream stream = System.IO.File.OpenRead(ExcelFilePath)) {
                return ImportDataSetFromExcel(stream, HeaderRowIndex);
            }
        }


        /// <summary>  
        /// 将Excel的列索引转换为列名，列索引从0开始，列名从A开始。如第0列为A，第1列为B...   
        /// </summary>   
        /// <param name="Index">列索引</param>   
        /// <returns>列名，如第0列为A，第1列为B...</returns>   
        //public static string ConvertColumnIndexToColumnName(int Index) {
        //    Index = Index + 1;
        //    int system = 26;
        //    char[] digArray = new char[100];
        //    int i = 0;
        //    while (Index > 0) {
        //        int mod = Index % system;
        //        if (mod == 0)
        //            mod = system;
        //        digArray[i++] = (char)(mod - 1 + 'A');
        //        Index = (Index - 1) / 26;
        //    }
        //    StringBuilder sb = new StringBuilder(i);
        //    for (int j = i - 1; j >= 0; j--) {
        //        sb.Append(digArray[j]);
        //    }
        //    return sb.ToString();
        //}
    }
}