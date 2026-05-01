# UniOne C# Implementation Plan

## Purpose

Build the C# backend as an ASP.NET Core implementation of the Laravel reference application in `unione_backend`.

The Laravel project is the source of truth for:

- database schema and relationships in `database/migrations`;
- domain entities in `app/Models`;
- business behavior in `app/Services`;
- API contracts in `routes/api.php`;
- dashboard and portal workflows in `routes/web.php`;
- expected behavior in `tests/Feature` and `tests/Unit`.

The current C# project is only a starter API shell. The implementation should proceed as a port, not as a greenfield redesign.

## Current C# Baseline

Existing files:

- `Program.cs` contains the default weather endpoint and OpenAPI bootstrap.
- `Controllers/AuthController.cs` has placeholder `login`, `logout`, and `me` actions.
- `UniOne.Api.csproj` targets `net10.0` and only references `Microsoft.AspNetCore.OpenApi`.
- No EF Core persistence, identity, JWT, domain models, services, authorization policies, imports, exports, background jobs, or tests exist yet.

Immediate cleanup:

- Remove the default weather endpoint.
- Replace placeholder auth responses with real authentication.
- Decide whether to keep `net10.0` or standardize all docs and tooling around the selected target framework.
- Split the codebase into projects before adding domain code.

## Target Architecture

Use a layered ASP.NET Core backend that mirrors Laravel's separation between controllers, models, services, requests, middleware, jobs, notifications, and tests.

```text
unione_csharp/
  src/
    UniOne.Api/
      Controllers/
      Middleware/
      Filters/
      OpenApi/
      Program.cs
    UniOne.Application/
      DTOs/
      Validators/
      Services/
      Authorization/
      Contracts/
      Mapping/
    UniOne.Domain/
      Entities/
      Enums/
      ValueObjects/
      Events/
      Interfaces/
    UniOne.Infrastructure/
      Persistence/
      Repositories/
      Identity/
      Mail/
      Files/
      Jobs/
      Integrations/
      Observability/
  tests/
    UniOne.UnitTests/
    UniOne.IntegrationTests/
```

Recommended packages:

- Persistence: `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Auth: `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`.
- Validation: `FluentValidation.AspNetCore`.
- Mapping: `Mapperly` or `AutoMapper`; choose one and use it consistently.
- API docs: built-in OpenAPI or Swashbuckle, depending on the selected .NET target.
- Jobs: `Hangfire` or `Quartz.NET`.
- Caching: `StackExchange.Redis`, `Microsoft.Extensions.Caching.StackExchangeRedis`.
- Files/imports/exports: `ClosedXML`, `CsvHelper`, `QuestPDF`.
- Testing: `xUnit`, `FluentAssertions`, `Microsoft.AspNetCore.Mvc.Testing`, `Testcontainers.PostgreSql`.
- Logging: `Serilog.AspNetCore`.

## Laravel Domain Map

Implement these entities first because they are directly represented in Laravel models and migrations:

- Identity and access: `User`, `Role`, `RoleUser`, password reset tokens, personal access tokens/sessions.
- Organization: `University`, `UniversityVicePresident`, `Faculty`, `Department`.
- People: `Student`, `Professor`, `Employee`, `StudentDepartmentHistory`.
- Academic catalog: `AcademicTerm`, `Course`, `DepartmentCourse`, `CoursePrerequisite`, `Section`.
- Enrollment and grades: `Enrollment`, `EnrollmentWaitlist`, `Grade`, `StudentTermGpa`.
- Attendance: `AttendanceSession`, `AttendanceRecord`.
- Communication: `Announcement`, `AnnouncementRead`, `SectionAnnouncement`, `Notification`.
- Student feedback: `CourseRating`.
- Section operations: `SectionTeachingAssistant`, `ExamSchedule`, `GroupProject`, `GroupProjectMember`.
- Compliance/integrations: `AuditLog`, `Webhook`, `WebhookDelivery`.
- Infrastructure tables: cache, jobs, failed jobs, job batches, sessions.

Entity implementation rules:

- Preserve Laravel table names where practical to reduce migration and data parity risk.
- Preserve nullable fields, soft-delete behavior, unique constraints, foreign key delete rules, and performance indexes from the Laravel migrations.
- Use enums for fixed Laravel enum values such as gender, semester, enrollment status, attendance status, academic standing, announcement audience, and webhook status.
- Store `Section.Schedule` as JSON initially, matching Laravel's JSON schedule field.
- Model scoped roles explicitly. Laravel uses `role_user` with optional `faculty_id`, `department_id`, `granted_at`, and `revoked_at`; C# should expose this as `RoleAssignment` or equivalent rather than losing scope inside plain Identity roles.

## API Contract From Laravel

All mobile/API routes should be versioned under `/api/v1`, matching `routes/api.php`.

Public routes:

- `GET /api/health`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/forgot-password`
- `POST /api/v1/auth/reset-password`

