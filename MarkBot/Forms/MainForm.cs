#region

using System;
using System.Text;
using System.Threading.Tasks;
using MarkBot.Repositories;
using MarkBot.SberClass;
using MarkBot.Services;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotBase.Base;
using TelegramBotBase.Form;
using TelegramBotBase.Form.Navigation;

#endregion

namespace MarkBot.Forms;

public class MainForm : AutoCleanForm
{
    private readonly Config _config;
    private readonly TimeService _timeService;

    /// <inheritdoc />
    public MainForm()
    {
        _timeService = ServiceProvider.GetRequiredService<TimeService>();
        _config = ServiceProvider.GetRequiredService<Config>();
    }

    public static IServiceProvider ServiceProvider { get; set; } = null!;

    /// <inheritdoc />
    public override async Task Load(MessageResult message)
    {
        // https://genshin-impact.fandom.com/ru/wiki/%D0%A0%D0%B0%D0%B9%D0%B4%D1%8D%D0%BD/%D0%9E%D0%B7%D0%B2%D1%83%D1%87%D0%BA%D0%B0
        using var scope = ServiceProvider.CreateScope();
        var subscriberRepository = scope.ServiceProvider.GetRequiredService<SubscriberRepository>();
        var sub = await subscriberRepository.FindOrCreate(message.DeviceId);

        switch (message.BotCommand)
        {
            case "/start":
                await Device.Send("_Ну и чего ты ждёшь?_ *Отправь ФИ.*");
                await Device.Send("_Держись ближе, и будешь жить._");

                break;
            case "/admin":
                if (!_config.Admins.Contains(message.DeviceId))
                {
                    await Device.Send(
                                      "_Как насчёт спарринга? Если ты снова останешься (невредимым/невредимой), я позволю тебе войти в Инадзуму. Стремление к развитию и совершенствованию в боевых искусствах поистине неудержимо._");
                    break;
                }

                var nc = new NavigationController(this);
                var f = new AdminForm();

                await NavigateTo(nc);
                await nc.PushAsync(f);

                break;
            case "/auth":
                var nc2 = new NavigationController(this);
                var f2 = new BebraAuthForm();

                await NavigateTo(nc2);
                await nc2.PushAsync(f2);

                break;
            case "/sber":
                if (!sub.HasBebraClassAuthorization)
                {
                    await Device.Send("*❌ Ты не авторизован(а)...*");
                    return;
                }

                var client = new BebraClassClient();
                var authRes = await client.Authorize(sub.Username, sub.Password);

                if (!authRes)
                {
                    sub.Username = null;
                    sub.Password = null;

                    await Device.Send("*❌ Не удалось авторизоваться.*");

                    return;
                }

                var nc3 = new NavigationController(this);
                var f3 = new BebraForm(client);

                await NavigateTo(nc3);
                await nc3.PushAsync(f3);

                break;
            case "/left":
                var sb = new StringBuilder();

                sb.AppendLine("Расписание уроков по времени:");
                sb.AppendLine();
                sb.AppendLine("```");
                foreach (var (lesson, (start, end)) in _timeService.Lessons)
                {
                    sb.AppendLine($"{lesson}. {start:HH\\:mm} - {end:HH\\:mm}");
                }

                sb.AppendLine("```");

                var (currentLesson, time) = _timeService.GetTimeUntilEnd();

                if (time is not { } t)
                {
                    sb.AppendLine("Сейчас нет урока.");
                }
                else
                {
                    sb.AppendLine($"До конца *{currentLesson}* урока *{t.Minutes} мин.* и *~{t.Seconds} сек.*");
                }

                sb.AppendLine();
                sb.AppendLine("_Смертный мир... мимолетный сон..._");

                await Device.Send(sb.ToString());

                break;
            case "/schedule":
                var name = message.BotCommandParameters.Count != 0
                               ? string.Join(' ', message.BotCommandParameters)
                               : sub.Name;

                if (string.IsNullOrWhiteSpace(name))
                {
                    await Device.Send("_Я не нашла такого человека..._");
                    return;
                }

                var (foundName, schedule) = ScheduleService.FindByPartialName(name, sub.ZavarFriendly);

                if (string.IsNullOrWhiteSpace(foundName))
                {
                    await Device.Send("_Я не нашла такого человека..._");
                    return;
                }

                await Device.Send(schedule);

                break;
            default:
                if (message.BotCommand == null && message.MessageText.Contains(' '))
                {
                    await subscriberRepository.ChangeName(message.DeviceId, message.MessageText);
                }

                break;
        }
    }

    /// <inheritdoc />
    public override async Task Render(MessageResult message)
    {
        using var scope = ServiceProvider.CreateScope();
        var subscriberRepository = scope.ServiceProvider.GetRequiredService<SubscriberRepository>();
        var sub = await subscriberRepository.FindOrCreate(message.DeviceId);
        var name = sub.Name ?? "неизвестно кто";

        var authText = sub.HasBebraClassAuthorization ? "авторизован(а)" : "не авторизован(а)";

        var sb = new StringBuilder();

        sb.AppendLine($"👤 *{name}*");
        sb.AppendLine($"🔗 Статус авторизации в СберКлассе: *{authText}*");
        sb.AppendLine();
        sb.AppendLine("*Команды*:");
        sb.AppendLine("• */schedule <ФАМИЛИЯ_ИМЯ>* - _узнать расписание ученика или учителя_");
        sb.AppendLine("• */left* - _узнать, сколько времени осталось до конца урока_");
        sb.AppendLine("• */auth* - _авторизация в СберКлассе_");
        sb.AppendLine("• */sber* - _просмотр оценок и домашки (early alpha beta preview версия)_");
        sb.AppendLine();
        sb.AppendLine("*Бот для отслеживания олимпиад: @olimpiadaparser_bot*");
        sb.AppendLine();
        sb.AppendLine(
                      "📞 _Связь с сёгуном - @alexeyzavar._ *Информация о расписании представлена чисто в ознакомительных целях, ответственность за прогул уроков не несу :>*");

        await Device.Send(sb.ToString());
    }
}
