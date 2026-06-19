using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Security;
using System.Text;

namespace PhoneCare.Class
{
    public static class ExcelExporter
    {
        /// <summary>
        /// Xuất danh sách dữ liệu thành tệp Excel theo tiêu đề và tên sheet được cung cấp.
        /// </summary>
        public static void Export(string fileName, string sheetName, IList<string> headers, IList<IList<string>> rows)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (var package = Package.Open(fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                var workbookUri = new Uri("/xl/workbook.xml", UriKind.Relative);
                var worksheetUri = new Uri("/xl/worksheets/sheet1.xml", UriKind.Relative);
                var stylesUri = new Uri("/xl/styles.xml", UriKind.Relative);

                WritePart(package, workbookUri, CreateWorkbookXml(sheetName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml");
                WritePart(package, worksheetUri, CreateWorksheetXml(headers, rows), "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml");
                WritePart(package, stylesUri, CreateStylesXml(), "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml");

                package.CreateRelationship(workbookUri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "rId1");

                var workbookPart = package.GetPart(workbookUri);
                workbookPart.CreateRelationship(worksheetUri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet", "rId1");
                workbookPart.CreateRelationship(stylesUri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles", "rId2");
            }
        }

        /// <summary>
        /// Tạo và ghi một part XML vào package của tệp Excel.
        /// </summary>
        private static void WritePart(Package package, Uri uri, string content, string contentType)
        {
            var part = package.CreatePart(uri, contentType, CompressionOption.Maximum);
            using (var stream = part.GetStream(FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
            {
                writer.Write(content);
            }
        }

        /// <summary>
        /// Tạo nội dung XML mô tả workbook và worksheet Excel.
        /// </summary>
        private static string CreateWorkbookXml(string sheetName)
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                   "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" " +
                   "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                   "<sheets><sheet name=\"" + Escape(sheetName) + "\" sheetId=\"1\" r:id=\"rId1\"/></sheets>" +
                   "</workbook>";
        }

        /// <summary>
        /// Tạo nội dung XML chứa tiêu đề và các dòng dữ liệu của worksheet.
        /// </summary>
        private static string CreateWorksheetXml(IList<string> headers, IList<IList<string>> rows)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            xml.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");
            xml.Append("<sheetData>");
            AppendRow(xml, 1, headers, true);

            for (int i = 0; i < rows.Count; i++)
            {
                AppendRow(xml, i + 2, rows[i], false);
            }

            xml.Append("</sheetData>");
            xml.Append("</worksheet>");
            return xml.ToString();
        }

        /// <summary>
        /// Tạo nội dung XML định nghĩa kiểu hiển thị cho tệp Excel.
        /// </summary>
        private static string CreateStylesXml()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                   "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
                   "<fonts count=\"2\"><font><sz val=\"11\"/><name val=\"Calibri\"/></font><font><b/><sz val=\"11\"/><name val=\"Calibri\"/></font></fonts>" +
                   "<fills count=\"1\"><fill><patternFill patternType=\"none\"/></fill></fills>" +
                   "<borders count=\"1\"><border><left/><right/><top/><bottom/><diagonal/></border></borders>" +
                   "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>" +
                   "<cellXfs count=\"2\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/><xf numFmtId=\"0\" fontId=\"1\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyFont=\"1\"/></cellXfs>" +
                   "</styleSheet>";
        }

        /// <summary>
        /// Ghi một dòng dữ liệu vào nội dung XML của worksheet.
        /// </summary>
        private static void AppendRow(StringBuilder xml, int rowIndex, IList<string> values, bool header)
        {
            xml.Append("<row r=\"").Append(rowIndex).Append("\">");
            for (int i = 0; i < values.Count; i++)
            {
                xml.Append("<c r=\"").Append(GetColumnName(i + 1)).Append(rowIndex).Append("\" t=\"inlineStr\"");
                if (header) xml.Append(" s=\"1\"");
                xml.Append("><is><t>");
                xml.Append(Escape(values[i]));
                xml.Append("</t></is></c>");
            }
            xml.Append("</row>");
        }

        /// <summary>
        /// Chuyển số thứ tự cột thành tên cột Excel.
        /// </summary>
        private static string GetColumnName(int columnNumber)
        {
            var columnName = string.Empty;
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }

        /// <summary>
        /// Mã hóa các ký tự đặc biệt để giá trị có thể ghi an toàn vào XML.
        /// </summary>
        private static string Escape(string value)
        {
            return SecurityElement.Escape(value ?? string.Empty);
        }
    }
}
