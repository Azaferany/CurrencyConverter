// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.WebApi.Services.Contracts;

public interface ICurrencyConverter
{
    Task UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates);
    Task<double> Convert(string fromCurrency, string toCurrency, double amount);
    void ClearConfiguration();
}
