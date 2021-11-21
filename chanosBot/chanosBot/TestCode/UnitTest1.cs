using chanosBot.Common;
using chanosBot.Crypto;
using chanosBot.Model;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestCode
{
    [TestClass]
    public class LogicTest
    {
        [TestMethod]
        public void CompareTimeSpan()
        {
            var source = new TimeSpan(19, 11, 0);
            var target = DateTime.Now.TimeOfDay;



            Assert.IsTrue(StructHelper.CompareTimeSpanHourMinutePair(source, target));
        }

        [TestMethod]
        public void GetWeather()
        {
            var foo = GetWeatherXml();

            Console.WriteLine(foo.Result);
        }

        public async Task<string> GetWeatherXml()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(@"http://www.kma.go.kr/wid/queryDFSRSS.jsp?zone=4161025000");

            if (!response.IsSuccessStatusCode)
                Assert.Fail("응답 실패");

            string content = await response.Content.ReadAsStringAsync();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(content);
            
            var node = xml.DocumentElement.SelectSingleNode("descendant::body"); 
            
            var weathers = new List<Weather>();

            foreach (XmlElement childNode in node.ChildNodes)
            {
                weathers.Add(Weather.CreateInstance(childNode));
            }

            return string.Join("\n", weathers); 
        }

        [TestMethod]
        public void GetWeatherWebParsing()
        {
            var location = "성남시";

            var url = $"https://search.naver.com/search.naver?query={location} 날씨";

            var web = new HtmlWeb();
            HtmlDocument htmlDocument = web.Load(url);

            var node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='status_wrap']");

            Console.WriteLine(GetNode1(node.SelectSingleNode("//div[@class='weather_graphic']")));
            Console.WriteLine(GetNode4(node.SelectSingleNode("//ul[@class='week_list']")));
            Console.WriteLine(GetNode2(node.SelectSingleNode("//div[@class='temperature_info']"))); 
            Console.WriteLine(GetNode3(node.SelectSingleNode("//div[@class='report_card_wrap']")));
        }

        private string GetNode1(HtmlNode node)
        {
            if (node is null)
                return string.Empty;

            var sb = new StringBuilder();

            var nodeItems = node.InnerText.Split(' ').Where(item => !string.IsNullOrEmpty(item));

            sb.AppendLine(string.Join(" ", nodeItems.Skip(1)));
            sb.Append($"날씨 {nodeItems.Take(1).First()}");

            return sb.ToString();
        }

        private string GetNode2(HtmlNode node)
        {
            if (node is null)
                return string.Empty;

            var sb = new StringBuilder();

            // 어제보다 2° 높아요  맑음   강수확률 0% 습도 78% 바람(남서풍) 0m/s   
            var nodeItems = node.InnerText.Split(' ').Where(item => !string.IsNullOrEmpty(item));

            try
            {
                sb.AppendLine(string.Join(" ", nodeItems.Take(3)));

                nodeItems = nodeItems.Skip(4);

                for (int idx = 0; idx < nodeItems.Count(); idx += 2)
                    sb.AppendLine(string.Join(" : ", nodeItems.Skip(idx).Take(2)));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return sb.ToString();
        }

        private string GetNode3(HtmlNode node)
        {
            if (node is null)
                return string.Empty;

            var sb = new StringBuilder();

            // 미세먼지 보통     초미세먼지 보통     자외선 보통     일몰 17:44    
            var nodeItems = node.InnerText.Split(' ').Where(item => !string.IsNullOrEmpty(item));

            try
            {
                for (int idx = 0; idx < nodeItems.Count(); idx += 2)
                    sb.AppendLine(string.Join(" : ", nodeItems.Skip(idx).Take(2)));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return sb.ToString();
        }

        private string GetNode4(HtmlNode node)
        {
            if (node is null)
                return string.Empty;

            var sb = new StringBuilder();

            // 미세먼지 보통     초미세먼지 보통     자외선 보통     일몰 17:44    
            var nodeItems = node.InnerText.Split(' ').Where(item => !string.IsNullOrEmpty(item));

            if (nodeItems.Count() < 1)
                return string.Empty;

            nodeItems = nodeItems.Skip(2).Select(item =>
            {
                if (item == "오전" || item == "오후")
                    return $"{item} 강수확률";

                return item;
            }); 

            try
            {
                for (int idx = 0; idx < nodeItems.Count(); idx += 3)
                {
                    if (nodeItems.Skip(idx).First() is string value)
                    {
                        if (value == "내일")
                            break;

                        sb.AppendLine(string.Join(" ", nodeItems.Skip(idx).Take(3)));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return sb.ToString();
        }

        [TestMethod]
        public void Crypto()
        {
            var aes = new AESCrypto();
            var key = aes.CreateKey("01234567890123456789012345678901");

            Console.WriteLine(key);

            var encData = aes.Encrypt(key, "hello, world");

            Console.WriteLine(encData);
            Console.WriteLine(aes.Decrypt(key, encData));
        }
    } 
}
