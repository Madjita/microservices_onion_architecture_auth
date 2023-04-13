using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuthBLL.Converters;

public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, long>
{
    public DateTimeOffsetConverter() : base(
        d => d.ToUniversalTime().ToUnixTimeSeconds(),
        d => DateTimeOffset.FromUnixTimeSeconds(d))
    {
    }
}