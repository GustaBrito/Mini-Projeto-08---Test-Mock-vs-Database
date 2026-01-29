# Mini Project 08 - Testing Strategy: Mocks vs Real Database

## 1. Project Overview
This project demonstrates how to choose the right testing strategy for the same behavior in an ASP.NET Core Web API. The focus is not new features, but the trade-offs between unit tests with mocks, integration tests with a real database, and a hybrid approach.

Context: portfolio and educational demonstration of testing strategy decisions in backend systems.

Relevant for: Backend Engineers, Platform Engineers, and anyone evaluating test strategy, validation, and data access boundaries in ASP.NET Core.

## 2. Architecture & Design
Style: Layered architecture.

Separation of responsibilities:
- API layer: controllers and DTOs.
- Service layer: business rules and validation.
- Repository layer: data access abstraction.
- Persistence layer: EF Core DbContext and migrations.

Main flow:
HTTP request -> Controller -> Service -> Repository -> EF Core -> PostgreSQL.

## 3. Tech Stack
- Language/runtime: C# with .NET 10
- Web framework: ASP.NET Core
- ORM/data access: Entity Framework Core, Npgsql provider
- API documentation: Swashbuckle (Swagger)
- Database: PostgreSQL
- Testing: xUnit, FluentAssertions, Moq, Microsoft.AspNetCore.Mvc.Testing
- Test infrastructure: Testcontainers (Docker)
- Observability/logging: default ASP.NET Core logging (no custom observability)

## 4. Key Features
- Create product
- Get product by id
- List products with pagination metadata
- Input validation via data annotations and service-level checks

## 5. API / Application Behavior
- POST /api/products
  - Validates input, normalizes name/description, stores product
  - Returns 201 Created with ProductDto
  - Returns 400 Bad Request for invalid input
- GET /api/products/{id}
  - Returns 200 with ProductDto when found
  - Returns 404 when missing
- GET /api/products?page=&pageSize=
  - Returns 200 with PagedResult<ProductDto>
  - Returns 400 for invalid pagination

Error handling:
- Model validation errors return 400 (ValidationProblem).
- Invalid service-level input (name/price/pagination) returns 400 with Problem details.

## 6. Testing Strategy
Test suites are split by intent:
- Unit tests: service layer using Moq to isolate business rules.
- Integration tests: full API + real PostgreSQL via Testcontainers.
- Hybrid tests: real PostgreSQL + repository + service (no HTTP).

Organization:
- tests/App.UnitTests
- tests/App.IntegrationTests
- tests/App.HybridTests

Note: Integration and Hybrid tests require Docker. Hybrid tests skip when Docker is not available; Integration tests require Docker running.

## 7. How to Run the Project
Prerequisites:
- .NET SDK 10.0.102 (see global.json)
- PostgreSQL running (for local API execution)
- Docker (for integration and hybrid tests)

Run the API:
```
 dotnet run --project .\src\App.Api\App.Api.csproj
```

Run tests:
```
 dotnet test .\tests\App.UnitTests\App.UnitTests.csproj
 dotnet test .\tests\App.IntegrationTests\App.IntegrationTests.csproj
 dotnet test .\tests\App.HybridTests\App.HybridTests.csproj
```

Helper script:
```
 .\scripts\run-tests.ps1
 .\scripts\run-tests.ps1 -suite unit
 .\scripts\run-tests.ps1 -suite integration
 .\scripts\run-tests.ps1 -suite hybrid
```

## 8. Project Status
Completed. This is a portfolio-oriented project focused on testing strategy trade-offs.

## 9. Why This Project Matters
This project demonstrates:
- Practical decision-making between test isolation and fidelity
- Layered architecture boundaries in ASP.NET Core
- Validation and error handling patterns
- Real database integration using Testcontainers
- Clear separation of unit, integration, and hybrid testing scopes
