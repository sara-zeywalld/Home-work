# Architecture and Main Flow

## Layers

```text
Angular UI
  -> HTTP API clients
.NET Core Web API Controllers
  -> Application services / use cases
  -> Domain entities and validation rules
  -> Repositories and Excel importer abstraction
  -> EF Core / SQLite
```

The controller handles HTTP concerns only. The application service coordinates a use case such as `ImportMetricFile`. The Excel reader is behind an interface, so the domain does not depend on a particular spreadsheet library. Repositories isolate persistence.

## Main Import Components

- `MetricController`: metric, field, schema, and data endpoints.
- `ImportController`: upload and import-history endpoints.
- `ImportMetricFileService`: validates the request, detects the schema, maps fields, and persists the batch.
- `IExcelReader`: returns normalized headers and row dictionaries.
- `ISchemaMatcher`: calculates and matches schema fingerprints.
- `IImportRepository`: writes the batch and JSON rows, including row validation results, in one transaction.
- `DynamicQueryService`: validates requested fields against metadata, then applies filters, sorting, and pagination.

## Transaction Boundary

Create the import batch as `Processing`, persist rows and row validation results in a transaction, update the row count and final status, then commit. For large files, process in chunks and use a resumable status model rather than keeping one very long transaction. The three-hour test can implement the simple transaction first and document chunking as the production extension.

## Idempotency

Calculate a SHA-256 file hash. Combine it with metric and business period. Reject an identical completed upload as a duplicate, or return the existing batch ID. This avoids double-counting when a user retries an upload.

## Angular Screen 3 State

- Reactive form state: metric, period, file, and optional schema version.
- Query state: page, page size, sort field, sort direction, and typed filters.
- Data state: columns, rows, total count, import history, loading, and error state.
- Keep query state in the URL where practical so a view can be refreshed or shared.
- Use server-side pagination and debounce text filters to prevent excessive requests.
