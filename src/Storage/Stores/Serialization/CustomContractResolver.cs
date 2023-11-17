using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IdentityServer4.Storage.Stores.Serialization
{
    public class CustomContractResolver : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // Implement logic to determine if the converter should handle the given type
            return true;
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Implement custom deserialization logic if necessary
            return JsonSerializer.Deserialize(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var type = value.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.GetMethod != null);

            writer.WriteStartObject();

            foreach (var property in properties)
            {
                var propValue = property.GetValue(value);
                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, propValue, property.PropertyType, options);
            }

            writer.WriteEndObject();
        }
    }
}