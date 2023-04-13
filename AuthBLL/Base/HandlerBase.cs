#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuthDAL.Enums;
using AuthDAL.Models;

namespace AuthBLL.Base;

public interface IHandlerBase
{
    public ErrorModelResult? ValidateModel(object data);
}

/// <summary>
/// Base Handler every Handler must implement
/// </summary>
public class HandlerBase : IHandlerBase
{
    /// <summary>
    ///     Validates a model
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public ErrorModelResult? ValidateModel(object data)
    {
        var context = new ValidationContext(data);
        var validationResults = new List<ValidationResult>();

        if (Validator.TryValidateObject(data, context, validationResults, true))
            return null;

        var errorModelResult = new ErrorModelResult
        {
            Errors = new List<ErrorModelResultEntry>()
        };

        foreach (var validationResult in validationResults)
            errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.ModelState, validationResult.ErrorMessage));

        return errorModelResult;
    }
}