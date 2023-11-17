using System.Text.Json.Serialization;

namespace UNC.IdentityModel.Client.Messages;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Metadata,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(DynamicClientRegistrationDocument))]
internal partial class ClientMessagesSourceGenerationContext : JsonSerializerContext
{
}
