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

                int count = 0;
                foreach (var uniqueId in await inbox.SearchAsync(SearchQuery.All))
                {
                    if (++count > size)
                        break;

                    var mimeMessage = await inbox.GetMessageAsync(uniqueId);
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"From: {mimeMessage.From} - Subject: {mimeMessage.Subject}");
                }

                await client.DisconnectAsync(true);
            }
        }
    }
}

