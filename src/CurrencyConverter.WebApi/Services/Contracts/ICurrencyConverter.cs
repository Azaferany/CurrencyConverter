// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyConverter.WebApi.Models;

namespace CurrencyConverter.WebApi.Services.Contracts;

public interface ICurrencyConverter
{
    Dictionary<CurrencyConversion, double> GetDefinedCurrencyConversions();
    Task UpdateConfigurationAsync(IEnumerable<Tuple<string, string, double>> conversionRates);
    Task<double> Convert(string fromCurrency, string toCurrency, double amount);
    void ClearConfiguration();
}