Authenticated common routes:

- `POST /api/v1/auth/logout`
- `GET /api/v1/auth/me`
- `POST /api/v1/auth/change-password`
- `PATCH /api/v1/auth/profile`
- `GET /api/v1/auth/tokens`
- `DELETE /api/v1/auth/tokens`
- `DELETE /api/v1/auth/tokens/{tokenId}`
- `GET /api/v1/announcements`
- `POST /api/v1/announcements/{id}/read`
- `GET /api/v1/notifications`
- `POST /api/v1/notifications/read-all`
- `POST /api/v1/notifications/{id}/read`
- `DELETE /api/v1/notifications/{id}`
- `GET /api/v1/broadcasting/auth`
- `GET /api/v1/broadcasting/config`
- `GET /api/v1/privacy/export`
- `POST /api/v1/privacy/export/download`
- `POST /api/v1/privacy/anonymize`
- `DELETE /api/v1/privacy/account`

Student routes:

- `GET /api/v1/student/profile`
- `GET /api/v1/student/enrollments`
- `POST /api/v1/student/enrollments`
- `DELETE /api/v1/student/enrollments/{enrollment}`
- `GET /api/v1/student/grades`
- `GET /api/v1/student/transcript`
- `GET /api/v1/student/transcript/pdf`
- `GET /api/v1/student/academic-history`
- `GET /api/v1/student/schedule`
- `GET /api/v1/student/schedule/ics`
- `GET /api/v1/student/attendance`
- `GET /api/v1/student/sections/{section}/announcements`
- `GET /api/v1/student/ratings`
- `POST /api/v1/student/ratings`
- `GET /api/v1/student/waitlist`
- `DELETE /api/v1/student/waitlist/{section}`

Professor routes:

- `GET /api/v1/professor/profile`
- `GET /api/v1/professor/sections`
- `GET /api/v1/professor/schedule`
- `GET /api/v1/professor/sections/{section}/students`
- `GET /api/v1/professor/sections/{section}/grades`
- `POST /api/v1/professor/sections/{section}/grades`
- `GET /api/v1/professor/sections/{section}/attendance`
- `POST /api/v1/professor/sections/{section}/attendance`
- `GET /api/v1/professor/sections/{section}/attendance/{session}`
- `PUT /api/v1/professor/sections/{section}/attendance/{session}`
- `GET /api/v1/professor/sections/{section}/announcements`
- `POST /api/v1/professor/sections/{section}/announcements`
- `DELETE /api/v1/professor/sections/{section}/announcements/{announcement}`

Admin API routes:

- Teaching assistants:
  - `GET /api/v1/admin/sections/{section}/teaching-assistants`
  - `POST /api/v1/admin/sections/{section}/teaching-assistants`
  - `DELETE /api/v1/admin/sections/{section}/teaching-assistants/{sectionTeachingAssistant}`
