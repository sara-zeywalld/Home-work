using System.Data;
using System.Text;
using ExcelDataReader;

namespace backend.MetricsDAL.Import;

public class ExcelImportReader : IExcelImportReader
{
    public async Task<MetricsBL.ExcelImportData> ReadAsync(Stream stream, string fileName)
    {
        if (Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return await ReadCsvAsync(stream);
        }

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration { ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true } });
        var table = dataSet.Tables[0];
        var headers = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();
        var rows = table.Rows.Cast<DataRow>().Select(row => (IReadOnlyDictionary<string, string?>)headers.ToDictionary(header => header, header => row[header] == DBNull.Value ? null : row[header]?.ToString(), StringComparer.OrdinalIgnoreCase)).ToList();
        return new MetricsBL.ExcelImportData(headers, rows);
    }

    private static async Task<MetricsBL.ExcelImportData> ReadCsvAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var rows = new List<IReadOnlyDictionary<string, string?>>();
        var headers = await ReadCsvRecordAsync(reader);
        if (headers.Count == 0) return new MetricsBL.ExcelImportData([], rows);

        while (true)
        {
            var values = await ReadCsvRecordAsync(reader);
            if (values.Count == 0 && reader.EndOfStream) break;
            if (values.Count == 0) continue;
            var row = headers.Select((header, index) => new
            {
                Header = header,
                Value = index < values.Count ? values[index] : null
            }).ToDictionary(item => item.Header, item => item.Value, StringComparer.OrdinalIgnoreCase);
            rows.Add(row);
        }

        return new MetricsBL.ExcelImportData(headers, rows);
    }

    private static async Task<List<string>> ReadCsvRecordAsync(StreamReader reader)
    {
        var values = new List<string>();
        var value = new StringBuilder();
        var quoted = false;
        var hasContent = false;

        while (true)
        {
            var character = reader.Read();
            if (character == -1)
            {
                if (hasContent || value.Length > 0 || values.Count > 0) values.Add(value.ToString());
                return values;
            }

            hasContent = true;
            var current = (char)character;
            if (current == '"')
            {
                if (quoted && reader.Peek() == '"')
                {
                    value.Append((char)reader.Read());
                }
                else
                {
                    quoted = !quoted;
                }
            }
            else if (current == ',' && !quoted)
            {
                values.Add(value.ToString());
                value.Clear();
            }
            else if ((current == '\r' || current == '\n') && !quoted)
            {
                if (current == '\r' && reader.Peek() == '\n') reader.Read();
                values.Add(value.ToString());
                return values;
            }
            else
            {
                value.Append(current);
            }
        }
    }
}
