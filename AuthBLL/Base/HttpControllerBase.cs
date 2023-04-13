using AuthBLL.Services;
using AuthDAL.Models;
using Logging.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuthService.Authentication;
using System.Linq;
using AuthDAL.Dto.Generic;

namespace AuthBLL.Base;

/// <summary>
/// Base Controller every Controller must implement
/// </summary>
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status500InternalServerError)]
public class HttpControllerBase : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly ILogger _logger;
    private readonly IWarningService _warningService;

    public HttpControllerBase(
        IHttpContextAccessor httpContextAccessor,
        IWarningService warningService,
        ICustomLoggerFactory customLoggerFactory
    )
    {
        _warningService = warningService;
        _logger = (ILogger)customLoggerFactory.Create("HttpControllerBase");
        _httpContext = httpContextAccessor.HttpContext;
    }

    /// <summary>
    ///     Used to response with the result from Handler
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    protected IActionResult ResponseWith(IDtoResultBase response)
    {
        response.TraceId = _httpContext.TraceIdentifier;

        var warnings = _warningService.GetAll();


        response.Warnings = warnings.Any() ? warnings : null;

        if (response.Errors != null && response.Errors.Any())
            return new BadRequestObjectResult(response);

        if (response is RedirectResultDto redirect)
            return new RedirectResult(redirect.Url, redirect.IsPermanent, redirect.PreserveMethod);

        return new OkObjectResult(response);
    }
}