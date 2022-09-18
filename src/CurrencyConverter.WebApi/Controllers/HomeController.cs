// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using CurrencyConverter.WebApi.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.WebApi.Controllers;

[ApiController]
[Route("v{version:apiVersion}/[controller]/[action]")]
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
    /// Get Weather Forecast
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "ClearConfiguration")]
    public ActionResult ClearConfiguration()
    {
        _currencyConverter.ClearConfiguration();
        return Ok();
    }
}
