# API Design

Base route: `/api`

## Premium Methods

- `GET /premium-methods?page=1&pageSize=20` - list methods.
- `POST /premium-methods` - create a method.
- `GET /premium-methods/{id}` - get a method with its metrics.
- `PUT /premium-methods/{id}` - update method metadata.
- `PATCH /premium-methods/{id}/status` - activate or deactivate.

Example create request:

```json
{
  "methodNumber": "PM-001",
  "description": "Quarterly employee premium",
  "premiumPercentage": 12.5,
  "calculationPeriod": "Quarterly"
}
```

## Metrics and Schema Definitions

- `GET /metrics` - list metrics.
- `POST /premium-methods/{premiumMethodId}/metrics` - create a metric belonging to one premium method.
- `GET /metrics/{id}` - get metric metadata and current schema.
- `PUT /metrics/{id}` - update metric metadata.
- `GET /metrics/{id}/schema-versions` - list immutable schema versions.
- `GET /metrics/{id}/schema-versions/{versionId}` - get fields and mappings.
- `POST /metrics/{id}/schema-versions` - create or configure a schema version.
- `PUT /schema-versions/{versionId}/fields` - replace draft field mappings.

## File Import

- `POST /metrics/{metricId}/imports` - upload an Excel file with `multipart/form-data`.
- `GET /metrics/{metricId}/imports` - list import history with period and status filters.
- `GET /imports/{importId}` - get batch details and validation summary.
- `GET /imports/{importId}/errors` - get row-level errors.

Upload form fields:

- `file`: Excel file.
- `periodYear`: integer.
- `periodValue`: month or quarter value.
- `schemaVersionId`: optional when the user selects an existing version.

The upload endpoint should return `202 Accepted` for an asynchronous import or `201 Created` for the small synchronous test implementation. The response includes the batch ID and status.

## Dynamic Data

- `GET /metrics/{metricId}/fields` - return fields for the selected schema/version, including data type, display label, relevance, and filter capability.
- `GET /metrics/{metricId}/data?batchId=10&page=1&pageSize=50&sortField=employee_id&sortDirection=asc` - return paginated rows and dynamic columns.

Example response:

```json
{
  "columns": [
    { "key": "employee_id", "label": "ID", "dataType": "Text", "filterable": true },
    { "key": "employee_name", "label": "Employee name", "dataType": "Text", "filterable": true },
    { "key": "status", "label": "Status", "dataType": "Text", "filterable": true }
  ],
  "rows": [
    { "id": 1001, "rowNumber": 2, "values": { "employee_id": "123", "employee_name": "Dana", "status": "Active" } }
  ],
  "page": 1,
  "pageSize": 50,
  "totalCount": 1250
}
```

## Validation and Errors

Return `400 Bad Request` for invalid mappings or filters, `409 Conflict` for duplicate file-period imports, `404 Not Found` for unknown IDs, and `422 Unprocessable Entity` for row validation failures where the batch itself is created but contains rejected rows.
