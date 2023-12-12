using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DemoMsUser.Common.Converters
{
    public class StringOrIntConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Integer)
            {
                return token.Value<int>();
            }
            else if (token.Type == JTokenType.String)
            {
                return token.Value<string>();
            }
            else
            {
                throw new JsonSerializationException("Expected integer or string");
            }
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is int)
            {
                writer.WriteValue((int)value);
            }
            else if (value is string)
            {
                writer.WriteValue((string)value);
            }
            else
            {
                throw new JsonSerializationException("Expected integer or string");
            }
        }
    }
}
