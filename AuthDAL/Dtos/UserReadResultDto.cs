
using System.Collections.Generic;
using AuthDAL.Models;

namespace AuthDAL.Dtos;

public class UserReadResultDto : UserReadUnlinkedResultDto, IDtoResultBase
{
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}