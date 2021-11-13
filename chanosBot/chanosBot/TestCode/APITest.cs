using chanosBot.Actions;
using chanosBot.API;
using chanosBot.Core;
using chanosBot.Interface;
using chanosBot.Model;
using chanosBot.Model.Delivery;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TestCode
{
    [TestClass]
    public class APITest
    {
        [TestMethod]
        public void GetCompanyList()
        {
            var api = new DeliveryAPI();
            api.SetAPIKey("key");

            var response = api.GetDeliveryList().Result;

            var comp = JsonConvert.DeserializeObject<DeliveryCollection>(response.Result);

            Console.WriteLine();
        } 
    }
}
