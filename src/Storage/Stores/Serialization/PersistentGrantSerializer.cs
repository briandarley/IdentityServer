using System.Text.Json;

namespace IdentityServer4.Storage.Stores.Serialization
{
    /// <summary>
    /// JSON-based persisted grant serializer
    /// </summary>
    /// <seealso cref="IPersistentGrantSerializer" />
    public class PersistentGrantSerializer : IPersistentGrantSerializer
    {
        private static readonly JsonSerializerOptions _options;

        static PersistentGrantSerializer()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            _options.Converters.Add(new ClaimConverter());
            _options.Converters.Add(new ClaimsPrincipalConverter());
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, _options);
        }

        /// <summary>
        /// Deserializes the specified string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }
    }
}