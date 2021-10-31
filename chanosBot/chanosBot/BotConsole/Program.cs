using chanosBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotConsole
{
    class Program
    {
        static void Main(string[] args)
        {            
            var TelegramBot = new TelegramBot("key");

            TelegramBot.Run();

            Console.ReadKey();
        }
    }
}
