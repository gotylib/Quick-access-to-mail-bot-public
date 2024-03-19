using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Infrastructure.WorkWithMail
{
    public class MailController
    {
        async public void ReadMessage(string mail, string password, ITelegramBotClient botClient, Message? message, int size)
        {
            using (var client = new ImapClient())
            {
                client.Connect("imap.mail.ru", 993, true);
                client.Authenticate(mail, password);
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                int score = 0;
                foreach (var uid in inbox.Search(SearchQuery.All))
                {
                    score++;
                    if (score == size)
                    {
                        break;
                    }
                    var messages = inbox.GetMessage(uid);
                    await botClient.SendTextMessageAsync(message.Chat, $"From: {messages.From} - Subject: {messages.Subject}");
                }
                client.Disconnect(true);
            }
        }

    }
}
