#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkBot.SberClass;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

#endregion

namespace MarkBot.Forms;

public class BebraForm : AutoCleanForm
{
    private static readonly Dictionary<char, string> EmojiMappings = new()
    {
        { '5', "🟢" },
        { '4', "🟢" },
        { '3', "🟡" },
        { '2', "🔴" }
    };

    private readonly BebraClassClient _client;

    public BebraForm(BebraClassClient client)
    {
        _client = client;
    }

    public override async Task Load(MessageResult message)
    {
        switch (message.BotCommand)
        {
            case "/back":
                await NavigationController.PopAsync();

                return;
        }
    }

    public override async Task Render(MessageResult message)
    {
        var sb = new StringBuilder();

        sb.AppendLine("*🎾 СберКласс*");
        sb.AppendLine("💼 Вернуться в главное меню - */back*.");

        var keyboard = new InlineKeyboardMarkup(
                                                InlineKeyboardButton.WithCallbackData("〽️ Оценки", "marks")
                                               );

        await Device.Send(sb.ToString(), keyboard);
    }

    public override async Task Action(MessageResult message)
    {
        var res = await _client.RefreshToken();
        if (!res)
        {
            await Device.Send("*❌ Не удалось авторизоваться.*");
            await NavigationController.PopAsync();
            return;
        }

        switch (message.RawData)
        {
            case "marks":
                var marks = await _client.FetchMarks();

                var sb = new StringBuilder();
                sb.AppendLine("*Последние 12 оценок.*");
                sb.AppendLine("➖➖➖➖➖➖➖➖");
                sb.AppendLine();
                sb.AppendLine();

                foreach (var date in marks.GroupBy(x => new DateOnly(x.Date.Year, x.Date.Month, x.Date.Day))
                                          .TakeLast(12))
                {
                    sb.AppendLine($"_{date.Key:dd.MM}_:");
                    foreach (var subject in date.GroupBy(x => x.Subject))
                    {
                        sb.AppendLine($"`  ` *{subject.Key}*:");
                        foreach (var mark in subject)
                        {
                            sb.AppendLine($"`    ` {EmojiMappings[mark.Value[0]]} {mark.Value} ({mark.Description})");
                        }
                    }

                    sb.AppendLine();
                }

                await Device.Send(sb.ToString());

                break;
        }

        await message.ConfirmAction();
    }
}
