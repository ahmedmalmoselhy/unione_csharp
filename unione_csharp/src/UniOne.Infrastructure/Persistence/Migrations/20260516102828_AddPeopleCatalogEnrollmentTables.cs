using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UniOne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPeopleCatalogEnrollmentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "academic_terms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NameAr = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AcademicYear = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Semester = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    StartsAt = table.Column<DateOnly>(type: "date", nullable: false),
                    EndsAt = table.Column<DateOnly>(type: "date", nullable: false),
                    RegistrationStartsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RegistrationEndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WithdrawalDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExamStartsAt = table.Column<DateOnly>(type: "date", nullable: true),
                    ExamEndsAt = table.Column<DateOnly>(type: "date", nullable: true),
                    GradeSubmissionDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_academic_terms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Action = table.Column<string>(type: "text", nullable: false),
                    AuditableType = table.Column<string>(type: "text", nullable: false),
                    AuditableId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_audit_logs_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NameAr = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreditHours = table.Column<byte>(type: "smallint", nullable: false),
                    LectureHours = table.Column<byte>(type: "smallint", nullable: false),
                    LabHours = table.Column<byte>(type: "smallint", nullable: false),
                    Level = table.Column<byte>(type: "smallint", nullable: false),
                    IsElective = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    StaffNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmploymentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Salary = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    HiredAt = table.Column<DateOnly>(type: "date", nullable: false),
                    TerminatedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employees_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employees_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "professors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    StaffNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    Specialization = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AcademicRank = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    OfficeLocation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HiredAt = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_professors_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_professors_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    StudentNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FacultyId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    AcademicYear = table.Column<byte>(type: "smallint", nullable: false),
                    Semester = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EnrollmentStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AcademicStanding = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Gpa = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    EnrolledAt = table.Column<DateOnly>(type: "date", nullable: false),
                    GraduatedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_students_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_students_faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_students_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_prerequisites",
                columns: table => new
                {
                    CourseId = table.Column<long>(type: "bigint", nullable: false),
                    PrerequisiteId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_prerequisites", x => new { x.CourseId, x.PrerequisiteId });
                    table.ForeignKey(
                        name: "FK_course_prerequisites_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_course_prerequisites_courses_PrerequisiteId",
                        column: x => x.PrerequisiteId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "department_course",
                columns: table => new
                {
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    CourseId = table.Column<long>(type: "bigint", nullable: false),
                    IsOwner = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department_course", x => new { x.DepartmentId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_department_course_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_department_course_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<long>(type: "bigint", nullable: false),
                    ProfessorId = table.Column<long>(type: "bigint", nullable: false),
                    AcademicTermId = table.Column<long>(type: "bigint", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Room = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Schedule = table.Column<string>(type: "jsonb", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sections_academic_terms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "academic_terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sections_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sections_professors_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "professors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "student_department_history",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    FromDepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    ToDepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    SwitchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SwitchedBy = table.Column<long>(type: "bigint", nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_department_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_student_department_history_departments_FromDepartmentId",
                        column: x => x.FromDepartmentId,
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_student_department_history_departments_ToDepartmentId",
                        column: x => x.ToDepartmentId,
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_department_history_students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_student_department_history_users_SwitchedBy",
                        column: x => x.SwitchedBy,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "enrollment_waitlist",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                    AcademicTermId = table.Column<long>(type: "bigint", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enrollment_waitlist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_enrollment_waitlist_academic_terms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "academic_terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_enrollment_waitlist_sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_enrollment_waitlist_students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "enrollments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                    AcademicTermId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DroppedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_enrollments_academic_terms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "academic_terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_enrollments_sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_enrollments_students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_role_user_DepartmentId",
                table: "role_user",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_role_user_FacultyId",
                table: "role_user",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_academic_terms_AcademicYear_Semester",
                table: "academic_terms",
                columns: new[] { "AcademicYear", "Semester" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_UserId",
                table: "audit_logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_course_prerequisites_PrerequisiteId",
                table: "course_prerequisites",
                column: "PrerequisiteId");

            migrationBuilder.CreateIndex(
                name: "IX_courses_Code",
                table: "courses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_department_course_CourseId",
                table: "department_course",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_employees_DepartmentId",
                table: "employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_employees_StaffNumber",
                table: "employees",
                column: "StaffNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_UserId",
                table: "employees",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_waitlist_AcademicTermId",
                table: "enrollment_waitlist",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_waitlist_SectionId",
                table: "enrollment_waitlist",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_waitlist_SectionId_Position",
                table: "enrollment_waitlist",
                columns: new[] { "SectionId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_waitlist_StudentId",
                table: "enrollment_waitlist",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_waitlist_StudentId_SectionId",
                table: "enrollment_waitlist",
                columns: new[] { "StudentId", "SectionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_AcademicTermId",
                table: "enrollments",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_SectionId",
                table: "enrollments",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_StudentId",
                table: "enrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_StudentId_SectionId",
                table: "enrollments",
                columns: new[] { "StudentId", "SectionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_professors_DepartmentId",
                table: "professors",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_professors_StaffNumber",
                table: "professors",
                column: "StaffNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_professors_UserId",
                table: "professors",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sections_AcademicTermId",
                table: "sections",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_sections_CourseId",
                table: "sections",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_sections_CourseId_AcademicTermId_ProfessorId",
                table: "sections",
                columns: new[] { "CourseId", "AcademicTermId", "ProfessorId" });

            migrationBuilder.CreateIndex(
                name: "IX_sections_ProfessorId",
                table: "sections",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_student_department_history_FromDepartmentId",
                table: "student_department_history",
                column: "FromDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_student_department_history_StudentId",
                table: "student_department_history",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_student_department_history_SwitchedBy",
                table: "student_department_history",
                column: "SwitchedBy");

            migrationBuilder.CreateIndex(
                name: "IX_student_department_history_ToDepartmentId",
                table: "student_department_history",
                column: "ToDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_students_DepartmentId",
                table: "students",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_students_FacultyId",
                table: "students",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_students_StudentNumber",
                table: "students",
                column: "StudentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_students_UserId",
                table: "students",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_role_user_departments_DepartmentId",
                table: "role_user",
                column: "DepartmentId",
                principalTable: "departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_role_user_faculties_FacultyId",
                table: "role_user",
                column: "FacultyId",
                principalTable: "faculties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_role_user_departments_DepartmentId",
                table: "role_user");

            migrationBuilder.DropForeignKey(
                name: "FK_role_user_faculties_FacultyId",
                table: "role_user");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "course_prerequisites");

            migrationBuilder.DropTable(
                name: "department_course");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "enrollment_waitlist");

            migrationBuilder.DropTable(
                name: "enrollments");

            migrationBuilder.DropTable(
                name: "student_department_history");

            migrationBuilder.DropTable(
                name: "sections");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "academic_terms");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "professors");

            migrationBuilder.DropIndex(
                name: "IX_role_user_DepartmentId",
                table: "role_user");

            migrationBuilder.DropIndex(
                name: "IX_role_user_FacultyId",
                table: "role_user");
        }
    }
}
