using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MimeKit.Tnef;
using TelegramBot.Core;
using TelegramBot.Infrastructure.DateBase;
using TelegramBot.Infrastructure.WorkWithMail;

namespace TelegramBot
{
    public class Bot
    {
        public static ITelegramBotClient bot;
        public static Dictionary<long, bool> isCreatingMessage = new Dictionary<long, bool>();
        public static Dictionary<long, bool> isReadingMessage = new Dictionary<long, bool>();
        public static Dictionary<long, string> loginInProgress = new Dictionary<long, string>();
        public static ApplicationContext db = new ApplicationContext();
        public static DBController controller = new DBController();
        public static MailController mailController = new MailController();  

        public static async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                
                var message = update.Message;

                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Привет, я телеграмм бот, который поможет перенести тебе твою mail.ru почту в телеграмм");
                    return;
                }
                if (message.Text.ToLower() == "/delete")
                {
                    var login = loginInProgress[message.Chat.Id];
                    var password = message.Text;
                    var userId = update.Message.From.Id;
                    MailUser? existingUser = db.Users.FirstOrDefault(u => u.Id == userId);
                    if (existingUser != null)
                    {
                        controller.DeleteUser(ref db, ref existingUser);
                        await botClient.SendTextMessageAsync(message.Chat, "Информация о пользователе удалена.");
                        return;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Такого пользователя нет.");
                        return;
                    }
                }
                if (message.Text.ToLower() == "/readmessage")
                {
                    isReadingMessage[message.Chat.Id] = true;
                    await botClient.SendTextMessageAsync(message.Chat, "Сколько сообщений вам вывести?");
                    return;
                }
                if(message.Text.ToLower() == "/sendmessage")
                {
                    return;
                }
                if (message.Text.ToLower() == "/create")
                {
                    isCreatingMessage[message.Chat.Id] = true;
                    await botClient.SendTextMessageAsync(message.Chat, "Введите имя вашей почты:");
                    return;
                    
                }

                if (isCreatingMessage.ContainsKey(message.Chat.Id) && isCreatingMessage[message.Chat.Id])
                {
                    if (!loginInProgress.ContainsKey(message.Chat.Id))
                    {
                        loginInProgress[message.Chat.Id] = message.Text;
                        await botClient.SendTextMessageAsync(message.Chat, "Теперь введите пароль:");
                        return;
                    }
                    else
                    {
                        var login = loginInProgress[message.Chat.Id];
                        var password = message.Text;
                        var userId = update.Message.From.Id;
                        MailUser? existingUser = db.Users.FirstOrDefault(u => u.Id == userId);

                        if (existingUser != null)
                        {
                            // Обновление информации о пользователе
                            controller.UpdateUser(ref db, ref existingUser, ref login, ref password);
                            await botClient.SendTextMessageAsync(message.Chat, "Информация о пользователе обновлена.");
                            
                        }
                        else
                        {
                            // Добавление нового пользователя
                            controller.CreateUser(ref db, ref userId, ref login, ref password);
                            await botClient.SendTextMessageAsync(message.Chat, "Новый пользователь добавлен.");

                        }

                        // Сброс переменных состояния
                        isCreatingMessage[message.Chat.Id] = false;
                        loginInProgress.Remove(message.Chat.Id);

                        await botClient.SendTextMessageAsync(message.Chat, "Логин и пароль приняты.");
                        return;
                    }
                }
                if (isReadingMessage.ContainsKey(message.Chat.Id) && isReadingMessage[message.Chat.Id] )
                {
                    int score = int.Parse(message.Text);
                    var userId = update.Message.From.Id;
                    MailUser existingUser = db.Users.FirstOrDefault(u => u.Id == userId);
                    isReadingMessage[message.Chat.Id] = false;
                    if (existingUser != null)
                    {
                        // чтение сообщений пользователя
                        var mail = existingUser.mail;
                        var password = existingUser.password;
                        mailController.ReadMessage(existingUser.mail, existingUser.password, botClient, message, score);
                        return;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Такого пользователя нет.");
                        return;
                    }

                }

            }
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

    }

}
