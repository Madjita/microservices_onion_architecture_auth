using System.Collections.Generic;
using AuthDAL.Models;

namespace AuthDAL.Dto.Generic;

public class RedirectResultDto : IDtoResultBase
{
    public string Url { get; set; }
    public bool IsPermanent { get; set; }
    public bool PreserveMethod { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}