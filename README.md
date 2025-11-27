## QTip – Solution Overview

This document focuses on explaining the **thought process and problem‑solving approach** taken to implement the QTip coding challenge, rather than listing requirements (those are now in `Instructions.md`). The sections below cover how to run the app, the architecture and assumptions, key trade‑offs, and notes about the optional extension.

---

## Instructions for running the application

### Running with Docker (end‑to‑end)

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

### Running locally for debugging (frontend + API, DB in Docker)

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

---

