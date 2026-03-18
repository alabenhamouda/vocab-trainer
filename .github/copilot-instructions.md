# Copilot Instructions for VocabTrainer

## Project Overview
This is a **.NET Aspire** distributed application following **Clean Architecture** principles.

The goal of VocabTrainer is to help learn German vocabulary (initially from
DW *Nicos Weg*) using **flashcards and spaced repetition**, with an emphasis on
*long-term retention* rather than short-term memorization.

This is **not** a generic dictionary, translation service, or Anki clone.

---

## Architectural Overview

- **Orchestration**: `VocabTrainer.AppHost` manages the solution composition.
- **Frontend**: `VocabTrainer.Web` (Blazor Interactive Server).
- **Backend**: `VocabTrainer.ApiService` (ASP.NET Core Web API).
- **Core Logic**:
  - `VocabTrainer.Domain` (Domain models & rules)
  - `VocabTrainer.Application` (Use cases, orchestration)
- **Infrastructure**: `VocabTrainer.Infrastructure` (EF Core, PostgreSQL, scraping, external services).
- **Defaults**: `VocabTrainer.ServiceDefaults` (OTel, HealthChecks, resilience).

---

## Domain Intent (Important)

### Core Problem Being Solved
The domain models *learning*, not just vocabulary data.

The system tracks:
- what vocabulary was learned
- where it came from (lessons)
- how well it is remembered over time
- when and how it should be reviewed again

Design decisions should favor **learning behavior** over pure data modeling.

---

### Vocabulary Modeling Philosophy
- A **vocabulary entry** represents *one learned unit*.
- Not all vocabulary behaves the same during training.
- The domain may distinguish between types (e.g. nouns, expressions, verbs)
  **when this distinction enables different learning or training behavior**.

Do not introduce distinctions purely for grammatical completeness.
Prefer behavior-driven modeling.

The exact shape of the vocabulary hierarchy is expected to evolve.

---

### Spaced Repetition
- Spaced repetition is a **core domain concern**
- It applies to all vocabulary, regardless of type
- Algorithms (e.g. SM-2) are implementation details and may change

---

## Clean Architecture Rules

### Domain (`VocabTrainer.Domain`)
- Pure C# code only
- No EF Core, ASP.NET, serialization, or framework attributes
- Focus on invariants, behavior, and domain language
- Domain models should be persistence-ignorant

### Application (`VocabTrainer.Application`)
- Coordinates use cases
- Depends only on Domain abstractions
- Defines interfaces for persistence and external services

### Infrastructure (`VocabTrainer.Infrastructure`)
- Implements Application interfaces
- Contains EF Core, PostgreSQL mappings, scraping logic
- Translates between domain models and persistence models if needed

### Presentation
- `ApiService`: Minimal APIs, orchestration only
- `Web`: Blazor UI logic
- No domain logic beyond simple coordination

---

## Persistence Assumptions (Guidance, not constraints)

- PostgreSQL is used as the primary database
- EF Core is used in Infrastructure
- Inheritance in the domain model is allowed when it reflects domain meaning
- Preferred EF Core mapping strategy (when inheritance exists): **TPH**

Do not push persistence concerns into the domain layer.

---

## .NET Aspire & Service Defaults

- **AppHost**: Use `VocabTrainer.AppHost/AppHost.cs` to define resources and relationships
  - Example: `builder.AddProject<Projects.VocabTrainer_ApiService>("apiservice")`
- **Service Discovery**: Use the resource name as hostname (e.g. `https+http://apiservice`)
- **Service Defaults**:
  - All services MUST call `builder.AddServiceDefaults()`
  - All services MUST call `app.MapDefaultEndpoints()`
- **Resilience**: HttpClient resilience is configured centrally in `ServiceDefaults`

---

## Frontend (Blazor)

- **Render Mode**: Interactive Server
- **Components**: `VocabTrainer.Web/Components`
- **API Communication**: Typed HttpClients registered in `Program.cs`
- Frontend should not contain domain logic

---

## Developer Workflow

### Build & Run
- Always start `VocabTrainer.AppHost` to run the full system
- Use the Aspire Dashboard for logs, traces, and metrics

### Testing
- Integration tests live in `VocabTrainer.Tests`
- Use `DistributedApplicationTestingBuilder`
- Always wait for services to become healthy before assertions

---

## Domain Evolution Guidelines (Very Important)

- The domain model is expected to evolve
- Avoid over-modeling grammar concepts too early
- Prefer:
  - adding behavior
  - adding value objects
  - enriching existing models

over:
- deep inheritance trees
- premature specialization

Favor **clarity and correctness** over exhaustiveness.

---

## Language & Naming

- Use domain language consistently:
  - Vocabulary
  - Lesson
  - Review
  - Training
  - Spaced Repetition
- Avoid introducing new terms when an existing domain term applies

Naming consistency matters more than clever abstractions.

## Code Style & Conventions

- **Primary Constructors**: Use C# 12 primary constructors when the constructor body would be empty or only contains simple property assignments. If validation or complex logic is required, use a standard constructor.
- **Code Formatting**: After making edits, always run `csharpier format .` to keep the code formatted consistently across the solution.
