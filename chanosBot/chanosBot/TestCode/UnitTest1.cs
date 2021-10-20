using chanosBot.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace TestCode
{
    [TestClass]
    public class LogicTest
    {
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
    } 
}
