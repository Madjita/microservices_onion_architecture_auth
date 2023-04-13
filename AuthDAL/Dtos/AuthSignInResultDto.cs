using System;
using System.Collections.Generic;
using AuthDAL.Models;

namespace AuthDAL.Dtos;

public class AuthSignInResultDto : IDtoResultBase
{
    public string Email { get; set; }
    public string JsonWebToken { get; set; }
    public DateTimeOffset JsonWebTokenExpiresAt { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}