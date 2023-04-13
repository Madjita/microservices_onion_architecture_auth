
using System.Collections.Generic;
using AuthDAL.Models;

namespace AuthDAL.Dto.Generic;

public class OkResultDto : IDtoResultBase
{
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}