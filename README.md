# Signal Ledger

Full-stack solution for managing premium methods, metrics, and heterogeneous Excel inputs.

The complete assignment deliverables are in [SUBMISSION.md](SUBMISSION.md). AI usage is documented in [AI_USAGE.md](AI_USAGE.md).

## Run

Prerequisites: .NET SDK 9 and Node.js 20+.

```powershell
dotnet run --project backend --urls http://localhost:5006
npm start --prefix frontend
```

Open `http://localhost:4200`. The frontend expects the API at `http://localhost:5006`.

## Build and Test

```powershell
dotnet build backend/backend.csproj
npm run build --prefix frontend
npm test --prefix frontend -- --watch=false --browsers=ChromeHeadless
```
