namespace backend.Models;

public class PremiumMethod
{
    public int Id { get; set; }
    public string Number { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal PremiumPercent { get; set; }
    public string CalculationPeriod { get; set; } = "";
    public List<Metric> Metrics { get; set; } = [];
}

public class Metric
{
    public int Id { get; set; }
    public int PremiumMethodId { get; set; }
    public PremiumMethod? PremiumMethod { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string SourceType { get; set; } = "Excel";
    public string SourceName { get; set; } = "";
    public string IngestionFrequency { get; set; } = "Monthly";
    public List<MetricField> Fields { get; set; } = [];
    public List<ImportBatch> ImportBatches { get; set; } = [];
}

public class MetricField
{
    public int Id { get; set; }
    public int MetricId { get; set; }
    public Metric? Metric { get; set; }
    public int Version { get; set; }
    public string Key { get; set; } = "";
    public string Label { get; set; } = "";
    public string DataType { get; set; } = "text";
    public bool IsRelevant { get; set; } = true;
    public int DisplayOrder { get; set; }
}

public class ImportBatch
{
    public int Id { get; set; }
    public int MetricId { get; set; }
    public Metric? Metric { get; set; }
    public int SchemaVersion { get; set; }
    public int PeriodYear { get; set; }
    public string PeriodLabel { get; set; } = "";
    public string FileName { get; set; } = "";
    public DateTime ImportedAtUtc { get; set; }
    public int RowCount { get; set; }
    public string Status { get; set; } = "Completed";
    public List<MetricRow> Rows { get; set; } = [];
}

public class MetricRow
{
    public long Id { get; set; }
    public int ImportBatchId { get; set; }
    public ImportBatch? ImportBatch { get; set; }
    public string ValuesJson { get; set; } = "{}";
}
