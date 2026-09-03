# Functional Specification

## Goal

Build a premium-method management module that lets users define premium methods and metrics, configure the expected Excel structure for each metric, upload files for a business period, and view imported rows dynamically.

## Main Use Cases

1. Create, edit, activate, and deactivate a premium method.
2. Define a metric and associate it with one premium method.
3. Define the relevant fields and data types for a metric's Excel structure.
4. Upload an Excel file for a metric and period.
5. Detect the file header and match it to an existing schema version.
6. Create a new schema version when the structure is new or changed.
7. Validate and import rows while preserving the original row JSON.
8. View import history, validation errors, and imported data.
9. Filter, sort, and paginate dynamic columns.

## Import Workflow

1. The user selects a metric, year, period, and Excel file.
2. The API validates the extension, size, file hash, and required period values.
3. The import service reads the header and normalizes column names for comparison.
4. A deterministic fingerprint is calculated from the ordered headers and detected types.
5. The fingerprint is matched to a `metric_schema_version`.
6. If no version matches, the system creates a draft version or asks the user to map columns and select relevant fields.
7. The service validates each row according to the field definitions.
8. The system writes an `import_batch` and one `import_row` per source row. Each row keeps its source values in `data_json`.
9. The batch is marked `Completed`, `CompletedWithErrors`, or `Failed`.
10. The UI refreshes import history and displays the selected batch using its schema version.

## Schema Change Example

When the employees file gains `direct_manager` and `location`, the system compares the new header fingerprint with the previous version. It creates a new schema version, keeps the old version immutable, and imports new rows under the new version. Existing rows remain readable because each row points to the schema version used at import time. No database table change is required. A code change is not required if field mapping and validation are configuration-driven.

## Non-Functional Requirements

- Use Angular reactive forms for all configuration and upload forms.
- Use .NET Core Web API with controller, application-service, domain, and infrastructure boundaries.
- Use SQLite for the test, while keeping relational constraints compatible with SQL Server.
- Use server-side filtering, sorting, and pagination for imported data.
- Do not load an entire large import into the browser.
- Return validation errors with row number, field, and reason.
- Keep imports auditable and idempotent through file hashes and period checks.
