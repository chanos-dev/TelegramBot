using chanosBot.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestCode
{
    [TestClass]
    public class BotTest
    { 
        [TestMethod]
        public void WeatherActionExecute()
        {
            var weatherAction = new WeatherAction();

            var input = "/날씨 성남시 중원구 /자동설정 2433";

            weatherAction.Execute(input.Split(' '));
        }
    }
}
