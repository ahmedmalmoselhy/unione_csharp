# UniOne C# Implementation Plan

## Goal

Deliver a robust, enterprise-grade ASP.NET Core backend with equivalent domain coverage for UniOne.

## Recommended Stack

- **Framework**: ASP.NET Core 8.0 (Web API)
- **ORM**: Entity Framework Core (EF Core)
- **Database**: PostgreSQL (Npgsql)
- **Identity**: ASP.NET Core Identity + JWT Bearer Authentication
- **Mapping**: AutoMapper
- **Validation**: FluentValidation
- **Documentation**: Swagger (Swashbuckle)
- **Testing**: xUnit + Moq + FluentAssertions
- **Logging**: Serilog
- **Background Jobs**: Hangfire (for scaled processing)

## Proposed Repository Structure (Clean Architecture approach)

```text
unione_csharp/
  UniOne.Api/             # API Layer (Controllers, Middlewares)
  UniOne.Core/            # Domain Layer (Entities, Interfaces, Constants)
  UniOne.Application/     # Application Layer (DTOs, Services, Business Logic)
  UniOne.Infrastructure/  # Infrastructure Layer (DbContext, Repositories, External Services)
  UniOne.Tests/           # Testing Project
```

## API Organization

Following the standard route structure:
- `/api/auth/*`
- `/api/student/*`
- `/api/professor/*`
- `/api/admin/*`
- `/api/announcements/*`
- `/api/notifications/*`

## Phased Plan

### Phase 1: Core Foundation & Identity
- Initialize Solution and Project structure.
- Configure DbContext and EF Core migrations.
- Implement ASP.NET Core Identity with JWT Bearer configuration.
- Setup Global Error Handling middleware.
- Implement basic Auth endpoints (Login, Me).

### Phase 2: Organization Hierarchy
- Implement Domain Entities: University, Faculty, Department.
- Scoped Authorization Handlers (Permission-based RBAC).
- Implement Organization management controllers.

### Phase 3: Academic Catalog & Enrollments
- Course and Section entities.
- Student profile and Enrollment logic.
- Academic terms management.

### Phase 4: Professor Portal & Grading
- Professor-specific APIs.
- Grading service and Attendance tracking.
- GPA Calculation logic (Application Service).

### Phase 5: Documents & Exports
- PDF Generation service (using QuestPDF or similar).
- iCal (.ics) export utility.
- Excel/CSV processing (using ClosedXML or CsvHelper).

### Phase 6: Communication & Notifications
- Announcements management.
- Notification dispatch system (In-app + Email via SMTP/SendGrid).
- Integration of Background Jobs for async notifications.

### Phase 7: Audit & Integrations
- Audit Logging (using EF Core Interceptors or specialized library).
- Webhook system implementation (Publisher/Subscriber pattern).

### Phase 8: Testing & Quality Assurance
- Unit tests for core domain and application services.
- Integration tests using WebApplicationFactory.
- Swagger/OpenAPI refinement.

## Success Criteria

- 100% parity with the Laravel/Node.js API specification.
- Strong type safety and architectural integrity.
- Production-ready error handling and logging.
- Comprehensive API documentation.
- High test coverage for critical business paths.
