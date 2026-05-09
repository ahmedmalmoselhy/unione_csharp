using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Application.Services;

public class GpaService : IGpaService
{
    private readonly IApplicationDbContext _context;

    public GpaService(IApplicationDbContext context)
    {
        _context = context;
    }

    public decimal CalculateGradePoints(string gradeLetter)
    {
        return gradeLetter.ToUpper() switch
        {
            "A+" => 4.0m,
            "A" => 4.0m,
            "A-" => 3.7m,
            "B+" => 3.3m,
            "B" => 3.0m,
            "B-" => 2.7m,
            "C+" => 2.3m,
            "C" => 2.0m,
            "C-" => 1.7m,
            "D+" => 1.3m,
            "D" => 1.0m,
            "F" => 0.0m,
            _ => 0.0m
        };
    }

    public string GetGradeLetter(decimal score)
    {
        return score switch
        {
            >= 95 => "A+",
            >= 90 => "A",
            >= 85 => "A-",
            >= 80 => "B+",
            >= 75 => "B",
            >= 70 => "B-",
            >= 65 => "C+",
            >= 60 => "C",
            >= 55 => "C-",
            >= 50 => "D+",
            >= 45 => "D",
            _ => "F"
        };
    }

    public async Task RecalculateStudentGpa(long studentId)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null) return;

        var allGrades = await _context.Grades
            .Include(g => g.Section)
            .ThenInclude(s => s.Course)
            .Where(g => g.StudentId == studentId && g.IsPublished)
            .ToListAsync();

        if (!allGrades.Any())
        {
            student.Gpa = 0;
            await _context.SaveChangesAsync(default);
            return;
        }

        // Calculate Cumulative GPA
        decimal totalPoints = allGrades.Sum(g => g.GradePoints * g.Section.Course.CreditHours);
        decimal totalHours = allGrades.Sum(g => g.Section.Course.CreditHours);

        student.Gpa = totalHours > 0 ? totalPoints / totalHours : 0;

        // Calculate Term GPAs and Update StudentTermGpa records
        var gradesByTerm = allGrades.GroupBy(g => g.Section.AcademicTermId);

        foreach (var termGrades in gradesByTerm)
        {
            var termId = termGrades.Key;
            decimal termPoints = termGrades.Sum(g => g.GradePoints * g.Section.Course.CreditHours);
            decimal termHours = termGrades.Sum(g => g.Section.Course.CreditHours);
            decimal termGpaValue = termHours > 0 ? termPoints / termHours : 0;

            var termGpa = await _context.StudentTermGpas
                .FirstOrDefaultAsync(tg => tg.StudentId == studentId && tg.AcademicTermId == termId);

            if (termGpa == null)
            {
                termGpa = new StudentTermGpa
                {
                    StudentId = studentId,
                    AcademicTermId = termId
                };
                _context.StudentTermGpas.Add(termGpa);
            }

            termGpa.TermGpa = termGpaValue;
            termGpa.CumulativeGpa = student.Gpa.Value;
            termGpa.CreditsAttempted = termHours;
            termGpa.CreditsEarned = termGrades.Where(g => g.GradeLetter != "F").Sum(g => g.Section.Course.CreditHours);
            termGpa.CalculatedAt = DateTime.UtcNow;
            termGpa.AcademicStanding = student.Gpa >= 2.0m ? AcademicStanding.GoodStanding : AcademicStanding.Probation;
        }

        student.AcademicStanding = student.Gpa >= 2.0m ? AcademicStanding.GoodStanding : AcademicStanding.Probation;

        await _context.SaveChangesAsync(default);
    }

    public async Task<StudentTermGpaDto> GetTermGpa(long studentId, long academicTermId)
    {
        var termGpa = await _context.StudentTermGpas
            .FirstOrDefaultAsync(tg => tg.StudentId == studentId && tg.AcademicTermId == academicTermId);

        if (termGpa == null) return null!;

        return new StudentTermGpaDto
        {
            Id = termGpa.Id,
            StudentId = termGpa.StudentId,
            AcademicTermId = termGpa.AcademicTermId,
            TermGpa = termGpa.TermGpa,
            CumulativeGpa = termGpa.CumulativeGpa,
            CreditsAttempted = termGpa.CreditsAttempted,
            CreditsEarned = termGpa.CreditsEarned,
            AcademicStanding = termGpa.AcademicStanding,
            CalculatedAt = termGpa.CalculatedAt
        };
    }
}
