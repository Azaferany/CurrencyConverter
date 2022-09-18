using FluentAssertions;

namespace CurrencyConverter.UnitTest;

public class CurrencyConverterTest
{
    [Fact]
    public async Task CanConvertAllPossibleCurrencyConvertFor4Currency()
    {
        var currencyConverter = new WebApi.Services.CurrencyConverter();
        double f = 4;
        double t = 3;
        await currencyConverter.UpdateConfigurationAsync(new List<Tuple<string, string, double>>()
        {
            new("0.1", "0.2", 2), new("0.1", "0.3", 3), new("0.3", "0.4", f / t),
        });
        var enumerable = Enumerable.Range(1, 4);

        foreach (double x in enumerable)
        {
            foreach (double y in enumerable.Where(y => y != x))
            {
                double res = await currencyConverter.ConvertAsync((x / 10.0).ToString(), (y / 10.0).ToString(), 60);

                double expected = y / x * 60;
                res.Should().Be(expected, $"on {x} -> {y}");
            }
        }
    }

    [Fact]
    public async Task FailOnInvalidCurrencyConvertWhenCastIsNotMatchOnDifferentCastPath()
    {
        var currencyConverter = new WebApi.Services.CurrencyConverter();

        Func<Task> x = new(() => currencyConverter.UpdateConfigurationAsync(
            new List<Tuple<string, string, double>>()
            {
                new("0.1", "0.2", 2), new("0.2", "0.3", 3), new("0.3", "0.1", 500),
            }));
        await Assert.ThrowsAsync(typeof(InvalidDataException), x);

    }
}
