using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using UNC.IdentityModel;

namespace IdentityServer4.Storage.Stores.Serialization
{
    public class ClaimsPrincipalConverter : JsonConverter<ClaimsPrincipal>
    {
        public override ClaimsPrincipal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonDocument.TryParseValue(ref reader, out var doc))
            {
                var root = doc.RootElement;
                var claimsArray = root.GetProperty("Claims").EnumerateArray();
                var authenticationType = root.GetProperty("AuthenticationType").GetString();

                var claims = claimsArray.Select(c =>
                    new Claim(
                        c.GetProperty("Type").GetString(),
                        c.GetProperty("Value").GetString(),
                        c.GetProperty("ValueType").GetString())
                ).ToList();

                var identity = new ClaimsIdentity(claims, authenticationType, JwtClaimTypes.Name, JwtClaimTypes.Role);
                return new ClaimsPrincipal(identity);
            }

            throw new JsonException("Invalid JSON for ClaimsPrincipal conversion.");
        }

        public override void Write(Utf8JsonWriter writer, ClaimsPrincipal value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("AuthenticationType", value.Identity.AuthenticationType);
            writer.WriteStartArray("Claims");

            foreach (var claim in value.Claims)
            {
                writer.WriteStartObject();
                writer.WriteString("Type", claim.Type);
                writer.WriteString("Value", claim.Value);
                writer.WriteString("ValueType", claim.ValueType);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
