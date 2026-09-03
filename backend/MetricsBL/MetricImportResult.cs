namespace backend.MetricsBL;

public sealed record ExcelImportData(IReadOnlyList<string> Headers, IReadOnlyList<IReadOnlyDictionary<string, string?>> Rows);
public sealed record ImportResult(int Id, string FileName, int RowCount, int SchemaVersion);
