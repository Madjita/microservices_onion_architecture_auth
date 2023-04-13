using System;
using AuthDAL.Enums;
using Microsoft.AspNetCore.Http;
namespace AuthDAL.Exceptions;

public class HttpResponseException : Exception
{
    public HttpResponseException(
        int? statusCode,
        ErrorType? type,
        string message = null,
        Exception innerException = null
    ) : base(message, innerException)
    {
        StatusCode = statusCode ?? StatusCodes.Status500InternalServerError;
        Type = type ?? ErrorType.None;
    }

    public int StatusCode { get; }
    public ErrorType Type { get; }
}