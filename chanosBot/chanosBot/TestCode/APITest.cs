using chanosBot.Actions;
using chanosBot.API;
using chanosBot.Converter;
using chanosBot.Core;
using chanosBot.Interface;
using chanosBot.Model;
using chanosBot.Model.Delivery;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [TestMethod]
        public void GetDeliveryTracking()
        {
            var api = new DeliveryAPI();
            api.SetAPIKey("key");

            var response = api.GetTextDeliveryTracking("04", "").Result;

            Console.WriteLine();
        }

        [TestMethod]
        public void GetInternalListFromHtml()
        {
            var htmlClient = new WebClient(); 
            htmlClient.Encoding = System.Text.Encoding.UTF8; 
            var html = htmlClient.DownloadString("https://info.sweettracker.co.kr/v2/api-docs");

            var roots = new[]
            {
                "info",
                "description",
            };

            var info = JsonConvert.DeserializeObject<string>(html, new SingleValueJsonConverter(roots));

            var doc = new HtmlDocument();
            doc.LoadHtml(info);
            var node = doc.DocumentNode.SelectSingleNode("//table[@class='table table-bordered']/tbody");

            var findNodes = node.ChildNodes.Where(n => n.Name == "tr").SelectMany(n =>
            {
                var tdList = n.ChildNodes.Where(c => c.Name == "td").ToList();

                List<string> str = new List<string>();

                for (int idx = 0; idx < tdList.Count; idx += 2)
                { 
                    str.Add($"{tdList[idx].InnerText} {tdList[idx + 1].InnerText}");
                }

                return str;
            });

        }
    } 
}