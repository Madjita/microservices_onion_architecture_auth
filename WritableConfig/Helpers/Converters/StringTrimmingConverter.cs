using System.Text.Json;
using System.Text.Json.Serialization;
namespace JsonHelpers.Converters;

public class StringTrimmingConverter : JsonConverter<string>
{
    /// <summary>
    ///     Trim the input string
    /// </summary>
    /// <param name="reader">reader</param>
    /// <param name="typeToConvert">Object type</param>
    /// <param name="options">Existing Value</param>
    /// <returns></returns>
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()!.Trim();
    }

    /// <summary>
    ///     Trim the output string
    /// </summary>
    /// <param name="writer">Writer</param>
    /// <param name="value">value</param>
    /// <param name="options">serializer</param>
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (value.GetType().GetCustomAttributes(true).OfType<Attribute>().Any())
        {
            writer.WriteStringValue(value);
            return;
        }

        writer.WriteStringValue(value?.Trim());
    }
}