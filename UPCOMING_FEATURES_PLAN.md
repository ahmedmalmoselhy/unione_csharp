# Upcoming Features Plan - UniOne C# Backend

## Reconciled Status

`CURRENT_STATUS.md` says the project is in Phase 5 with Phases 0-4 complete. The codebase is closer to:

- Phase 0: implemented and building.
- Phase 1: implemented enough for auth endpoints and token revocation tests.
- Phase 2: partially implemented. Organization entities, services, controllers, scoped role policy, and audit logging exist.
- Phase 3: partially implemented in code, but not complete by the plan's acceptance criteria.
- Phase 4: partially implemented in code, but not complete by the plan's acceptance criteria.
- Phase 5: not started in source. No Grade, StudentTermGpa, GPA service, transcript API, PDF transcript, schedule API, or ICS service exists yet.

The main discrepancy is that people, catalog, and enrollment source files exist, but EF Core migrations only cover identity, personal access tokens, and organization tables. The database schema does not currently include people, catalog, enrollment, waitlist, grade, or GPA tables.

## Verification Snapshot

- `dotnet build ..\UniOne.sln` passes.
- `dotnet test ..\UniOne.sln --no-build` passes: 15 tests total.
- Build emits 192 Mapperly warnings, mostly unmapped DTO/entity members in people and catalog mappings.
- Test coverage currently focuses on auth validators, current-user behavior, and auth endpoints. There are no organization, people, catalog, enrollment, grade, GPA, transcript, schedule, or scoped admin integration tests.

## Immediate Stabilization Work

Before adding Phase 5 features, stabilize the implemented surface so new features are built on a valid schema and tested contracts.

1. Add missing migrations for implemented entities.
   - Create migrations for professors, employees, students, student department history.
   - Create migrations for academic terms, courses, course prerequisites, department-course, sections.
   - Create migrations for enrollments and enrollment waitlist.
   - Verify foreign keys, unique indexes, delete behavior, nullable fields, and enum storage against the Laravel migrations.

2. Close Phase 3 gaps.
   - Add integration tests for student, professor, and employee CRUD.
   - Add integration tests for student transfer and department history.
   - Validate import/export behavior with row-level errors and expected columns.
   - Confirm scoped admin filtering is applied before search/export.

3. Close Phase 4 gaps.
   - Add admin section endpoints for create/update/delete if not fully exposed.
   - Add endpoints for prerequisites and department-course assignment.
   - Add enrollment integration tests for duplicate enrollment, capacity, waitlist promotion, prerequisite failure, registration window, and drop.
   - Make waitlist behavior API-friendly; do not signal successful waitlist insertion only through an exception path.

4. Reduce mapping risk.
   - Review all Mapperly warnings.
   - Explicitly ignore intentional unmapped navigation/audit fields.
   - Add DTO fields where the API contract requires them, especially term deadlines, course hours, and schedule fields.

## Phase 5 Delivery Plan

### 5.1 Grade Domain And Persistence

Deliver:

- `Grade` entity matching the Laravel schema.
- `StudentTermGpa` entity matching the Laravel schema.
- EF Core configuration and migration for grades and term GPA.
- `DbSet<Grade>` and `DbSet<StudentTermGpa>` in `IApplicationDbContext` and `UniOneDbContext`.
- DTOs and validators for grade create/update/import/professor submission.

Acceptance:

- Migration applies from an empty database.
- Unique constraints prevent duplicate student/section grade records.
- Grade values and status fields match Laravel enum/value behavior.

### 5.2 GPA Service

Deliver:

- `IGpaService` and implementation.
- Grade point conversion matching Laravel `GpaService`.
- Term GPA calculation.
- Cumulative GPA calculation.
- Academic standing calculation.
- Persistence/update of `StudentTermGpa`.

Acceptance:

- Unit tests cover every grade symbol/score boundary from Laravel.
- Unit tests cover empty terms, repeated courses, dropped courses, failed courses, and cumulative GPA.

### 5.3 Admin Grade APIs

Deliver:

- Admin grade CRUD endpoints.
- Grade import/export endpoints.
- Scoped admin enforcement for faculty/department admins.
- Audit logging for grade mutations.

Acceptance:

- Admin integration tests cover create, update, delete, import validation failures, export shape, and scoped access denial.
- Problem details are consistent for validation, not found, unauthorized, and forbidden cases.

### 5.4 Professor Grade Submission APIs

Deliver:

- `GET /api/v1/professor/sections/{section}/grades`
- `POST /api/v1/professor/sections/{section}/grades`
- Ownership checks so professors only grade assigned sections.
- Optional grade publication hook if required by the Laravel behavior.

Acceptance:

- Professor integration tests cover owned section success, unowned section forbidden, invalid student, non-enrolled student, duplicate grade, and grade update.

### 5.5 Student Grade And Transcript APIs

Deliver:

- `GET /api/v1/student/grades`
- `GET /api/v1/student/transcript`
- `GET /api/v1/student/academic-history`
- Transcript DTO with terms, courses, grades, term GPA, cumulative GPA, and academic standing.

Acceptance:

- Student integration tests prove users only see their own grades/transcript.
- Transcript shape is stable and matches Laravel API expectations.

### 5.6 Transcript PDF

Deliver:

- `IPdfService` implementation using QuestPDF.
- `GET /api/v1/student/transcript/pdf`
- Reusable transcript rendering model shared with the JSON transcript API.

Acceptance:

- PDF endpoint returns `application/pdf`.
- Integration test verifies non-empty PDF output and student access control.

### 5.7 Schedule And ICS APIs

Deliver:

- `ICalendarService` for ICS output.
- `GET /api/v1/student/schedule`
- `GET /api/v1/student/schedule/ics`
- `GET /api/v1/professor/schedule`
- Schedule parser/serializer for the existing JSON schedule field.

Acceptance:

- Schedule JSON keeps the Laravel-compatible shape.
- ICS endpoint returns `text/calendar`.
- Tests cover empty schedules, multi-day schedules, room/course metadata, and access control.

## Phase 6 Preview

Start Phase 6 only after Phase 5 is tested end to end.

1. Attendance sessions and records.
2. Global announcements and read tracking.
3. Section announcements.
4. Notifications with read/delete/read-all.
5. Course ratings.
6. Email dispatch hooks for announcements, exam schedules, and grade publication.

## Recommended Next Commit Sequence

1. `fix: add missing people catalog enrollment migrations`
2. `test: cover people catalog enrollment workflows`
3. `chore: resolve mapper warnings`
4. `feat: add grade and gpa persistence`
5. `feat: implement gpa calculations`
6. `feat: add admin and professor grade APIs`
7. `feat: add student grades and transcripts`
8. `feat: add transcript pdf and schedule exports`

