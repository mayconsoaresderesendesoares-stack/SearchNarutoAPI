using System.Text.Json;
using System.Text.Json.Serialization;

namespace SearchNarutoAPI.Models
{
    public class NarutoResponse
    {
        public List<NarutoCharacter>? characters { get; set; }
    }

    public class NarutoCharacter
    {
        public int id { get; set; }
        public string? name { get; set; }
        public List<string>? images { get; set; }
        public List<string>? jutsu { get; set; }
        public PersonalInfo? personal { get; set; }
        public RankInfo? rank { get; set; }
    }

    public class PersonalInfo
    {
        [JsonConverter(typeof(FlexibleStringConverter))]
        public string? clan { get; set; }
        
        [JsonConverter(typeof(StringOrListConverter))]
        public List<string>? affiliation { get; set; }
    }

    public class RankInfo
    {
        [JsonConverter(typeof(FlexibleStringConverter))]
        public string? ninjaRank { get; set; }
    }

    public class FlexibleStringConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    return prop.Value.ToString();
                }
            }
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                return doc.RootElement.ToString();
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }

    public class StringOrListConverter : JsonConverter<List<string>?>
    {
        public override List<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new List<string> { reader.GetString() ?? "" };
            }
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var list = new List<string>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.String)
                        list.Add(reader.GetString() ?? "");
                }
                return list;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, List<string>? value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}