#region

using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace MarkBot.Schedule;

public class ScheduleData
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("groups")] public string[] Groups { get; set; }

    [JsonPropertyName("data")] public ScheduleLesson[] Data { get; set; }
}

public class ScheduleLesson
{
    [JsonPropertyName("day")] public string Day { get; set; }

    [JsonPropertyName("hour")] public long Hour { get; set; }

    [JsonPropertyName("subject")] public string Subject { get; set; }

    [JsonPropertyName("teacher")] public string Teacher { get; set; }

    [JsonPropertyName("venue")]
    [JsonConverter(typeof(VenueJsonConverter))]
    public Venue Venue { get; set; }
}

public struct Venue
{
    public override string? ToString()
    {
        if (Integer != null)
        {
            return Integer.ToString();
        }

        if (!string.IsNullOrWhiteSpace(String))
        {
            return String;
        }

        return null;
    }

    public long? Integer;
    public string String;

    public static implicit operator Venue(long integer)
    {
        return new Venue { Integer = integer };
    }

    public static implicit operator Venue(string @string)
    {
        return new Venue { String = @string };
    }

    public bool IsNull => Integer == null && String == null;
}

public class VenueJsonConverter : JsonConverter<Venue>
{
    public override Venue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new Venue
            {
                String = reader.GetString() ?? "FUCKED"
            };
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return new Venue
            {
                Integer = reader.GetInt64()
            };
        }

        return new Venue();
    }

    public override void Write(Utf8JsonWriter writer, Venue value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
