namespace CurrencyConverter.WebApi.Models;

public record CurrencyConversion(string From, string To);

public record CurrencyConversionRate(string From, string To, double Rate);

