# Decision Log — Mini Projeto 08

This file documents *why* each testing strategy was chosen, based on cost, risk, and confidence.

## Decision 001 — Unit tests use mocks (Service layer)
- **Why**: The goal is to validate business rules with fast feedback and isolated failure diagnosis.
- **Trade-off**: Lower fidelity (no DB mapping/config), but very cheap to maintain and very fast.
- **Scope**: `ProductService` only.

## Decision 002 — Integration tests use real database (API + DB)
- **Why**: Validate full request flow, EF mapping, configuration, and migrations.
- **Trade-off**: Slower, higher setup cost, more infrastructure dependency.
- **Scope**: `ProductsController` endpoints with Testcontainers.

## Decision 003 — Hybrid tests use real database (Service + DB)
- **Why**: Keep DB fidelity but avoid HTTP overhead. Faster than full integration while still testing persistence.
- **Trade-off**: Does not validate routing/serialization/HTTP pipeline.
- **Scope**: `ProductService` with `ProductRepository` against real Postgres.

## Decision 004 — Same behavior across strategies
- **Why**: Comparing different strategies only makes sense if the behavior is the same.
- **Trade-off**: Avoids “extra coverage” that distorts comparison.
- **Scope**: Create product, get by id, list with pagination.
