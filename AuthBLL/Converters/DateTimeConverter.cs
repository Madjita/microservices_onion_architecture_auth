using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuthBLL.Converters;

public class DateTimeConverter : ValueConverter<DateTime, int>
{
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public DateTimeConverter()
        : base(
            d => (int) (d.ToUniversalTime() - UnixEpoch).TotalSeconds,
            d => UnixEpoch.AddSeconds(d))
    {
    }
}