- Exam schedules:
  - `GET /api/v1/admin/sections/{section}/exam-schedule`
  - `POST /api/v1/admin/sections/{section}/exam-schedule`
  - `PATCH /api/v1/admin/sections/{section}/exam-schedule`
  - `POST /api/v1/admin/sections/{section}/exam-schedule/publish`
- Group projects:
  - `GET /api/v1/admin/sections/{section}/group-projects`
  - `POST /api/v1/admin/sections/{section}/group-projects`
  - `PATCH /api/v1/admin/sections/{section}/group-projects/{groupProject}`
  - `DELETE /api/v1/admin/sections/{section}/group-projects/{groupProject}`
  - `POST /api/v1/admin/sections/{section}/group-projects/{groupProject}/members`
  - `DELETE /api/v1/admin/sections/{section}/group-projects/{groupProject}/members/{groupProjectMember}`
- Webhooks:
  - `GET /api/v1/admin/webhooks`
  - `POST /api/v1/admin/webhooks`
  - `PATCH /api/v1/admin/webhooks/{webhook}`
  - `DELETE /api/v1/admin/webhooks/{webhook}`
  - `GET /api/v1/admin/webhooks/{webhook}/deliveries`
- Queue:
  - `GET /api/v1/admin/queue/health`
  - `GET /api/v1/admin/queue/failed`
  - `DELETE /api/v1/admin/queue/failed/clear`
- Bulk operations:
  - `POST /api/v1/admin/bulk/enroll`
  - `POST /api/v1/admin/bulk/grades`
  - `POST /api/v1/admin/bulk/transfer`
- Analytics:
  - `GET /api/v1/admin/analytics/enrollment-trends`
  - `GET /api/v1/admin/analytics/student-performance/{studentId}`
  - `GET /api/v1/admin/analytics/course-demand`
  - `GET /api/v1/admin/analytics/professor-workload`
  - `GET /api/v1/admin/analytics/attendance`
- Integrations:
  - `GET /api/v1/admin/integrations`
  - `GET /api/v1/admin/integrations/{integration}/test`
  - `POST /api/v1/admin/integrations/{integration}/sync`

Backward compatibility:

- Laravel returns HTTP 410 for old unversioned API paths with a redirect hint. Add equivalent middleware or endpoint fallback after `/api/v1` parity is stable.

## Dashboard And Portal Scope

Laravel includes server-rendered dashboard and portal routes in `routes/web.php`.

For the C# port, implement the underlying behaviors as JSON APIs first. Razor Pages or MVC views are optional and should only be added if the C# project is required to serve HTML.

Admin/dashboard behavior to expose through API controllers:

- faculty CRUD and faculty admin assignment;
- university settings and vice president management;
- department CRUD, department admin assignment, department head assignment;
- professor CRUD, import templates, imports, exports;
- employee CRUD and exports;
- course CRUD and prerequisites;
- section CRUD and schedule management;
- student CRUD, imports, exports, transfer, transcript PDF;
- enrollment CRUD and exports;
- grade CRUD, imports, exports;
- announcement CRUD;
- ratings index;
- audit log index;
- dashboard statistics.

Portal behavior to expose through student/professor/common API controllers:

- portal login/logout equivalent through API auth;
- home/profile/schedule;
- announcements and notifications;
- student enrollments, grades, attendance, ratings;
- professor sections, grades, attendance, section announcements.

## Service Mapping

Port Laravel service behavior into application services:

- `GpaService` -> `IGpaService`: calculate grade points, term GPA, cumulative GPA, academic standing.
- `AdvancedAnalyticsService` -> `IAnalyticsService`: enrollment trends, course demand, student performance, professor workload, attendance analytics.
- `BulkOperationService` -> `IBulkOperationService`: bulk enrollment, grade updates, student transfers.
- `DataPrivacyService` -> `IDataPrivacyService`: export user data, generate downloads, anonymize, account deletion.
- `QueueMonitorService` -> `IQueueMonitorService`: queue health, failed job listing, failed job clearing.
- `CacheService` -> `ICacheService`: common cache access and invalidation helpers.
- Laravel integrations `MoodleIntegration` and `SSOIntegration` -> `IIntegrationAdapter` implementations.

