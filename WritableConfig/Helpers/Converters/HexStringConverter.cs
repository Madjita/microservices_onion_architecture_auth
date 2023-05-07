using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonHelpers.Converters;

public class HexStringToUInt32Converter : JsonConverter<uint>
{
    public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hexString = reader.GetString();

        if (string.IsNullOrEmpty(hexString))
            throw new JsonException("Input string is null or empty!");
        
        if (!hexString.StartsWith("0x"))
            throw new JsonException("Input string is not a hex encoded value!");

        return uint.Parse(hexString[2..], System.Globalization.NumberStyles.HexNumber);
    }

    public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
    {
        writer.WriteStringValue("0x" + value.ToString("X"));
    }
}

public class HexStringToUInt8Converter : JsonConverter<byte>
{
    public override byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hexString = reader.GetString();

        if (string.IsNullOrEmpty(hexString))
            throw new JsonException("Input string is null or empty!");
        
        if (!hexString.StartsWith("0x"))
            throw new JsonException("Input string is not a hex encoded value!");

        return byte.Parse(hexString[2..], System.Globalization.NumberStyles.HexNumber);
    }

    public override void Write(Utf8JsonWriter writer, byte value, JsonSerializerOptions options)
    {
        writer.WriteStringValue("0x" + value.ToString("X"));
    }
}