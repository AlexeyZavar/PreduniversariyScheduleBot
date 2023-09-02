#region

using System;

#endregion

namespace MarkBot.Parsers.Entities;

public class Mark : IEquatable<Mark>
{
    public string Id { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public string Subject { get; set; }
    public DateTimeOffset Date { get; set; }

    /// <inheritdoc />
    public bool Equals(Mark? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Date:dd.MM} {Subject}: {Value} (\"{Description}\")";
    }

    /// <inheritdoc />
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

        return Equals((Mark)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}
