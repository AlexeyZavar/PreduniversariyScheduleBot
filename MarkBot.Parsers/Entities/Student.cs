#region

using System;
using System.Collections.Generic;

#endregion

namespace MarkBot.Parsers.Entities;

public sealed class Student : IEquatable<Student>
{
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;

    public List<Mark> Marks { get; set; } = new();

    /// <inheritdoc />
    public bool Equals(Student? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return FirstName == other.FirstName && LastName == other.LastName;
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

        return Equals((Student)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(FirstName, LastName);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{FirstName} {MiddleName} {LastName} ({Marks.Count})";
    }
}
