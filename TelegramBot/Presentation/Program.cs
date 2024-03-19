using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot.bot = new TelegramBotClient(Console.ReadLine());

            Console.WriteLine("Запущен бот " + Bot.bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message } // Пример: получение только обновлений с типом Message
            };


            Bot.bot.StartReceiving(
                Bot.HandlerUpdateAsync,
                Bot.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}
