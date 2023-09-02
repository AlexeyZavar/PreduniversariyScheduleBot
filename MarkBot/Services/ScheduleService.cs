#region

using System.Linq;
using System.Text;
using MarkBot.Schedule;

#endregion

namespace MarkBot.Services;

public sealed class ScheduleService
{
    public static (string? name, string? shedule) FindByPartialName(string req, bool zavarFriendly)
    {
        var (foundName, schedule, groups) = ScheduleParser.GetSchedule(req);

        if (string.IsNullOrWhiteSpace(foundName))
        {
            return (null, null);
        }

        var sb2 = new StringBuilder();

        sb2.AppendLine($"Расписание по запросу *{req}* (*{foundName}*)");
        sb2.AppendLine();

        foreach (var day in schedule!)
        {
            sb2.AppendLine($"*{day.Name}*");

            var prev = 0;
            foreach (var lesson in day.Lessons)
            {
                while (prev + 1 != lesson.Hour)
                {
                    ++prev;
                    sb2.AppendLine($"{prev}. Окно (_коворка_)");
                }

                sb2.AppendLine($"{lesson.Hour}. {lesson.Name} (_{lesson.Room ?? "-"}_)");

                ++prev;
            }

            sb2.AppendLine();
        }

        if (zavarFriendly)
        {
            sb2.Append("Группы: ");
            sb2.AppendLine(string.Join(";` `", groups!.OrderBy(x => x).Select(x => $"`{x}`")));
        }

        return (foundName, sb2.ToString());
    }
}
