using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CurrencyConverter.WebApi.Models;
using CurrencyConverter.WebApi.Services.Contracts;
using Spare;

namespace CurrencyConverter.WebApi.Services;

public class CurrencyConverter : ICurrencyConverter
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private double[,] _conversionTable = { };
    private List<string> _currencies = new();
    private Dictionary<CurrencyConversion, double> _currencyConversions = new();

    public void ClearConfiguration()
    {
        _semaphore.Wait();
        _conversionTable = new double[,] { };
        _currencies.Clear();
        _currencyConversions.Clear();
        _semaphore.Release();
    }

    public Dictionary<CurrencyConversion, double> GetDefinedCurrencyConversions()
    {
        return _currencyConversions;
    }

    public async Task UpdateConfigurationAsync(IEnumerable<Tuple<string, string, double>> conversionRates)
    {
        try
        {
            var currencyConversions = new Dictionary<CurrencyConversion, double>();
            var currencies = new List<string>();

            foreach (var (key, value) in _currencyConversions)
                currencyConversions[key] = value;

            foreach (var currency in _currencies)
                currencies.Add(currency);

            foreach (var (from, to, rate) in conversionRates)
            {
                currencies.Add(from.ToLower());
                currencies.Add(to.ToLower());

                currencyConversions[new CurrencyConversion(from.ToLower(), to.ToLower())] = rate;
            }
            currencies = currencies.Distinct().ToList();

            var conversionTable = new double[currencies.Count, currencies.Count];

            foreach (var currency in currencies)
            {
                foreach (var (key, value) in currencyConversions.Where(x => x.Key.From == currency))
                {
                    conversionTable[currencies.IndexOf(key.From), currencies.IndexOf(key.To)] = value;
                    conversionTable[currencies.IndexOf(key.To), currencies.IndexOf(key.From)] = 1 / value;
                }
            }

            await _semaphore.WaitAsync();

            CompleteTable(conversionTable, currencies);
            Console.WriteLine(conversionTable.Deco());
            _conversionTable = conversionTable;
            _currencyConversions = currencyConversions;
            _currencies = currencies;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<double> ConvertAsync(string fromCurrency, string toCurrency, double amount)
    {
        await _semaphore.WaitAsync();
        _semaphore.Release();

        fromCurrency = fromCurrency.ToLower();
        toCurrency = toCurrency.ToLower();

        if (fromCurrency == toCurrency)
            return amount;

        var rate = _conversionTable[_currencies.IndexOf(fromCurrency), _currencies.IndexOf(toCurrency)];

        if (rate == 0)
            throw new DataException("Cant convert amount due to lack of data");

        return rate * amount;
    }

    private void CompleteTable(double[,] conversionTable, List<string> currencies)
    {
        bool newItemAdded;
        do
        {
            newItemAdded = false;
            var list = new List<(int i, int j)>();
            for (int i = 0; i < conversionTable.GetLength(0); i++)
                for (int j = 0; j < conversionTable.GetLength(1); j++)
                    if (conversionTable[i, j] != 0)
                        list.Add((i, j));

            foreach (var currency in currencies)
            {
                var rows = list.Where(x => x.i == currencies.IndexOf(currency)).ToList();
                if (rows.Count > 1)
                    foreach (var x in rows)
                        foreach (var y in rows.Where(y => y != x))
                            if (conversionTable[x.j, y.j] == 0)
                            {
                                conversionTable[x.j, y.j] = conversionTable[y.i, y.j] / conversionTable[x.i, x.j];
                                newItemAdded = newItemAdded || true;
                            }
                            else
                            {
                                if (Math.Abs(conversionTable[x.j, y.j] - conversionTable[y.i, y.j] / conversionTable[x.i, x.j]) > 0.0000000000000099)
                                {
                                    throw new InvalidDataException("Invalid Rates");
                                }
                            }

                var columns = list.Where(x => x.j == currencies.IndexOf(currency)).ToList();
                if (columns.Count > 1)
                    foreach (var x in columns)
                        foreach (var y in columns.Where(y => y != x))
                            if (conversionTable[x.i, y.i] == 0)
                            {
                                conversionTable[x.i, y.i] = conversionTable[x.i, x.j] / conversionTable[y.i, y.j];
                                newItemAdded = newItemAdded || true;
                            }
                            else
                            {
                                if (Math.Abs(conversionTable[x.i, y.i] - (conversionTable[x.i, x.j] / conversionTable[y.i, y.j])) > 0.0000000000000099)
                                {
                                    throw new InvalidDataException("Invalid Rates");
                                }
                            }
            }
        } while (newItemAdded);
    }
}
