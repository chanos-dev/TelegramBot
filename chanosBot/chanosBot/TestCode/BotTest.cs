using chanosBot.Actions;
using chanosBot.Core;
using chanosBot.Interface;
using chanosBot.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TestCode
{
    [TestClass]
    public class BotTest
    {
        private TelegramBotClient Bot { get; set; }
        [TestMethod]
        public void MyTestMethod()
        {
            Bot = new TelegramBotClient("key");
            
            Bot.OnMessage += Bot_OnMessage;

            Bot.OnUpdate += Bot_OnUpdate;

            Bot.OnInlineQuery += Bot_OnInlineQuery;

            Bot.OnCallbackQuery += Bot_OnCallbackQuery;

            Bot.StartReceiving();

            while(true)
            {

            }
        }

        private async void Bot_OnInlineQuery(object sender, Telegram.Bot.Args.InlineQueryEventArgs e)
        {
            var aa = e.InlineQuery.Id;
        }

        private async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            await Bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "empty");
        }

        private async void Bot_OnUpdate(object sender, Telegram.Bot.Args.UpdateEventArgs e)
        {
            var update = e.Update;

            // 일반 : type -> Message
            // 버튼 클릭 : type -> CallbackQuery
            if (update.CallbackQuery != null)
            {
                await Bot.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Data);

                await Bot.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            }

        }

        private async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] // first row
                {
                    new InlineKeyboardButton { Text = "테스트", CallbackData = "테스트1" },
                },
                new[] // second row
                {
                    new InlineKeyboardButton { Text = "Hello", CallbackData = "World" },
                }
            });

            await Bot.SendTextMessageAsync(message.Chat.Id, "버튼", replyMarkup: keyboard);
        } 

        [TestMethod]
        public void WeatherActionExecute()
        {
            var weatherAction = new WeatherAction();

            var input = "/날씨 성남시 중원구 /자동설정 0830";

            weatherAction.Execute(input.Split(' '));
        }

        [TestMethod]
        public void CompareAutoCommand()
        {
            AutoCommandHelper helper = new AutoCommandHelper();
            ICommand weather = new WeatherAction();
            ICommand lotto = new LottoAction();


            var command1 = new AutoCommand()
            {
                UserID = 100,
                ChatID = 202110272433,
                Command = weather,
                Time = new Time()
                {
                    Hour = 22,
                    Minute = 30,
                },
            }; 

            var command2 = new AutoCommand()
            {
                UserID = 100,
                ChatID = 202110272433,
                Command = lotto,
                Time = new Time()
                {
                    Hour = 22,
                    Minute = 30,
                },
            };

            var command3 = new AutoCommand()
            {
                UserID = 200,
                ChatID = 202110272433,
                Command = weather,
                Time = new Time()
                {
                    Hour = 22,
                    Minute = 30,
                },
            };

            var command4 = new AutoCommand()
            {
                UserID = 100,
                ChatID = 202110272433,
                Command = weather,
                Time = new Time()
                {
                    Hour = 07,
                    Minute = 30,
                },
            };

            helper.AddAutoCommand(command1);
            helper.AddAutoCommand(command2);
            helper.AddAutoCommand(command3);
            helper.AddAutoCommand(command4);


            Console.WriteLine(weather);
            Console.WriteLine("DONE");
        }
    }
}
