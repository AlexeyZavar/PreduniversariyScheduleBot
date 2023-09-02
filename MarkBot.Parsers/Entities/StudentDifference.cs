#region

using System.Collections.Generic;

#endregion

namespace MarkBot.Parsers.Entities;

public class StudentDifference
{
    public Student Student { get; set; }

    public List<Mark> MarksAdd { get; set; } = new();
    public List<Mark> MarksRemove { get; set; } = new();
    public List<MarkDifference> MarksChange { get; set; } = new();

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Differences for {Student} {MarksAdd.Count} {MarksRemove.Count} {MarksChange.Count}";
    }
}
