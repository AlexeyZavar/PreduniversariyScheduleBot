#region

using System.Text.Json;
using MarkBot.Parsers.ParserUtils;

#endregion

namespace MarkBot.Schedule;

public static class ScheduleParser
{
    private static ScheduleData[] _schedule = null!;
    private static readonly SortedDictionary<string, ParsedDay[]> ParsedSchedule = new();
    private static readonly Dictionary<string, string[]> ParsedScheduleByGroups = new();

    public static (string?, ParsedDay[]?, string[]?) GetSchedule(string name)
    {
        name = name.Replace("ё", "е");
        var (firstname, middlename, lastname) = NameParser.Parse(name.Split());
        var res = ParsedSchedule.FirstOrDefault(x =>
        {
            var (key, schedule) = x;

            if (key.Contains(name, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (firstname != null && key.Contains(firstname, StringComparison.InvariantCultureIgnoreCase) &&
                lastname != null && key.Contains(lastname, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        });

        if (string.IsNullOrWhiteSpace(res.Key))
        {
            return (default, default, default);
        }

        return (res.Key, res.Value, ParsedScheduleByGroups[res.Key]);
    }

    public static async Task ParseSchedule()
    {
        await Task.Yield();

        await using var f = File.OpenRead("./Schedule.json");
        var data = await JsonSerializer.DeserializeAsync<ScheduleData[]>(f);

        await using var f2 = File.OpenRead("./NameMappings.json");
        var nameMappings = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(f2);

        _schedule = data!;

        var teachers = new Dictionary<string, Dictionary<string, HashSet<ParsedLesson>>>();
        var teachersGroups = new Dictionary<string, HashSet<string>>();
        foreach (var scheduleData in _schedule)
        {
            var studentName = scheduleData.Name.Replace("ё", "е");
            var parsedLessons = scheduleData.Data.Select(x => x.Day).ToHashSet()
                                            .ToDictionary(x => x, _ => new List<ParsedLesson>());

            foreach (var lesson in scheduleData.Data)
            {
                var teacher = !string.IsNullOrWhiteSpace(lesson.Teacher) ? lesson.Teacher.Split()[0] : null;
                var parsedLesson = new ParsedLesson
                {
                    Name = lesson.Subject,
                    Hour = lesson.Hour,
                    Teacher = teacher,
                    Room = lesson.Venue.ToString()
                };
                parsedLessons[lesson.Day].Add(parsedLesson);

                if (!string.IsNullOrWhiteSpace(teacher))
                {
                    foreach (var teacher2 in lesson.Teacher.Replace("/", ",")
                                                   .Split(',',
                                                          StringSplitOptions.RemoveEmptyEntries |
                                                          StringSplitOptions.TrimEntries))
                    {
                        var teacherMapped = nameMappings[teacher2];
                        if (!teachers.ContainsKey(teacherMapped))
                        {
                            teachers[teacherMapped] = new Dictionary<string, HashSet<ParsedLesson>>();
                        }

                        if (!teachers[teacherMapped].ContainsKey(lesson.Day))
                        {
                            teachers[teacherMapped][lesson.Day] = new HashSet<ParsedLesson>();
                        }

                        teachers[teacherMapped][lesson.Day].Add(parsedLesson);

                        if (!teachersGroups.ContainsKey(teacherMapped))
                        {
                            teachersGroups[teacherMapped] = new HashSet<string>();
                        }

                        teachersGroups[teacherMapped].UnionWith(scheduleData.Groups);
                    }
                }
            }

            ParsedSchedule[studentName] = parsedLessons.Select(x => new ParsedDay
            {
                Name = x.Key,
                Lessons = x.Value.OrderBy(x => x.Hour).ToArray()
            }).ToArray();
            ParsedScheduleByGroups[studentName] = scheduleData.Groups;
        }

        foreach (var (name, lessons) in teachers)
        {
            ParsedSchedule[name] = lessons.Select(x => new ParsedDay
            {
                Name = x.Key,
                Lessons = x.Value.OrderBy(x => x.Hour).ToArray()
            }).OrderBy(x => x.Name switch
                            {
                                "Понедельник" => 1,
                                "Вторник"     => 2,
                                "Среда"       => 3,
                                "Четверг"     => 4,
                                "Пятница"     => 5,
                                "Суббота"     => 6,
                                _             => throw new ArgumentOutOfRangeException(nameof(data), x.Name)
                            }).ToArray();
            ParsedScheduleByGroups[name] = teachersGroups[name].OrderBy(x => x).ToArray();
        }
    }
}
