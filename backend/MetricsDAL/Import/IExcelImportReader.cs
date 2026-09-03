namespace backend.MetricsDAL.Import;

public interface IExcelImportReader
{
    Task<MetricsBL.ExcelImportData> ReadAsync(Stream stream, string fileName);
}
