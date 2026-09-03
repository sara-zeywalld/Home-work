# Architectural Alternatives and Decision

## Alternative A: One JSON Document Per Imported Row

All source columns are stored in `import_row.data_json`; metric fields and schema versions describe how to interpret the document.

- **Advantages:** maximum flexibility; new metrics and columns require no table migration; simple dynamic rendering; preserves the original row.
- **Disadvantages:** typed filtering and sorting are harder; JSON queries may be slower; constraints are mostly application-level; indexes require generated columns or a projection table.
- **Development complexity:** low to medium.
- **Maintenance complexity:** low for schema changes, medium for query and validation code.
- **Performance:** good for ingestion and display; weaker for complex searches over large datasets.
- **Search and filtering:** acceptable for simple JSON queries, weaker for many typed filters.
- **Future flexibility:** very high.

## Alternative B: Fully Relational Tables Per Metric

Each metric has dedicated tables and columns matching its Excel layout.

- **Advantages:** strong typing and constraints; excellent SQL querying, indexing, and reporting; straightforward filtering.
- **Disadvantages:** every new metric or changed Excel layout requires migrations and code changes; many tables and mappings become difficult to maintain; old layouts need extra compatibility handling.
- **Development complexity:** medium initially, high as metrics grow.
- **Maintenance complexity:** high.
- **Performance:** excellent for known query patterns.
- **Search and filtering:** excellent.
- **Future flexibility:** low to medium.

## Alternative C: JSON Rows Plus Relational Field Metadata

Keep the complete row in JSON and store field definitions, data types, relevance, and immutable schema versions relationally. Filter and sort only through fields that the API has whitelisted from the schema metadata.

- **Advantages:** flexible ingestion and display; strong metadata and history; no migration for every Excel change; old imports remain compatible; simple implementation for the test.
- **Disadvantages:** JSON filtering and sorting are slower than normal indexed columns; complex reporting may need a separate reporting model later.
- **Development complexity:** low to medium.
- **Maintenance complexity:** low to medium.
- **Performance:** good for paginated display and moderate data volumes; weaker for large analytical queries.
- **Search and filtering:** good for configured fields, but dependent on SQLite JSON functions and validated field paths.
- **Future flexibility:** high.

## Decision

Choose Alternative C. It best balances the assignment's central requirement, which is flexible and non-homogeneous Excel data, with the need to demonstrate SQL, validation, filtering, and a relational design. It is also realistic to implement within three hours. If the system later needs high-volume filtering, typed projection columns can be added as a measured optimization without changing the import contract.

## Tradeoff to Explain in the Interview

JSON is not being used as an excuse to abandon relational design. The relational tables own identity, relationships, configuration, schema history, and import lifecycle. JSON is limited to the variable source payload. If production query volume grows, frequently filtered fields can receive generated columns or a projection table as a later optimization.
