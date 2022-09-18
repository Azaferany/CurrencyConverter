
using BenchmarkDotNet.Attributes;

namespace CurrencyConverter.Benchmark;

public class UpdateConfigurationBenchmark
{
    [Benchmark]
    public async Task UpdateConfiguration()
    {
        var currencyConverter = new WebApi.Services.CurrencyConverter();
        await currencyConverter.UpdateConfigurationAsync(new List<Tuple<string, string, double>>()
        {
            new("1", "1-4", 4), new("1", "1-5", 5), new("1", "1-6", 6), new("1-8", "1-16", 2),

        });
    }
}