Add C#-specific services needed for parity:

- `IAuthTokenService` for JWT and refresh/session token lifecycle.
- `IAuthorizationScopeService` for admin/faculty/department scoping.
- `IAuditLogService` using EF Core interceptors plus explicit audit calls for sensitive workflows.
- `INotificationService` and `IEmailService` for in-app and email delivery.
- `IImportExportService` for CSV/Excel templates, imports, exports, and validation reports.
- `IPdfService` for transcripts.
- `ICalendarService` for `.ics` schedule output.
- `IWebhookDispatcher` for signed webhook delivery and retry tracking.

## Authorization Model

Mirror Laravel middleware:

- `auth:sanctum` -> JWT bearer authentication plus token/session revocation checks.
- `api.role:student` -> policy requiring active student role/profile.
- `api.role:professor` -> policy requiring active professor role/profile.
- `api.role:admin,faculty_admin,department_admin` -> policy requiring active scoped admin assignment.
- `dashboard`, `admin`, `university.admin`, `scoped.admin` -> policy handlers built on role assignments and optional faculty/department scope.
- `force.password` -> middleware that blocks protected actions when `MustChangePassword` is true except password update endpoints.
- Laravel throttle middleware -> ASP.NET Core rate limiting policies for login, password reset, enrollment, grades, and default API access.

Implementation details:

- Use policy-based authorization with custom handlers, not role strings alone.
- Add a current-user abstraction that exposes user ID, roles, faculty scope, department scope, student ID, professor ID, and employee ID.
- Scope every admin query before pagination and filtering.
- Tests must prove faculty admins cannot access departments outside their faculty and department admins cannot mutate data outside their department.

## Data And Migration Plan

Create EF Core migrations in the same order as Laravel:

1. Infrastructure: cache, jobs, sessions, password reset tokens, personal access tokens.
2. Identity: users, roles, role assignments.
3. Organization: university, faculties, departments, vice presidents.
4. People: professors, employees, students, student department history.
5. Academic catalog: academic terms, courses, department-course, prerequisites, sections.
6. Enrollments and grades: enrollments, grades, waitlist, term GPA.
7. Communication: announcements, announcement reads, notifications, section announcements.
8. Attendance: attendance sessions and records.
9. Feedback and section operations: course ratings, teaching assistants, exam schedules, group projects, group project members.
10. Compliance and integrations: audit logs, webhooks, webhook deliveries.
11. Performance indexes matching `2026_04_12_161737_add_performance_indexes_to_tables.php`.

Seed data:

- roles matching Laravel role names;
- baseline university record;
- development users for system admin, faculty admin, department admin, professor, student, employee;
- representative faculties, departments, courses, sections, enrollments, grades, announcements, and attendance sessions.

## Implementation Phases

### Phase 0 - Project Foundation

Deliverables:

- Convert to the target multi-project solution structure.
- Add package references and shared build settings.
- Configure `Program.cs` with controllers, OpenAPI, JSON options, validation, authentication, authorization, rate limiting, exception handling, logging, health checks, and CORS.
- Add Docker Compose or local configuration for PostgreSQL and Redis.
- Remove scaffold weather endpoint.

Acceptance checks:

- `dotnet build` succeeds.
- `GET /api/health` returns healthy.
- OpenAPI document is generated.

### Phase 1 - Persistence And Identity

Deliverables:

- `UniOneDbContext` with entity configurations for identity, roles, scoped assignments, organization tables, and soft deletes.
- Identity/password hashing setup.
- JWT login, logout, me, change password, update profile.
- Password reset token workflow.
- Token listing and revocation endpoints.
- Current-user service and auth policies.

Laravel references:

- `app/Http/Controllers/Auth/AuthController.php`
- `app/Http/Controllers/Auth/PasswordResetController.php`
- `app/Http/Controllers/Api/TokenController.php`
- `app/Http/Middleware/EnsureApiRole.php`
- `app/Http/Middleware/ForcePasswordChange.php`

