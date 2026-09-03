using System.Text.Json;
using System.Text.Json.Serialization;
using backend.MetricsBL;
using backend.MetricsBL.Interfaces;
using backend.MetricsDAL;
using backend.MetricsDAL.Import;
using backend.MetricsDAL.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=premium-methods.db"));
builder.Services.AddScoped<IMetricRepository, MetricRepository>();
builder.Services.AddScoped<IPremiumMethodRepository, PremiumMethodRepository>();
builder.Services.AddScoped<IMetricService, MetricService>();
builder.Services.AddScoped<IPremiumMethodService, PremiumMethodService>();
builder.Services.AddScoped<IExcelImportReader, ExcelImportReader>();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddOpenApi();
builder.Services.AddCors(options => options.AddPolicy("frontend", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    await SeedData.InitializeAsync(db);
}
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseCors("frontend");
app.MapControllers();

app.Run();

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        if (await db.PremiumMethods.AnyAsync()) return;
        var method = new PremiumMethod { Number = "PM-2026-01", Description = "Annual performance premium", PremiumPercent = 7.5m, CalculationPeriod = "Quarterly" };
        var employees = new Metric { Code = "employees", Name = "Employees", Description = "Workforce status and organisation", SourceName = "HRIS Excel export" };
        employees.Fields.AddRange([
            new MetricField { Version = 1, Key = "employeeId", Label = "Employee ID", DataType = "text", DisplayOrder = 1 },
            new MetricField { Version = 1, Key = "employeeName", Label = "Employee name", DataType = "text", DisplayOrder = 2 },
            new MetricField { Version = 1, Key = "department", Label = "Department", DataType = "text", DisplayOrder = 3 },
            new MetricField { Version = 1, Key = "status", Label = "Status", DataType = "text", DisplayOrder = 4 },
            new MetricField { Version = 2, Key = "manager", Label = "Direct manager", DataType = "text", DisplayOrder = 5, IsRelevant = false },
            new MetricField { Version = 2, Key = "location", Label = "Location", DataType = "text", DisplayOrder = 6, IsRelevant = false }
        ]);
        method.Metrics.Add(employees);
        db.PremiumMethods.Add(method);
        await db.SaveChangesAsync();
        var batch = new ImportBatch { MetricId = employees.Id, SchemaVersion = 1, PeriodYear = 2026, PeriodLabel = "Q1", FileName = "employees-q1-2026.xlsx", ImportedAtUtc = DateTime.UtcNow, RowCount = 3 };
        batch.Rows.AddRange([
            new MetricRow { ValuesJson = JsonSerializer.Serialize(new { employeeId = "10482", employeeName = "Maya Cohen", department = "Operations", status = "Active" }) },
            new MetricRow { ValuesJson = JsonSerializer.Serialize(new { employeeId = "10831", employeeName = "Noam Levi", department = "Finance", status = "Active" }) },
            new MetricRow { ValuesJson = JsonSerializer.Serialize(new { employeeId = "10907", employeeName = "Dana Shaham", department = "Operations", status = "On leave" }) }
        ]);
        db.ImportBatches.Add(batch);
        await db.SaveChangesAsync();
    }
}
