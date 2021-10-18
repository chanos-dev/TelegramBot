using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

            foreach (XmlElement childNode in node.ChildNodes)
            {
                var hour = childNode.SelectSingleNode("hour").InnerText;
            }

            //foreach (var node in nodes)
            // child node
            //< hour > 3 </ hour >
            //< day > 1 </ day >
            //< temp > 1.0 </ temp >
            //< tmx > 15.0 </ tmx >
            //< tmn > 0.0 </ tmn >
            //< sky > 1 </ sky >
            //< pty > 0 </ pty >
            //< wfKor > 맑음 </ wfKor >
            //< wfEn > Clear </ wfEn >
            //< pop > 0 </ pop >
            //< r12 > 0.0 </ r12 >
            //< s12 > 0.0 </ s12 >
            //< ws > 1.4000000000000001 </ ws >
            //< wd > 2 </ wd >
            //< wdKor > 동 </ wdKor >
            //< wdEn > E </ wdEn >
            //< reh > 80 </ reh >
            //< r06 > 0.0 </ r06 >
            //< s06 > 0.0 </ s06 >

            return "";
        } 
    } 
}
