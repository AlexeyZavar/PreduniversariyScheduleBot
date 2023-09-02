namespace MarkBot.Schedule;

public class ParsedDay
{
    public string Name { get; set; }
    public ParsedLesson[] Lessons { get; set; }
}

public class ParsedLesson : IEquatable<ParsedLesson>
{
    public string Name { get; set; }
    public long Hour { get; set; }
    public string? Teacher { get; set; }
    public string? Room { get; set; }

    public bool Equals(ParsedLesson? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Name == other.Name && Hour == other.Hour && Teacher == other.Teacher &&
               Room == other.Room;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((ParsedLesson)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Hour, Teacher, Room);
    }
}
