using System;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IdentityServer4.Storage.Stores.Serialization
{
    public class ClaimConverter : JsonConverter<Claim>
    {
        public override Claim Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonDocument.TryParseValue(ref reader, out var doc))
            {
                var root = doc.RootElement;

                // Assuming ClaimLite is a simple class with Type, Value, and ValueType properties
                var type = root.GetProperty("Type").GetString();
                var value = root.GetProperty("Value").GetString();
                var valueType = root.GetProperty("ValueType").GetString();

                return new Claim(type, value, valueType);
            }

            throw new JsonException("Invalid JSON for Claim conversion.");
        }

        public override void Write(Utf8JsonWriter writer, Claim value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("Type", value.Type);
            writer.WriteString("Value", value.Value);
            writer.WriteString("ValueType", value.ValueType);

            writer.WriteEndObject();
        }
    }
}