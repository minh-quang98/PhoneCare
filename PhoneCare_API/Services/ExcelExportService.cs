using System.Globalization;
using System.IO.Compression;
using System.Security;
using System.Text;

namespace PhoneCare_API.Services
{
    public static class ExcelExportService
    {
        public static byte[] Create(
            string sheetName,
            IReadOnlyList<string> headers,
            IReadOnlyList<IReadOnlyList<object?>> rows)
        {
            using var output = new MemoryStream();
            using (var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true))
            {
                WriteEntry(archive, "[Content_Types].xml", CreateContentTypesXml());
                WriteEntry(archive, "_rels/.rels", CreateRootRelationshipsXml());
                WriteEntry(archive, "xl/workbook.xml", CreateWorkbookXml(sheetName));
                WriteEntry(archive, "xl/_rels/workbook.xml.rels", CreateWorkbookRelationshipsXml());
                WriteEntry(archive, "xl/styles.xml", CreateStylesXml());
                WriteEntry(archive, "xl/worksheets/sheet1.xml", CreateWorksheetXml(headers, rows));
            }

            return output.ToArray();
        }

        private static void WriteEntry(ZipArchive archive, string path, string content)
        {
            var entry = archive.CreateEntry(path, CompressionLevel.Optimal);
            using var stream = entry.Open();
            using var writer = new StreamWriter(stream, new UTF8Encoding(false));
            writer.Write(content);
        }

        private static string CreateWorksheetXml(
            IReadOnlyList<string> headers,
            IReadOnlyList<IReadOnlyList<object?>> rows)
        {
            var lastColumn = GetColumnName(headers.Count);
            var lastRow = rows.Count + 1;
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            xml.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");
            xml.Append("<dimension ref=\"A1:").Append(lastColumn).Append(lastRow).Append("\"/>");
            xml.Append("<sheetViews><sheetView workbookViewId=\"0\"><pane ySplit=\"1\" topLeftCell=\"A2\" activePane=\"bottomLeft\" state=\"frozen\"/></sheetView></sheetViews>");
            AppendColumns(xml, headers, rows);
            xml.Append("<sheetData>");
            AppendRow(xml, 1, headers.Cast<object?>().ToArray(), header: true);

            for (var index = 0; index < rows.Count; index++)
            {
                AppendRow(xml, index + 2, rows[index], header: false);
            }

            xml.Append("</sheetData>");
            xml.Append("<autoFilter ref=\"A1:").Append(lastColumn).Append(lastRow).Append("\"/>");
            xml.Append("</worksheet>");
            return xml.ToString();
        }

        private static void AppendColumns(
            StringBuilder xml,
            IReadOnlyList<string> headers,
            IReadOnlyList<IReadOnlyList<object?>> rows)
        {
            xml.Append("<cols>");
            for (var column = 0; column < headers.Count; column++)
            {
                var maxLength = headers[column].Length;
                foreach (var row in rows)
                {
                    if (column < row.Count)
                    {
                        maxLength = Math.Max(maxLength, Convert.ToString(row[column], CultureInfo.InvariantCulture)?.Length ?? 0);
                    }
                }

                var width = Math.Clamp(maxLength + 2, 8, 35);
                xml.Append("<col min=\"").Append(column + 1).Append("\" max=\"").Append(column + 1)
                    .Append("\" width=\"").Append(width).Append("\" customWidth=\"1\"/>");
            }
            xml.Append("</cols>");
        }

        private static void AppendRow(StringBuilder xml, int rowIndex, IReadOnlyList<object?> values, bool header)
        {
            xml.Append("<row r=\"").Append(rowIndex).Append("\">");
            for (var column = 0; column < values.Count; column++)
            {
                var cellReference = GetColumnName(column + 1) + rowIndex;
                var value = values[column];
                if (value is DateTime date)
                {
                    xml.Append("<c r=\"").Append(cellReference).Append("\" s=\"2\"><v>")
                        .Append(date.ToOADate().ToString(CultureInfo.InvariantCulture)).Append("</v></c>");
                }
                else if (IsNumber(value))
                {
                    xml.Append("<c r=\"").Append(cellReference).Append("\"><v>")
                        .Append(Convert.ToString(value, CultureInfo.InvariantCulture)).Append("</v></c>");
                }
                else
                {
                    xml.Append("<c r=\"").Append(cellReference).Append("\" t=\"inlineStr\"");
                    if (header) xml.Append(" s=\"1\"");
                    xml.Append("><is><t xml:space=\"preserve\">").Append(Escape(Convert.ToString(value) ?? string.Empty))
                        .Append("</t></is></c>");
                }
            }
            xml.Append("</row>");
        }

        private static bool IsNumber(object? value)
        {
            return value is byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal;
        }

        private static string GetColumnName(int columnNumber)
        {
            var name = string.Empty;
            while (columnNumber > 0)
            {
                var remainder = (columnNumber - 1) % 26;
                name = Convert.ToChar('A' + remainder) + name;
                columnNumber = (columnNumber - remainder) / 26;
            }
            return name;
        }

        private static string Escape(string value)
        {
            var validXml = new string(value.Where(character => character is '\t' or '\n' or '\r' || character >= ' ').ToArray());
            return SecurityElement.Escape(validXml) ?? string.Empty;
        }

        private static string CreateContentTypesXml() => """
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
              <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
              <Default Extension="xml" ContentType="application/xml"/>
              <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
              <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
              <Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>
            </Types>
            """;

        private static string CreateRootRelationshipsXml() => """
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
              <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
            </Relationships>
            """;

        private static string CreateWorkbookRelationshipsXml() => """
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
              <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
              <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
            </Relationships>
            """;

        private static string CreateWorkbookXml(string sheetName) => $"""
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
              <sheets><sheet name="{Escape(sheetName)}" sheetId="1" r:id="rId1"/></sheets>
            </workbook>
            """;

        private static string CreateStylesXml() => """
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">
              <fonts count="2"><font><sz val="11"/><name val="Calibri"/></font><font><b/><color rgb="FFFFFFFF"/><sz val="11"/><name val="Calibri"/></font></fonts>
              <fills count="3"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill><fill><patternFill patternType="solid"><fgColor rgb="FF1F4E78"/><bgColor indexed="64"/></patternFill></fill></fills>
              <borders count="1"><border><left/><right/><top/><bottom/><diagonal/></border></borders>
              <cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs>
              <cellXfs count="3"><xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/><xf numFmtId="0" fontId="1" fillId="2" borderId="0" xfId="0" applyFont="1" applyFill="1" applyAlignment="1"><alignment horizontal="center"/></xf><xf numFmtId="22" fontId="0" fillId="0" borderId="0" xfId="0" applyNumberFormat="1"/></cellXfs>
            </styleSheet>
            """;
    }
}
