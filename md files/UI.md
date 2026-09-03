# Screen Sketches

The test prioritizes working logic over visual polish. Use a compact Angular layout with clear loading, empty, validation-error, and server-error states.

## Screen 1: Premium Methods

```text
[Premium Methods]                              [+ New method]

Search: [____________]  Status: [All v]

Method no. | Description             | Premium % | Period    | Status | Actions
PM-001     | Quarterly employee      | 12.5      | Quarterly | Active | Edit / Disable
```

The edit form includes method number, description, percentage, calculation period, status, and a multi-select list of metrics.

## Screen 2: Metrics

```text
[Metrics]                                      [+ New metric]

Search: [____________]  Source: [Excel v]

Name      | Source | Frequency | Current schema | Status | Actions
Employees | Excel  | Monthly   | v2             | Active | Fields / Edit
```

The metric form includes name, description, source type, source name, ingestion frequency, and field configuration. Field configuration includes source column name, stable field key, display label, type, relevant, required, and display order.

## Screen 3: Import and View Data

```text
[Employees]

Upload file: [Choose file]  Year: [2026]  Period: [Q1 v] [Upload]

Import history
Date       | File                    | Version | Rows | Status
2026-01-12 | employees-q1.xlsx       | v2      | 500  | Completed

Data                         [Filters] [Clear]
ID [____]  Status [Active v]  [Apply]

ID   | Employee name | Department | Status | Manager | Location
123  | Dana          | Finance    | Active | ...     | ...

[< Previous] Page 1 of 10 [Next >]
```

The table is generated from the `columns` response rather than hard-coded metric fields. Render dates, numbers, and booleans according to `dataType`. Use server-side filtering, sorting, and pagination. Do not render thousands of rows at once.

## Angular Implementation Notes

- Use `FormBuilder` and typed reactive forms for upload and configuration forms.
- Disable upload while a file is processing and show row-level validation errors from the API.
- Reload fields before rendering a selected batch because different batches can use different schema versions.
- Keep the selected metric, batch, page, and filters in component state; use a route resolver or explicit loading state for initial data.