Acceptance checks:

- Auth integration tests cover login failure, login success, me, logout, token revocation, password reset, change password, and forced password changes.
- Unauthorized, forbidden, validation, and not-found responses use consistent problem details.

### Phase 2 - Organization And Admin Scope

Deliverables:

- University, vice president, faculty, and department entities/controllers/services.
- Faculty admin, department admin, and department head assignment workflows.
- Scoped admin authorization.
- Audit logging for admin assignment and organization mutations.

Laravel references:

- Dashboard controllers for `University`, `UniversityVicePresident`, `Faculty`, `Department`, `AdminAssignment`, `DepartmentHead`.
- Middleware `AdminMiddleware`, `UniversityAdminMiddleware`, `ScopedAdminMiddleware`.

Acceptance checks:

- Admin scope tests match Laravel dashboard middleware expectations.
- CRUD endpoints preserve foreign key restrictions and soft-delete behavior.

### Phase 3 - People Management

Deliverables:

- Student, professor, and employee CRUD.
- Student transfer workflow and department history.
- Professor/student/employee imports and exports.
- Import templates and validation reports.

Laravel references:

- `Dashboard/StudentController`, `ProfessorController`, `EmployeeController`.
- `Imports/*`, `Exports/*`.
- `StudentDepartmentHistory` model and migrations.

Acceptance checks:

- Imports reject invalid rows with row-level errors.
- Student transfer updates current department and writes history.
- Exports include the fields expected by Laravel tests.

### Phase 4 - Academic Catalog And Enrollment

Deliverables:

- Academic terms, courses, prerequisites, departments-to-courses, sections, and section schedules.
- Enrollment CRUD with capacity checks, prerequisite checks, active term checks, duplicate prevention, waitlist behavior, and drop behavior.
- Student enrollment APIs.

Laravel references:

- `AcademicTermController`, `CourseController`, `SectionController`, `EnrollmentController`.
- `Api/StudentEnrollmentController`, `Api/StudentWaitlistController`.
- Tests for prerequisites, student enrollment, and waitlist.

Acceptance checks:

- Enrollment integration tests cover full, duplicate, missing prerequisite, waitlist promotion, and delete/drop cases.
- Schedule JSON serializes consistently with Laravel's shape.

### Phase 5 - Grades, GPA, Transcripts, And Schedules

Deliverables:

- Grade CRUD and professor grade submission.
- Grade import/export.
- GPA calculation and persisted `StudentTermGpa`.
- Transcript JSON and transcript PDF.
- Student/professor schedule APIs and `.ics` export.

Laravel references:

- `GpaService`
- `GradeController`
- `Api/ProfessorGradeController`
- `Api/StudentController`
- Transcript and schedule tests.

Acceptance checks:

- GPA unit tests match Laravel grade point behavior.
- Transcript and `.ics` endpoints pass parity tests for shape and content.

### Phase 6 - Attendance, Announcements, Notifications, Ratings

Deliverables:

- Attendance sessions and records for professor and student views.
- Global announcements with read tracking.
- Section announcements.
- In-app notifications with read/delete actions.
- Course ratings.
- Email dispatch hooks for announcements, exam schedules, and grade publication.

Laravel references:

- `Api/AttendanceController`
- `Api/AnnouncementController`
- `Api/NotificationController`
- `Api/SectionAnnouncementController`
- `Api/CourseRatingController`
- Laravel notification classes.

Acceptance checks:

- Professor can create/update attendance only for owned sections.
- Students only see their own attendance and enrolled section announcements.
- Notification read-all/read-one/delete behavior matches Laravel.

### Phase 7 - Section Operations

Deliverables:

- Teaching assistant assignment.
- Exam schedule create/update/publish with conflict detection.
- Group projects and members.
- Notification/email dispatch on published exam schedules where Laravel does so.

