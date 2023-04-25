using FluentAssertions;
using FluentAssertions.Execution;

namespace OpenTabletDriver.UI.Tests;

public class StringUtilityTests
{
    [Fact]
    public void TestTryParseFloat()
    {
        static void Assert(string? input, bool expectedResult, float expectedValue = 0)
        {
            var result = StringUtility.TryParseFloat(input, out float floatResult);
            result.Should().Be(expectedResult, $"because input is \"{input}\"");
            floatResult.Should().Be(expectedValue, $"because input is \"{input}\"");
        }

        Assert(null, true);
        Assert("", true);
        Assert("5", true, 5f);
        Assert("0.5", true, 0.5f);
        Assert("-", true, 0f);
        Assert("-.", true, 0f);
        Assert(".", true, 0f);
        Assert("-.5", true, -0.5f);
        Assert("5.", true, 5f);
        Assert("-5.", true, -5f);
    }
}
