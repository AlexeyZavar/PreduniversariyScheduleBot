#region

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MarkBot.Forms;
using Microsoft.Extensions.Hosting;
using TelegramBotBase;

#endregion

namespace MarkBot;

public sealed class MarkService : BackgroundService
{
    private readonly BotBase _bot;
    private readonly IServiceProvider _services;
    private readonly MarkUpdater _updater;

    public MarkService(BotBase bot, MarkUpdater updater, IServiceProvider services)
    {
        _bot = bot;
        _updater = updater;
        _services = services;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MainForm.ServiceProvider = _services;
        Directory.CreateDirectory("./marks");

        await _bot.Start();
        await _updater.Run(stoppingToken);
    }
}
