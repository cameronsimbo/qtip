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

**Troubleshooting port conflicts**: If you get "ports are not available" errors (e.g., port 5432 already in use):
- Stop your local PostgreSQL service (`sudo systemctl stop postgresql` on Linux)

</details>

<details>
<summary>Running locally for debugging (frontend + API, DB in Docker)</summary>

<br />

1. **Clone the repository**

```bash
git clone <your-fork-or-repo-url>
cd qtip
```

2. **Start PostgreSQL (via Docker)**

```bash
docker run --name qtip-db \
  -e POSTGRES_USER=qtip \
  -e POSTGRES_PASSWORD=qtip \
  -e POSTGRES_DB=qtip \
  -p 5432:5432 \
  -d postgres:16
```

3. **Run the backend**

```bash
cd QTip.Api
dotnet run
```

The API listens on `http://localhost:5001`.

4. **Run the frontend**

Create or update `frontend/.env.local`:

```ini
NEXT_PUBLIC_API_BASE_URL=http://localhost:5001
```

Then:

```bash
cd ../frontend
npm install
npm run dev
```

Open `http://localhost:3000` in your browser.

</details>

---

## My thought process and how I approached the problem

<details>
<summary>My thought process and how I approached the problem</summary>

<br />

- **Bootstrap with AI for the heavy lifting**
  - I used AI to help generate the initial solution skeleton because the setup (four .NET projects, Next.js app, Docker, EF Core, MediatR, FluentValidation, PostgreSQL wiring) is boilerplate heavy and time consuming.
  - AI helped me quickly wire up the core dependencies and project structure so I could spend more time on design, domain decisions, and polishing the implementation.

- **Move into a test driven, iterative workflow**
  - Once the skeleton was in place, I followed a test driven style of working: write small focused tests, make them pass, then refactor.
  - I started with a simple email detection service, added tests early, and reused those tests as I refactored to catch regressions in the detection pipeline.

- **Evolve the design step by step**
  - I iterated on the architecture by:
    - Gradually reorganizing projects into `Domain`, `Application`, `Infrastructure`, and `Api`.
    - Introducing abstractions (e.g., `IClassificationDetectionService`, `IApplicationDbContext`, `ITokenGenerator`) only when they simplified reasoning or testing.
    - Refining logic in the MediatR handlers so that tokenization, persistence, and statistics were easy to understand and extend.

- **Extend beyond email and polish the UX**
  - After the email only version was stable, I implemented the optional extension with additional tags (phone numbers, IBANs, and security style tokens) using straightforward regex patterns.
  - I then added small UX improvements: a richer stats panel that shows both totals and last submission counts, and a clear stats button so it is easy to reset and re exercise the system.
  - I also added a light CI pipeline on GitHub Actions that builds the .NET solution, runs the tests, and builds the Next.js frontend so pull requests get clear build and test checkmarks.

</details>

---

## How tokenisation works

<details>
<summary>How tokenisation works</summary>

<br />

- **Detect sensitive values in the text**
  - When you submit text, the backend scans it with simple regex patterns to find emails, phone numbers, IBANs, and security-style tokens.
  - For each match, it records what was found, where it appears, and which tag it belongs to (for example `"pii.email"`).

- **Replace those values with tokens**
  - The handler walks through the original text once from left to right.
  - Each time it hits a detected value, it inserts a unique token (for example `{{TKN-1234}}`) instead of the real value, and builds a new “tokenised” version of the text.

- **Store safe text and the mapping separately**
  - The tokenised text is saved in the `Submission` entity (`TokenizedText` field).
  - Each original value plus its token and tag is saved as a separate `ClassificationRecord`, linked back to the submission.
  - This lets the app show and analyse tokenised text safely, while keeping the real sensitive values in a separate vault-like table.

</details>

---

## Trade offs and decisions made

<details>
<summary>Trade offs and decisions made</summary>

<br />

### 1. EF Core code first with `EnsureCreated` (no migrations checked in)

- **Decision**: Use EF Core’s code first model and `Database.EnsureCreated()` at startup instead of maintaining explicit migrations in this repo.  
- **Reasoning**: For a small challenge with a simple schema, this keeps setup and maintenance very light while still giving a real relational database and clear schema.  
- **Trade off**: This approach is not ideal for production schema evolution, but it is acceptable  for this tech test.

### 2. From email only detection to a generalized classification service

- **Decision**: Start with a dedicated `IEmailDetectionService` and later evolve to a single `IClassificationDetectionService` that detects multiple configured types (email, IBAN, phone, tokens) using regex patterns and enum backed tags.  
- **Reasoning**: Beginning with email only kept the initial pipeline simple and easy to validate; once that was correct, it was generalized to support additional classification types without changing the controller or command.

### 3. Regex based detection for all types

- **Decision**: Use simple regexes for email, IBAN, phone, and tokens instead of external libraries or strict format validators.  
- **Reasoning**: Regex is sufficient here to demonstrate detection behavior and tokenization, and keeps the implementation easy to understand and adjust.  
- **Trade off**: Patterns are deliberately simplified; they may miss some valid real world formats or match slightly more than a production grade validator would.

</details>

---

## What I Would Do If I Had More Time

<details>
<summary>What I Would Do If I Had More Time</summary>

<br />

- **Add more tests**
  - Increase coverage around edge cases in detection (spacing, punctuation, unusual but valid formats).

- **Improve the UI layout and styling**

- **Strengthen detection logic**
  - Replace the basic regexes with more robust patterns or libraries, and tune them per tag type (emails, IBANs, phone numbers, security tokens) to reduce false positives and false negatives.

- **Extend tags**
  - Add more classification types as needed (e.g., national IDs).

</details>

## Screenshot of UI

<img width="1221" height="972" alt="image" src="https://github.com/user-attachments/assets/8a1210e5-d929-47d2-b628-c8ab07048162" />

