# SEO PublicGuard

Technical assessment solution for evaluating SEO article quality prior to publishing.

SEO PublicGuard analyzes Google Docs articles, applies configurable SEO validation rules, calculates a deterministic quality score, and generates a simulated WordPress upload payload.

The solution emphasizes clean architecture, separation of concerns, and testable domain logic.

---

## Architecture

The backend follows a layered architecture:

```
Domain → Application → Infrastructure → Presentation (API)
```

### Domain
- Validation rules
- Scoring logic
- Core entities

### Application
- Use case orchestration (Analyze / Upload)

### Infrastructure
- HTML extraction and parsing
- External concerns

### Presentation
- REST API endpoints
- Configuration
- Swagger
- CORS

Business logic is isolated from framework and transport dependencies to ensure maintainability and extensibility.

The Angular frontend follows a feature-based structure with strongly typed models and a centralized API service layer. The UI is intentionally minimal to focus on assessment objectives.

---

## Core Features

- Google Docs URL ingestion
- HTML parsing and structural analysis
- Configurable SEO validation engine
- Deterministic quality scoring
- Publishing readiness status
- Simulated WordPress payload generation
- Dedicated unit test project

---

## Configuration

Validation thresholds are defined in:

```
PublishGuard.Presentation/appsettings.json
```

Example configuration:

```json
{
  "ValidationRules": {
    "MinImages": 2,
    "MaxImages": 10,
    "MinProductLinks": 1,
    "MaxProductLinks": 10,
    "MinH2Headings": 2,
    "MaxBoldPercentage": 20
  }
}
```

Rules can be adjusted without modifying domain logic.

---

## API

### Analyze Article

```
POST /api/article/analyze
```

Accepts a Google Docs URL and returns metrics, validation issues, score, and publishing readiness status.

---

### Upload Article (Simulated)

```
POST /api/article/upload
```

Returns a structured WordPress payload (placeholder implementation).

---

## Running the Solution

### Backend

Restore dependencies:

```
dotnet restore
```

Run the API:

```
dotnet run --project PublishGuard.Presentation
```

Default API URL:

```
http://localhost:5174
```

Swagger (development):

```
http://localhost:5174/api
```

---

### Frontend

```
cd publishguard-frontend
npm install
npm start
```

Frontend URL:

```
http://localhost:4200
```

---

## Testing

Run all tests:

```
dotnet test
```

Test coverage includes:

- Validation rule boundaries
- Score calculation logic
- Success and failure scenarios
- Edge cases

Domain logic is tested independently of the API layer.

---

## Screenshots

### 1. Home Screen

![Alt text](https://github.com/AmnaManzoor/PublishGuard/blob/main/resources/PG-1.png?raw=true)

### 2. Article Analysis Report 1
![Alt text](https://github.com/AmnaManzoor/PublishGuard/blob/main/resources/PG-2.png?raw=true)

### 3. Article Analysis Report 2
![Alt text](https://github.com/AmnaManzoor/PublishGuard/blob/main/resources/PG-3.png?raw=true)

### 4. Upload Simulation Result
![Alt text](https://github.com/AmnaManzoor/PublishGuard/blob/main/resources/PG-4.png?raw=true)

### 5. Swagger API 
![Alt text](https://github.com/AmnaManzoor/PublishGuard/blob/main/resources/PG-5.png?raw=true)

---

## Walkthrough Video

A short walkthrough demonstrating the full flow (Analyze → Validate → Upload simulation):

[▶️ View Walkthrough Video](docs/walkthrough.mp4)

---

## Design Principles

- Clean architecture
- Config-driven validation engine
- Deterministic scoring
- Testable business logic
- Clear separation of concerns
- Intentional scope control

---

This solution prioritizes correctness, clarity, and extensibility while maintaining focus on the assessment requirements.

