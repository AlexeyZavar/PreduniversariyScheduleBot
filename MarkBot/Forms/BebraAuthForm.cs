#region

using System.Text;
using System.Threading.Tasks;
using MarkBot.Repositories;
using MarkBot.SberClass;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

#endregion

namespace MarkBot.Forms;

public class BebraAuthForm : AutoCleanForm
{
    private readonly BebraClassClient _client = new();

    public string? Username { get; set; }
    public string? Password { get; set; }

    public override async Task Load(MessageResult message)
    {
        switch (message.BotCommand)
        {
            case "/back":
                await NavigationController.PopAsync();

                return;
        }

        if (!string.IsNullOrWhiteSpace(message.MessageText) && !message.IsBotCommand)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                Username = message.MessageText;
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                Password = message.MessageText;
            }
        }

        if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
        {
            await Device.Send("*⏳ Авторизовываюсь...*");
            await TryAuthorize();
        }
        else
        {
            var sb = new StringBuilder();

            sb.AppendLine($"*🔗 Логин*: `{Username}`");
            sb.AppendLine($"*🔗 Пароль*: `{Password}`");
            sb.AppendLine();
            sb.AppendLine("_🕊 Отправь мне логин и пароль двумя сообщениями._");
            sb.AppendLine("💼 Отменить авторизацию - */back*.");
            sb.AppendLine();
            sb.AppendLine("🤯 *А ты не украдёшь мою мору?!*");
            sb.AppendLine(
                          "_Нет. Я не оставляю данные от аккаунта в БД, а лишь 2 токена для авторизации. После смены пароля токены сбрасываются._");

            await Device.Send(sb.ToString());
        }
    }

    public async Task TryAuthorize()
    {
        var res = await _client.Authorize(Username, Password);
        if (!res)
        {
            Username = null;
            Password = null;
            await Device.Send("*❌ Неверный логин или пароль.*");
            return;
        }

        await Device.Send("*✅ Успешная авторизация!*");

        using var scope = MainForm.ServiceProvider.CreateScope();
        var subscriberRepository = scope.ServiceProvider.GetRequiredService<SubscriberRepository>();
        await subscriberRepository.SetBebraClassData(Device.DeviceId, Username, Password);

        await Task.Delay(2000);

        await NavigationController.PopAsync();
    }
}
