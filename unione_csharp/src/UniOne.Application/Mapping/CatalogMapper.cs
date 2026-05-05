using Riok.Mapperly.Abstractions;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Mapping;

[Mapper]
public partial class CatalogMapper
{
    public partial AcademicTermDto ToDto(AcademicTerm term);
    public partial AcademicTerm ToEntity(CreateAcademicTermDto dto);
    public partial void UpdateTerm(UpdateAcademicTermDto dto, AcademicTerm term);

    public partial CourseDto ToDto(Course course);
    public partial Course ToEntity(CreateCourseDto dto);
    public partial void UpdateCourse(UpdateCourseDto dto, Course course);

    [MapProperty(nameof(Section.Course.Name), nameof(SectionDto.CourseName))]
    [MapProperty(nameof(Section.Course.Code), nameof(SectionDto.CourseCode))]
    [MapProperty(nameof(Section.Professor.User.FirstName), nameof(SectionDto.ProfessorFullName))]
    [MapProperty(nameof(Section.AcademicTerm.Name), nameof(SectionDto.AcademicTermName))]
    public partial SectionDto ToDto(Section section);
    public partial Section ToEntity(CreateSectionDto dto);
    public partial void UpdateSection(UpdateSectionDto dto, Section section);

    private string MapProfessorName(Professor professor) => $"{professor.User.FirstName} {professor.User.LastName}";
}
