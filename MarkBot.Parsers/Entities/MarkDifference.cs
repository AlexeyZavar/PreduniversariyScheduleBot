namespace MarkBot.Parsers.Entities;

public class MarkDifference
{
    public Mark Outdated { get; set; }
    public Mark Updated { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Outdated} -> {Updated}";
    }
}
