// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CurrencyConverter.WebApi.Models;
using CurrencyConverter.WebApi.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.WebApi.Controllers;

[ApiController]
[Route("v{version:apiVersion}/[action]")]
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Produces("application/json")]
[Consumes("application/json")]
[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType((int)HttpStatusCode.OK)]
public class HomeController : ControllerBase
{
    private readonly ICurrencyConverter _currencyConverter;

    public HomeController(ICurrencyConverter currencyConverter) => _currencyConverter = currencyConverter;

    /// <summary>
    /// Clear Configuration
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "ClearConfiguration")]
    public ActionResult ClearConfiguration()
    {
        _currencyConverter.ClearConfiguration();
        return Ok();
    }

    /// <summary>
    /// Get Defined Currency Conversions
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetDefinedCurrencyConversions")]
    public ActionResult<Dictionary<CurrencyConversion, double>> GetDefinedCurrencyConversions()
    {
        return Ok(_currencyConverter.GetDefinedCurrencyConversions());
    }

    /// <summary>
    /// Update Configuration
    /// </summary>
    /// <returns></returns>
    [HttpPut(Name = "UpdateConfiguration")]
    public async Task<ActionResult> UpdateConfigurationAsync([FromBody]IEnumerable<Tuple<string, string, double>> conversionRates)
    {
        try
        {
           await _currencyConverter.UpdateConfigurationAsync(conversionRates);
           return Ok();
        }
        catch (InvalidDataException e)
        {
            return ValidationProblem(e.Message);
        }
    }

    /// <summary>
    /// Convert
    /// </summary>
    /// <returns></returns>
    [HttpPut(Name = "Convert")]
    public async Task<ActionResult<double>> ConvertAsync(string fromCurrency, string toCurrency, double amount)
    {
        try
        {
            return Ok(await _currencyConverter.ConvertAsync(fromCurrency,toCurrency,amount));
        }
        catch (DataException e)
        {
            return ValidationProblem(e.Message);
        }
    }
}
