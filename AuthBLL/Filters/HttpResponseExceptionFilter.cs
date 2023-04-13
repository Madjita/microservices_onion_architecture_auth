using System;
using AuthDAL.Enums;
using AuthDAL.Exceptions;
using AuthDAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthBLL.Filters;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var errorModelResult = new ErrorModelResult
        {
            TraceId = context.HttpContext.TraceIdentifier
        };

        var environment = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>() ??
                          throw new ApplicationException(Localize.Error.DependencyInjectionFailed.ToString());
        var logger = context.HttpContext.RequestServices.GetService<ILogger<HttpResponseExceptionFilter>>() ??
                     throw new ApplicationException(Localize.Error.DependencyInjectionFailed.ToString());

        switch (context.Exception)
        {
            case HttpResponseException httpResponseException:
                errorModelResult.Errors.Add(new ErrorModelResultEntry(httpResponseException.Type,
                    httpResponseException.Message, ErrorEntryType.Message));

                logger.LogWarning(context.Exception, Localize.Error.HandledExceptionHttpResponseException.ToString());

                if (environment.IsDevelopment())
                {
                    errorModelResult.Errors.Add(new ErrorModelResultEntry(httpResponseException.Type, context.Exception.StackTrace, ErrorEntryType.StackTrace));
                    errorModelResult.Errors.Add(new ErrorModelResultEntry(httpResponseException.Type, context.Exception.Source, ErrorEntryType.Source));
                    errorModelResult.Errors.Add(new ErrorModelResultEntry(httpResponseException.Type, context.HttpContext.Request.Path, ErrorEntryType.Path));
                }

                context.Result = new ObjectResult(errorModelResult)
                {
                    StatusCode = httpResponseException.StatusCode
                };

                context.ExceptionHandled = true;
                break;
            case CustomException customException:
                errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Generic,
                    customException.Message, ErrorEntryType.Message));

                logger.LogWarning(context.Exception, Localize.Error.HandledExceptionCustomException.ToString());

                if (environment.IsDevelopment())
                {
                    errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Generic, context.Exception.StackTrace, ErrorEntryType.StackTrace));
                    errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Generic, context.Exception.Source, ErrorEntryType.Source));
                    errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Generic, context.HttpContext.Request.Path, ErrorEntryType.Path));
                }

                context.Result = new ObjectResult(errorModelResult)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                context.ExceptionHandled = true;
                break;
            case { } exception:
                //This is handled by /Error controller

                context.ExceptionHandled = false;
                break;
        }
    }

    public int Order => int.MaxValue - 10;
}