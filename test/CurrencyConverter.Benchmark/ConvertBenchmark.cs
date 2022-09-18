
using BenchmarkDotNet.Attributes;

namespace CurrencyConverter.Benchmark;

public class ConvertBenchmark
{
    private WebApi.Services.CurrencyConverter _converter;

    [GlobalSetup]
    public async Task Setup()
    {
        _converter = new WebApi.Services.CurrencyConverter();
        await _converter.UpdateConfigurationAsync(new List<Tuple<string, string, double>>()
        {
            new("1", "1-4", 4), new("1", "1-5", 5), new("1", "1-6", 6), new("1-5", "1-15", 3),

        });
    }

    [Benchmark]
    public async Task Convert()
    {
        await _converter.ConvertAsync("1-15", "1", 60);
    }
}
