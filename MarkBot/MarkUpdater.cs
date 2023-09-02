#region

using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MarkBot.Parsers.Entities;
using MarkBot.Schedule;
using Microsoft.Extensions.Hosting;
using TelegramBotBase;

#endregion

namespace MarkBot;

public sealed class MarkUpdater
{
    private readonly BotBase _bot;
    private readonly Config _config;
    private readonly IHostEnvironment _env;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve
    };

    private readonly IServiceProvider _services;

    public MarkUpdater(BotBase bot, Config config, IHostEnvironment env, IServiceProvider services)
    {
        _bot = bot;
        _config = config;
        _env = env;
        _services = services;
    }

    public async Task Run(CancellationToken stoppingToken)
    {
        var t = ScheduleParser.ParseSchedule();
        await t;

        // HashSet<Student> previous = null!;
        //
        // if (File.Exists("previous.json") && _env.IsProduction())
        // {
        //     var f = File.OpenRead("previous.json");
        //     if (f.Length > 4)
        //     {
        //         previous =
        //             await JsonSerializer.DeserializeAsync<HashSet<Student>>(f, _serializerOptions, stoppingToken) ??
        //             throw new InvalidOperationException();
        //         MainParser.Students = previous;
        //     }
        //
        //     await f.DisposeAsync();
        // }
        // else
        // {
        //     await Download();
        // }
        //
        // await t;
        //
        //
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     var diffs = MainParser.FindDifferences(students, previous);
        //
        //     if (diffs.Count != 0)
        //     {
        //         Log.Verbose("Something changed!");
        //         using var scope = _services.CreateScope();
        //         var subscriberRepository = scope.ServiceProvider.GetRequiredService<SubscriberRepository>();
        //
        //         foreach (var diff in diffs)
        //             try
        //             {
        //                 var sub = await subscriberRepository.FindByName(diff.Student.FirstName,
        //                     diff.Student.LastName);
        //
        //                 if (sub == null) continue;
        //
        //                 var msg = GenerateMessage(diff);
        //
        //                 try
        //                 {
        //                     await _bot.Client.TelegramClient.SendTextMessageAsync(new ChatId(sub.Id), msg,
        //                         ParseMode.Markdown, cancellationToken: stoppingToken);
        //                 }
        //                 catch (Exception e)
        //                 {
        //                     Log.Error(e, "Failed to to send message to {Id} ({Name})", sub.Id, sub.Name);
        //                 }
        //             }
        //             catch (Exception e)
        //             {
        //                 Log.Error(e, "???");
        //             }
        //     }
        //
        //     await Task.Delay(_config.UpdateDelay, stoppingToken);
        //
        //     var f = File.Open("previous.json", FileMode.Create);
        //     await JsonSerializer.SerializeAsync(f, students, _serializerOptions, stoppingToken);
        //     await f.DisposeAsync();
        //
        //     previous = students;
        // }
    }

    private static string GenerateMessage(StudentDifference diff)
    {
        var sb = new StringBuilder();

        foreach (var mark in diff.MarksAdd)
        {
            sb.AppendLine($"`[+]` {mark.Subject}: *{mark.Value}* (`{mark.Description}`)");
        }

        if (diff.MarksAdd.Count != 0)
        {
            sb.AppendLine();
        }

        foreach (var mark in diff.MarksRemove)
        {
            sb.AppendLine($"`[-]` {mark.Subject}: *{mark.Value}* (`{mark.Description}`)");
        }

        if (diff.MarksRemove.Count != 0)
        {
            sb.AppendLine();
        }

        foreach (var mark in diff.MarksChange)
        {
            sb.Append($"`[*]` {mark.Updated.Subject}: ");

            if (mark.Outdated.Value != mark.Updated.Value)
            {
                sb.Append($"*{mark.Outdated.Value} → {mark.Updated.Value}*");
            }
            else
            {
                sb.Append($"*{mark.Updated.Value}*");
            }

            sb.Append(' ');

            if (!mark.Outdated.Description.Equals(mark.Updated.Description, StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine($"(`{mark.Outdated.Description}` → `{mark.Updated.Description}`)");
            }
            else
            {
                sb.AppendLine($"(`{mark.Updated.Description}`)");
            }
        }

        var marksPart = sb.ToString();
        sb.Clear();

        var violationsPart = sb.ToString();

        if (!string.IsNullOrWhiteSpace(marksPart) && !string.IsNullOrWhiteSpace(violationsPart))
        {
            return $"{marksPart}{Environment.NewLine}{violationsPart}";
        }

        if (!string.IsNullOrWhiteSpace(marksPart))
        {
            return marksPart;
        }

        if (!string.IsNullOrWhiteSpace(violationsPart))
        {
            return violationsPart;
        }

        return "Нет изменений. Если ты видишь это сообщение, то что-то пошло не так >_<";
    }
}
