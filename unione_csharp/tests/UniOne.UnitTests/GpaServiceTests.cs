using Moq;
using UniOne.Application.Contracts;
using UniOne.Application.Services;
using Xunit;

namespace UniOne.UnitTests;

public class GpaServiceTests
{
    private readonly GpaService _gpaService;
    private readonly Mock<IApplicationDbContext> _mockContext;

    public GpaServiceTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _gpaService = new GpaService(_mockContext.Object);
    }

    [Theory]
    [InlineData("A+", 4.0)]
    [InlineData("A", 4.0)]
    [InlineData("B+", 3.3)]
    [InlineData("C", 2.0)]
    [InlineData("D", 1.0)]
    [InlineData("F", 0.0)]
    [InlineData("Unknown", 0.0)]
    public void CalculateGradePoints_ShouldReturnCorrectPoints(string letter, decimal expected)
    {
        var result = _gpaService.CalculateGradePoints(letter);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(95, "A+")]
    [InlineData(90, "A")]
    [InlineData(85, "A-")]
    [InlineData(80, "B+")]
    [InlineData(75, "B")]
    [InlineData(70, "B-")]
    [InlineData(65, "C+")]
    [InlineData(60, "C")]
    [InlineData(55, "C-")]
    [InlineData(50, "D+")]
    [InlineData(45, "D")]
    [InlineData(40, "F")]
    public void GetGradeLetter_ShouldReturnCorrectLetter(decimal score, string expected)
    {
        var result = _gpaService.GetGradeLetter(score);
        Assert.Equal(expected, result);
    }
}
