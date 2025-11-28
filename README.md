## QTip – Solution Overview

QTip is a small end to end system that detects email addresses in free text input, replaces them with tokens for safe storage, and keeps a separate vault of the original values so you can compute statistics like the total number of PII emails submitted.  
This document focuses on explaining the **thought process and problem‑solving approach** taken to implement the QTip coding challenge, rather than listing requirements (those are now in `Instructions.md`). The sections below cover how to run the app, the architecture and assumptions, key trade‑offs, and notes about the optional extension.

---

## Instructions for running the application

<details>
<summary>Running with Docker (end‑to‑end)</summary>

<br />

- Requirements: Docker Desktop (or compatible Docker environment).
- From the repository root:

```bash
docker compose up --build
```

- Services:
  - Backend API: exposed on `http://localhost:5000`
  - Frontend: exposed on `http://localhost:3000`
  - PostgreSQL database: internal service `db` on port `5432` with database `qtip`, user `qtip`, password `qtip`
- Open the UI at `http://localhost:3000`, paste some text with emails, and submit. The stats panel will show `Total PII emails submitted: X` aggregated from the backend.

</details>

<details>
<summary>Running locally for debugging (frontend + API, DB in Docker)</summary>

<br />

1. **Start PostgreSQL via Docker (once per session):**

```bash
docker start qtip-db  # if it already exists
```

If the container does not exist yet:

```bash
docker run --name qtip-db -e POSTGRES_USER=qtip -e POSTGRES_PASSWORD=qtip -e POSTGRES_DB=qtip -p 5432:5432 -d postgres:16
```

2. **Run the API locally:**

- `QTip.Api/appsettings.Development.json` contains:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=qtip;Username=qtip;Password=qtip"
  }
}
```

- From the repo root:

```bash
cd QTip.Api
dotnet run
```

- The API listens on `http://localhost:5001` (per `launchSettings.json`). Swagger is available at `http://localhost:5001/swagger`.

3. **Run the frontend locally:**

- In `frontend/.env.local`:

```ini
NEXT_PUBLIC_API_BASE_URL=http://localhost:5001
```

- From the repo root:

```bash
cd frontend
npm install
npm run dev
```

- Open `http://localhost:3000` to use the app with full debugging support in your editor and browser dev tools.

</details>

---

## Trade‑offs and decisions made

### 1. EF Core code‑first with `EnsureCreated` (no migrations checked in)

- **Decision**: Use EF Core’s code‑first model and `Database.EnsureCreated()` at startup instead of maintaining explicit migrations in this repo.  
- **Reasoning**: For a small challenge with a simple schema, this keeps setup and maintenance very light while still giving a real relational database and clear schema.  
- **Trade‑off**: This approach is not ideal for production schema evolution, but it is acceptable  for this tech test.

### 2. From email‑only detection to a generalized classification service

- **Decision**: Start with a dedicated `IEmailDetectionService` and later evolve to a single `IClassificationDetectionService` that detects multiple configured types (email, IBAN, phone, tokens) using regex patterns and enum‑backed tags.  
- **Reasoning**: Beginning with email only kept the initial pipeline simple and easy to validate; once that was correct, it was generalized to support additional classification types without changing the controller or command.

### 3. Regex‑based detection for all types

- **Decision**: Use simple regexes for email, IBAN, phone, and tokens instead of external libraries or strict format validators.  
- **Reasoning**: Regex is sufficient here to demonstrate detection behavior and tokenization, and keeps the implementation easy to understand and adjust.  
- **Trade‑off**: Patterns are deliberately simplified; they may miss some valid real‑world formats or match slightly more than a production grade validator would.
