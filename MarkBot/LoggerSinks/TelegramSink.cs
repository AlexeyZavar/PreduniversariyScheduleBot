#region

using System;
using Serilog.Core;
using Serilog.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotBase;

#endregion

namespace MarkBot.LoggerSinks;

public sealed class TelegramSink : ILogEventSink
{
    private readonly BotBase _bot;

    public TelegramSink(BotBase bot)
    {
        _bot = bot;
    }

    /// <inheritdoc />
    public void Emit(LogEvent ev)
    {
        _bot.Client.TelegramClient.SendTextMessageAsync(new ChatId(139303278),
                                                        $"<strong>{ev.RenderMessage()}</strong>{Environment.NewLine}<code>{ev.Exception}</code>",
                                                        ParseMode.Html);
    }
}
