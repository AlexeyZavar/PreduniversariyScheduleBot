#region

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkBot.Forms;
using MarkBot.LoggerSinks;
using MarkBot.Repositories;
using MarkBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Telegram.Bot.Types;
using TelegramBotBase.Builder;

#endregion

namespace MarkBot;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        await Host
              .CreateDefaultBuilder(args)
              .ConfigureAppConfiguration(ConfigureAppConfiguration)
              .ConfigureServices(ConfigureServices)
              .UseSerilog(ConfigureSerilog)
              .RunConsoleAsync();
    }

    private static void ConfigureAppConfiguration(HostBuilderContext host, IConfigurationBuilder builder)
    {
        builder.Sources.Clear();

        builder.AddJsonFile("appsettings.json", false, false);
        builder.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", true, false);
        builder.AddEnvironmentVariables();
    }

    private static void ConfigureSerilog(HostBuilderContext host, IServiceProvider services,
                                         LoggerConfiguration configuration)
    {
        configuration
            .WriteTo.Console()
            .WriteTo.Sink(ActivatorUtilities.CreateInstance<TelegramSink>(services), LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning);

        if (host.HostingEnvironment.IsDevelopment())
        {
            configuration.MinimumLevel.Verbose();
        }
    }

    private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
    {
        services
            .AddConfig()
            .AddBot()
            .AddDatabase()
            .AddSingleton<TimeService>()
            .AddSingleton<ScheduleService>()
            .AddSingleton<MarkUpdater>()
            .AddHostedService<MarkService>()
            .Configure<HostOptions>(option =>
            {
                option.ShutdownTimeout = TimeSpan.FromSeconds(20);
                option.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
            });
    }

    private static IServiceCollection AddConfig(this IServiceCollection services)
    {
        return services.AddSingleton(x =>
        {
            var config = new Config();
            x.GetRequiredService<IConfiguration>().Bind(config);
            return config;
        });
    }

    private static IServiceCollection AddBot(this IServiceCollection services)
    {
        return services.AddSingleton(x =>
        {
            var config = x.GetRequiredService<Config>();
            var bot = BotBaseBuilder.Create()
                                    .WithAPIKey(config.Key)
                                    .DefaultMessageLoop()
                                    .WithStartForm<MainForm>()
                                    .NoProxy()
                                    .CustomCommands(commands =>
                                    {
                                        commands[BotCommandScope.Default()] = new List<BotCommand>
                                        {
                                            new()
                                            {
                                                Command = "/schedule",
                                                Description = "узнать расписание"
                                            },
                                            new()
                                            {
                                                Command = "/left",
                                                Description = "узнать временя до конца урока"
                                            },
                                            new()
                                            {
                                                Command = "/auth",
                                                Description = "авторизоваться в сбере"
                                            },
                                            new()
                                            {
                                                Command = "/sber",
                                                Description = "узнать оценки и дз"
                                            }
                                        };
                                    })
                                    .NoSerialization()
                                    .DefaultLanguage()
                                    .Build();

            bot.Exception += (sender, eventArgs) => { Log.Error(eventArgs.Error, "Error in TelegramBot"); };
            bot.UploadBotCommands();

            return bot;
        });
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        return services
               .AddDbContext<MarkDatabase>((provider, builder) =>
               {
                   var config = provider.GetRequiredService<Config>();
                   builder.UseNpgsql(config.ConnectionString, optionsBuilder => optionsBuilder.EnableRetryOnFailure());
               })
               .AddScoped<SubscriberRepository>();
    }
}
