#region

using System;
using System.Text;
using System.Threading.Tasks;
using MarkBot.Parsers.ParserUtils;
using MarkBot.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotBase;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

#endregion

namespace MarkBot.Forms;

public class AdminForm : AutoCleanForm
{
    /// <inheritdoc />
    public override async Task Load(MessageResult message)
    {
        switch (message.BotCommand)
        {
            case "/back":
                await NavigationController.PopAsync();

                break;
            case "/advertisement":
                await Advertisement(message.MessageText.Replace("/advertisement ", ""));
                await Device.Send(
                                  "*Рассылка завершена ~*");

                break;
            case "/who":
                await Device.Send(
                                  $"[Посмотрим, что это за покемон такой ~](tg://user?id={message.BotCommandParameters[0]})");

                break;
            case "/users":
                using (var scope = MainForm.ServiceProvider.CreateScope())
                {
                    var subscriberRepository = scope.ServiceProvider.GetRequiredService<SubscriberRepository>();

                    var subs = await subscriberRepository.Fetch();
                    var sb = new StringBuilder();

                    sb.AppendLine("*Список пользователей*:");
                    sb.AppendLine();

                    foreach (var sub in subs)
                    {
                        sb.AppendLine($"`{sub.Id,-11}`: _{sub.Name}_ ([тык](tg://user?id={sub.Id}))");
                    }

                    await Device.Send(sb.ToString());
                }

                break;
            case "/remove":
                using (var scope = MainForm.ServiceProvider.CreateScope())
                {
                    var subscriberRepository = scope.ServiceProvider.GetRequiredService<SubscriberRepository>();

                    await subscriberRepository.Remove(long.Parse(message.BotCommandParameters[0]));
                }

                await Device.Send($"*Я удалила пользователя с ID* `{message.BotCommandParameters[0]}`");

                break;
        }
    }

    /// <inheritdoc />
    public override async Task Render(MessageResult message)
    {
        var sb = new StringBuilder();

        sb.AppendLine("*Команды*:");
        sb.AppendLine();
        sb.AppendLine("• */back* - _вернуться в меню_");
        sb.AppendLine("• */marks <ИМЯ>* - _актуальная версия таблицы с оценками_");
        sb.AppendLine("• */advertisement <ТЕКСТ>* - _оповещение пользователям_");
        sb.AppendLine("• */users* - _все пользователи бота_");
        sb.AppendLine("• */who <ID>* - _выслать ссылку на пользователя_");
        sb.AppendLine("• */remove <ID>* - _удалить пользователя из БД_");
        sb.AppendLine();
        sb.AppendLine("*Закодил и поддерживал бота весь год - @alexeyzavar, при поддержке REDACTED и других!*");

        await Device.Send(sb.ToString());
    }

    private static async Task Advertisement(string adv)
    {
        using var scope = MainForm.ServiceProvider.CreateScope();
        var bot = scope.ServiceProvider.GetRequiredService<BotBase>();
        var subscriberRepository = scope.ServiceProvider.GetRequiredService<SubscriberRepository>();

        var subs = await subscriberRepository.Fetch();

        foreach (var sub in subs)
        {
            try
            {
                var (first, _, _) = NameParser.Parse(sub.Name.Split());
                var s = adv.Replace("%ИМЯ%", first);

                await bot.Client.TelegramClient.SendTextMessageAsync(new ChatId(sub.Id), s, ParseMode.Markdown);
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to send adv. :(");
            }
        }
    }
}
