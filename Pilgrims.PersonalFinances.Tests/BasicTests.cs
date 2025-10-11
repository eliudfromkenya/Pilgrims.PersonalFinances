using Xunit;
using FluentAssertions;

namespace Pilgrims.PersonalFinances.Tests;

public class BasicTests
{
    [Fact]
    public void BasicTest_ShouldPass()
    {
        // Arrange
        var expected = 5;
        
        // Act
        var actual = 2 + 3;
        
        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(5, 5, 10)]
    [InlineData(-1, 1, 0)]
    public void Addition_ShouldReturnCorrectSum(int a, int b, int expected)
    {
        // Act
        var result = a + b;
        
        // Assert
        result.Should().Be(expected);
    }
}