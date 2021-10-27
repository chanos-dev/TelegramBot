using chanosBot.Actions;
using chanosBot.Core;
using chanosBot.Interface;
using chanosBot.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestCode
{
    [TestClass]
    public class BotTest
    { 
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