Laravel references:

- `Api/SectionTeachingAssistantController`
- `Api/SectionExamScheduleController`
- `Api/SectionGroupProjectController`
- Feature tests under `tests/Feature/Api`.

Acceptance checks:

- Admin scope is enforced for all section operations.
- Exam publish workflow prevents conflicting or invalid schedules.
- Group project member endpoints validate enrollment and duplicates.

### Phase 8 - Privacy, Webhooks, Queue, Analytics, Integrations

Deliverables:

- Privacy export, download, anonymize, account deletion.
- Webhook CRUD, signed dispatch, delivery records, retry state.
- Queue health and failed-job endpoints.
- Bulk enroll, bulk grade update, bulk transfer.
- Analytics endpoints.
- Integration marketplace with Moodle and SSO adapters.

Laravel references:

- `DataPrivacyService`
- `WebhookController`, `DispatchWebhooks`
- `QueueMonitorService`
- `BulkOperationService`
- `AdvancedAnalyticsService`
- `IntegrationMarketplaceController`

Acceptance checks:

- Webhook delivery tests verify signatures and failure tracking.
- Privacy tests verify exports exclude other users' data.
- Analytics endpoints return stable DTOs even when datasets are empty.

### Phase 9 - Parity Hardening

Deliverables:

- Backward-compatible 410 response for unversioned API routes.
- OpenAPI documentation aligned with `/api/v1`.
- Response DTO audit against Laravel API tests.
- Query pagination, filtering, field selection, and mobile optimization where Laravel middleware supports them.
- Performance indexes applied and verified.
- CI workflow for build, format, unit tests, integration tests, and migration validation.

Laravel references:

- `routes/api.php` fallback.
- `FieldSelector`, `MobileApiOptimizer`, `AddRateLimitHeaders`.
- `2026_04_12_161737_add_performance_indexes_to_tables.php`.

Acceptance checks:

- Full C# test suite passes.
- Endpoint inventory matches Laravel `routes/api.php`.
- Critical Laravel feature tests have equivalent C# integration tests.

## Test Plan

Port the Laravel test intent into C#:

- Unit tests:
  - GPA calculations.
  - enrollment rules.
  - grade calculations.
  - scoped authorization decisions.
  - webhook signing.
  - import row validation.
- Integration tests:
  - auth endpoints;
  - student profile/enrollment/grades/transcript/schedule/attendance/waitlist;
  - professor sections/grades/attendance/announcements;
  - admin organization/people/catalog/enrollment/grades/import/export;
  - notifications and announcements;
  - section operations;
  - privacy, webhooks, queue, analytics, integrations.
- Migration tests:
  - database can migrate from empty;
  - foreign keys and indexes exist;
  - soft-delete query filters behave correctly.

Use Testcontainers PostgreSQL for integration tests so behavior matches production database semantics.

## Delivery Order

The safest delivery order is:

1. Foundation, persistence, identity.
2. Organization and scoped authorization.
3. People management.
4. Academic catalog.
5. Enrollment and waitlist.
6. Grades, GPA, transcripts, schedules.
7. Attendance, communication, notifications, ratings.
8. Section operations.
9. Privacy, webhooks, queues, analytics, integrations.
10. Parity hardening, docs, CI/CD.

Do not start imports, analytics, or integrations before the core schema and authorization model are stable; those features depend heavily on correct scoped access and entity relationships.

## Definition Of Done

The C# project reaches parity when:

- every Laravel `/api/v1` route has an equivalent C# endpoint or a documented reason it is intentionally excluded;
- all Laravel domain tables have equivalent EF Core entities/migrations;
- scoped authorization behavior matches Laravel middleware;
- student, professor, and admin workflows are covered by integration tests;
- imports, exports, transcripts, schedules, notifications, webhooks, privacy, queue, and analytics are implemented;
- OpenAPI accurately documents the implemented API;
- build, tests, and migrations run in CI;
- seeded local development data supports the main roles and workflows.
