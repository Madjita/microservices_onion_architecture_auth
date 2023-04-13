using System.Collections.Generic;
using AuthDAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AuthBLL.Services;

public interface IWarningService
{
    void Add(WarningModelResultEntry warningModelResultEntry);
    List<WarningModelResultEntry> GetAll();
}

public class WarningService : IWarningService
{
    private readonly HttpContext _httpContext;

    private readonly ILogger<WarningService> _logger;
    private readonly List<WarningModelResultEntry> _warningModelResultEntries;

    public WarningService(ILogger<WarningService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _warningModelResultEntries = new List<WarningModelResultEntry>();
        _httpContext = httpContextAccessor.HttpContext;
    }


    public void Add(WarningModelResultEntry warningModelResultEntry)
    {
        _warningModelResultEntries.Add(warningModelResultEntry);
    }

    public List<WarningModelResultEntry> GetAll()
    {
        return _warningModelResultEntries;
    }
}