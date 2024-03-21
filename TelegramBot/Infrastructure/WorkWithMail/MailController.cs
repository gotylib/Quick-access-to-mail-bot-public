using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Infrastructure.WorkWithMail
{
    public class MailController
    {
        public async Task ReadMessage(string mail, string password, ITelegramBotClient botClient, Message message, int size)
        {
            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.mail.ru", 993, true);
                await client.AuthenticateAsync(mail, password);
                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);
                int messageCount = inbox.Count;
                for (int i = Math.Max(messageCount - size + 1, 1); i <= messageCount; i++) // Читаем последние 'size' сообщений
                {
                    var mimeMessage = await inbox.GetMessageAsync(i);
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"From: {mimeMessage.From} - Subject: {mimeMessage.Subject}");
                }
                await client.DisconnectAsync(true);
            }

        }

    }
}

