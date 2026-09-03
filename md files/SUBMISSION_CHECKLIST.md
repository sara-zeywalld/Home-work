# Submission Checklist

## Required Deliverables

- [x] Short specification: `SPECIFICATION.md`
- [x] ERD and database design: `DB_ERD.md`
- [x] SQL DDL, indexes, and queries: `SQL.md`
- [x] Dynamic-data solution and import workflow: `ARCHITECTURE.md` and `SPECIFICATION.md`
- [x] At least two alternatives and a decision: `DECISIONS.md`
- [x] Screen sketches: `UI.md`
- [x] API design: `API.md`
- [ ] Angular implementation of Screen 3
- [ ] .NET Core Web API implementation
- [ ] SQLite migrations and seed data
- [ ] Git repository link
- [ ] PDF or Word export of the documentation
- [ ] Screen recording showing the working application

## AI Disclosure

Example text for the submission:

> I used GitHub Copilot as an AI-assisted development and documentation tool. I reviewed, tested, and remain responsible for the submitted design and code. I can explain the database structure, dynamic JSON strategy, schema versioning, SQL queries, API design, and the alternatives considered.

Attach the relevant AI conversation history as requested by the assignment. Do not claim that AI performed testing or decisions that were not actually reviewed by the candidate.

## Recommended Three-Hour Scope

1. Implement SQLite schema, seed three metrics, and create one sample premium method.
2. Implement `GET /metrics`, `GET /metrics/{id}/fields`, `POST /metrics/{id}/imports`, and paginated `GET /metrics/{id}/data`.
3. Implement Angular Screen 3 with metric selection, upload, import history, dynamic table, filters, sorting, and pagination.
4. Add one schema-change demonstration: import employees v1, then v2 with manager and location.
5. Add a README with setup commands and a short explanation of the design.
6. Record the demo and export these markdown files to PDF.

## Final Explanation Points

Be ready to explain:

- Why core metadata is relational.
- Why the variable Excel row is stored as JSON.
- Why schema versions are immutable.
- Why field metadata and whitelisted JSON paths are used for filtering and sorting.
- How old imports remain readable after a file layout changes.
- How arbitrary sort fields are whitelisted to avoid SQL injection.
- What would change for SQL Server and very large asynchronous imports.
