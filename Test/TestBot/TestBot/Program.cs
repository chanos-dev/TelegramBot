using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace TestBot
{
    class Program
    {        
        static void Main(string[] args)
        {
            var bot = new TelegramBot();

            while (true)
            {                
                var command = Console.ReadLine();

                if (command.ToUpper() == "EXIT")
                    break;
            }
        }
    }

    internal class TelegramBot
    {
        private TelegramBotClient Bot;

        private string Token { get; set; }

        public TelegramBot()
        {
            Token = "key";

            Bot = new TelegramBotClient(Token);

            StartTelegramPolling(); 
        } 

        private async void StartTelegramPolling()
        {
            var me = await Bot.GetMeAsync();
            Console.WriteLine($"Start {me.FirstName}");

            Bot.OnMessage += Bot_OnMessage;
            Bot.StartReceiving();
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message is null ||
                message.Type != MessageType.Text ||
                !message.Text.StartsWith("/"))
                return;

            // limit message length -> 4096
            //await Bot.SendTextMessageAsync(message.Chat.Id, string.Join("", Enumerable.Repeat("0", 4096)));

            Console.WriteLine($"{message.From.FirstName}{message.From.LastName} : {message.Text}");

            var commandItems = message.Text.Split(' ');

            if (message.Text.StartsWith("/로또"))
            {
                int loop = 1;

                if (commandItems.Count() > 1)
                {
                    if (!int.TryParse(commandItems[1], out loop))
                    {
                        loop = 1;
                    }
                }

                var msg = Lotto(loop);

                if (msg.Length > 4096)
                    await Bot.SendTextMessageAsync(message.Chat.Id, "메세지 길이 제한이 있습니다.");
                else
                    await Bot.SendTextMessageAsync(message.Chat.Id, Lotto(loop));
            }
            else
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "없는 명령어 입니다.");
            }
        }

        private string Lotto(int loop)
        {
            var sb = new StringBuilder();

            var lotto = new HashSet<int>();
            var ran = new Random();

            for (int idx = 0; idx < loop; idx++)
            {
                while (lotto.Count < 6)
                {
                    lotto.Add(ran.Next(1, 46));
                }

                sb.AppendLine($"{idx + 1} : {string.Join(", ", lotto.OrderBy(l => l))}");
                lotto.Clear();
            }

            return sb.ToString();
        }
    }
}
