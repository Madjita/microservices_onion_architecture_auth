using System.Collections.Generic;
using AuthDAL.Enums;

namespace AuthDAL.Models;

public class ErrorModelResultEntry
{
    public ErrorModelResultEntry(
        ErrorType errorType,
        string message,
        ErrorEntryType errorEntryType = ErrorEntryType.None
    )
    {
        ErrorType = errorType;
        Message = message;
        ErrorEntryType = errorEntryType;
    }

    public ErrorType ErrorType { get; }
    public string Message { get; }
    public ErrorEntryType ErrorEntryType { get; }
}

public interface IErrorModelResult
{
    public List<ErrorModelResultEntry> Errors { get; set; }
}

public class ErrorModelResult : IDtoResultBase
{
    public ErrorModelResult()
    {
        Errors = new List<ErrorModelResultEntry>();
        Warnings = new List<WarningModelResultEntry>();
    }

    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}