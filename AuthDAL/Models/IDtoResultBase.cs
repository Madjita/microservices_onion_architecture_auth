using System.Text.Json.Serialization;

namespace AuthDAL.Models;

public interface IDtoResultBase : IWarningModelResult, IErrorModelResult
{
    public string TraceId { get; set; }

    [JsonPropertyName("$type")]
    public string Type => GetType().FullName;
}