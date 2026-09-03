# SQL Examples

The examples use SQLite syntax. In a production SQL Server implementation, replace `INTEGER PRIMARY KEY` with the chosen SQL Server key strategy and `TEXT` JSON storage with `nvarchar(max)` plus JSON functions where appropriate.

## Core DDL

```sql
CREATE TABLE premium_method (
    id INTEGER PRIMARY KEY,
    method_number TEXT NOT NULL UNIQUE,
    description TEXT NOT NULL,
    premium_percentage NUMERIC NOT NULL CHECK (premium_percentage >= 0),
    calculation_period TEXT NOT NULL,
    status TEXT NOT NULL DEFAULT 'Active'
        CHECK (status IN ('Active', 'Inactive'))
);

CREATE TABLE metric (
    id INTEGER PRIMARY KEY,
    premium_method_id INTEGER NOT NULL REFERENCES premium_method(id),
    name TEXT NOT NULL UNIQUE,
    description TEXT NOT NULL,
    source_type TEXT NOT NULL CHECK (source_type IN ('Excel', 'ExternalInterface')),
    source_name TEXT NOT NULL,
    ingestion_frequency TEXT NOT NULL,
    status TEXT NOT NULL DEFAULT 'Active'
        CHECK (status IN ('Active', 'Inactive'))
);

CREATE TABLE metric_schema_version (
    id INTEGER PRIMARY KEY,
    metric_id INTEGER NOT NULL REFERENCES metric(id),
    version_number INTEGER NOT NULL,
    fingerprint TEXT NOT NULL,
    status TEXT NOT NULL CHECK (status IN ('Draft', 'Active', 'Retired')),
    created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (metric_id, version_number),
    UNIQUE (metric_id, fingerprint)
);

CREATE TABLE metric_field (
    id INTEGER PRIMARY KEY,
    schema_version_id INTEGER NOT NULL REFERENCES metric_schema_version(id),
    field_key TEXT NOT NULL,
    source_column_name TEXT NOT NULL,
    display_name TEXT NOT NULL,
    data_type TEXT NOT NULL CHECK (data_type IN ('Text', 'Number', 'Date', 'Boolean')),
    is_relevant INTEGER NOT NULL DEFAULT 1 CHECK (is_relevant IN (0, 1)),
    is_required INTEGER NOT NULL DEFAULT 0 CHECK (is_required IN (0, 1)),
    display_order INTEGER NOT NULL DEFAULT 0,
    UNIQUE (schema_version_id, field_key),
    UNIQUE (schema_version_id, source_column_name)
);

CREATE TABLE import_batch (
    id INTEGER PRIMARY KEY,
    schema_version_id INTEGER NOT NULL REFERENCES metric_schema_version(id),
    file_name TEXT NOT NULL,
    file_hash TEXT NOT NULL,
    period_year INTEGER NOT NULL,
    period_value TEXT NOT NULL,
    status TEXT NOT NULL CHECK (status IN ('Processing', 'Completed', 'CompletedWithErrors', 'Failed')),
    row_count INTEGER NOT NULL DEFAULT 0,
    error_message TEXT,
    started_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    completed_at TEXT,
    UNIQUE (schema_version_id, file_hash, period_year, period_value)
);

CREATE TABLE import_row (
    id INTEGER PRIMARY KEY,
    import_batch_id INTEGER NOT NULL REFERENCES import_batch(id),
    row_number INTEGER NOT NULL,
    data_json TEXT NOT NULL CHECK (json_valid(data_json)),
    status TEXT NOT NULL DEFAULT 'Valid'
        CHECK (status IN ('Valid', 'Invalid')),
    error_message TEXT,
    created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (import_batch_id, row_number)
);
```

## Indexes

```sql
CREATE INDEX ix_schema_metric ON metric_schema_version(metric_id, version_number);
CREATE INDEX ix_import_batch_lookup
    ON import_batch(schema_version_id, period_year, period_value, status);
CREATE INDEX ix_import_row_batch
    ON import_row(import_batch_id, row_number);
```

## Dynamic Data Query

The API should whitelist `field_key` and sort direction using the selected schema metadata. Never concatenate an arbitrary request field into SQL.

```sql
SELECT
    d.id,
    d.row_number,
    d.data_json
FROM import_row AS d
JOIN import_batch AS b ON b.id = d.import_batch_id
JOIN metric_schema_version AS s ON s.id = b.schema_version_id
WHERE s.metric_id = :metric_id
  AND (:batch_id IS NULL OR b.id = :batch_id)
ORDER BY d.row_number
LIMIT :page_size OFFSET :offset;
```

For filtering and sorting, the API first validates the requested `field_key` against the selected schema fields, then uses a parameterized SQLite JSON query. The UI renders `data_json` using the field definitions returned by the API.

## Import History Query

```sql
SELECT id, file_name, period_year, period_value, status,
       row_count, started_at, completed_at, error_message
FROM import_batch
WHERE schema_version_id IN (
    SELECT id FROM metric_schema_version WHERE metric_id = :metric_id
)
ORDER BY started_at DESC
LIMIT :page_size OFFSET :offset;
```
