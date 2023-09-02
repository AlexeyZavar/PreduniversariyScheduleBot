#region

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#endregion

namespace MarkBot.Services;

public sealed class TimeService
{
    // LESSON: <START, END>
    private static readonly Dictionary<int, (TimeOnly start, TimeOnly end)> _lessons = new()
    {
        { 1, (TimeOnly.Parse("9:00"), TimeOnly.Parse("9:45")) },
        { 2, (TimeOnly.Parse("9:55"), TimeOnly.Parse("10:40")) },
        { 3, (TimeOnly.Parse("11:00"), TimeOnly.Parse("11:45")) },
        { 4, (TimeOnly.Parse("12:05"), TimeOnly.Parse("12:50")) },
        { 5, (TimeOnly.Parse("13:00"), TimeOnly.Parse("13:45")) },
        { 6, (TimeOnly.Parse("14:05"), TimeOnly.Parse("14:50")) },
        { 7, (TimeOnly.Parse("15:10"), TimeOnly.Parse("15:55")) },
        { 8, (TimeOnly.Parse("16:05"), TimeOnly.Parse("16:50")) },
        { 9, (TimeOnly.Parse("17:00"), TimeOnly.Parse("17:45")) },
        { 10, (TimeOnly.Parse("17:45"), TimeOnly.Parse("18:30")) }
    };

    public ImmutableDictionary<int, (TimeOnly start, TimeOnly end)> Lessons { get; } = _lessons.ToImmutableDictionary();

    public (int lesson, TimeSpan? time) GetTimeUntilEnd()
    {
        var now = TimeOnly.FromDateTime(
                                        TimeZoneInfo.ConvertTime(DateTime.Now.Add(TimeSpan.FromSeconds(11)),
                                                                 TimeZoneInfo
                                                                     .FindSystemTimeZoneById("Russian Standard Time")
                                                                )
                                       );
        var (currentLesson, (currentStart, currentEnd)) = Lessons.FirstOrDefault(x =>
        {
            var (lesson, (start, end)) = x;

            return now.IsBetween(start, end);
        });

        if (currentStart == default)
        {
            return (default, null);
        }

        return (currentLesson, currentEnd - now);
    }
}
