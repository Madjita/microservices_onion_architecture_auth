using System.Collections.Generic;
using AuthDAL.Enums;

namespace AuthDAL.Models;

public class WarningModelResultEntry
{
    public WarningModelResultEntry(
        WarningType warningType,
        string message,
        WarningEntryType warningEntryType = WarningEntryType.None
    )
    {
        WarningType = warningType;
        Message = message;
        WarningEntryType = warningEntryType;
    }

    public WarningType WarningType { get; }
    public string Message { get; }
    public WarningEntryType WarningEntryType { get; }
}

public interface IWarningModelResult
{
    public List<WarningModelResultEntry> Warnings { get; set; }
}