using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Mapping;

public class CatalogMapper
{
    public AcademicTermDto ToDto(AcademicTerm term)
    {
        return new AcademicTermDto
        {
            Id = term.Id,
            Name = term.Name,
            NameAr = term.NameAr,
            AcademicYear = term.AcademicYear,
            Semester = term.Semester,
            StartsAt = term.StartsAt,
            EndsAt = term.EndsAt,
            RegistrationStartsAt = term.RegistrationStartsAt,
            RegistrationEndsAt = term.RegistrationEndsAt,
            IsActive = term.IsActive
        };
    }

    public AcademicTerm ToEntity(CreateAcademicTermDto dto)
    {
        return new AcademicTerm
        {
            Name = dto.Name,
            NameAr = dto.NameAr,
            AcademicYear = dto.AcademicYear,
            Semester = dto.Semester,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
            RegistrationStartsAt = dto.RegistrationStartsAt,
            RegistrationEndsAt = dto.RegistrationEndsAt
        };
    }

    public void UpdateTerm(UpdateAcademicTermDto dto, AcademicTerm term)
    {
        term.Name = dto.Name;
        term.NameAr = dto.NameAr;
        term.AcademicYear = dto.AcademicYear;
        term.Semester = dto.Semester;
        term.StartsAt = dto.StartsAt;
        term.EndsAt = dto.EndsAt;
        term.RegistrationStartsAt = dto.RegistrationStartsAt;
        term.RegistrationEndsAt = dto.RegistrationEndsAt;
        term.IsActive = dto.IsActive;
    }

    public CourseDto ToDto(Course course)
    {
        return new CourseDto
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name,
            NameAr = course.NameAr,
            Description = course.Description,
            CreditHours = course.CreditHours,
            Level = course.Level,
            IsElective = course.IsElective,
            IsActive = course.IsActive
        };
    }

    public Course ToEntity(CreateCourseDto dto)
    {
        return new Course
        {
            Code = dto.Code,
            Name = dto.Name,
            NameAr = dto.NameAr,
            Description = dto.Description,
            CreditHours = dto.CreditHours,
            LectureHours = dto.LectureHours,
            LabHours = dto.LabHours,
            Level = dto.Level,
            IsElective = dto.IsElective
        };
    }

    public void UpdateCourse(UpdateCourseDto dto, Course course)
    {
        course.Code = dto.Code;
        course.Name = dto.Name;
        course.NameAr = dto.NameAr;
        course.Description = dto.Description;
        course.CreditHours = dto.CreditHours;
        course.LectureHours = dto.LectureHours;
        course.LabHours = dto.LabHours;
        course.Level = dto.Level;
        course.IsElective = dto.IsElective;
        course.IsActive = dto.IsActive;
    }

    public SectionDto ToDto(Section section)
    {
        return new SectionDto
        {
            Id = section.Id,
            CourseId = section.CourseId,
            CourseName = section.Course.Name,
            CourseCode = section.Course.Code,
            ProfessorId = section.ProfessorId,
            ProfessorFullName = MapProfessorName(section.Professor),
            AcademicTermId = section.AcademicTermId,
            AcademicTermName = section.AcademicTerm.Name,
            Capacity = section.Capacity,
            Room = section.Room,
            Schedule = section.Schedule,
            IsActive = section.IsActive
        };
    }

    public Section ToEntity(CreateSectionDto dto)
    {
        return new Section
        {
            CourseId = dto.CourseId,
            ProfessorId = dto.ProfessorId,
            AcademicTermId = dto.AcademicTermId,
            Capacity = dto.Capacity,
            Room = dto.Room,
            Schedule = dto.Schedule
        };
    }

    public void UpdateSection(UpdateSectionDto dto, Section section)
    {
        section.CourseId = dto.CourseId;
        section.ProfessorId = dto.ProfessorId;
        section.AcademicTermId = dto.AcademicTermId;
        section.Capacity = dto.Capacity;
        section.Room = dto.Room;
        section.Schedule = dto.Schedule;
        section.IsActive = dto.IsActive;
    }

    private static string MapProfessorName(Professor professor) => $"{professor.User.FirstName} {professor.User.LastName}";
}
