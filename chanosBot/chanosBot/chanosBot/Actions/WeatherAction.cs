using chanosBot.Interface;
using chanosBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace chanosBot.Actions
{
    internal class WeatherAction : ICommand
    {
        public string CommandName => "/날씨";

        public string Execute(params string[] options)
        {
            if (options.Length > 0)
                throw new ArgumentException($"Too many options.\ne.g) {CommandName}");

            var content = GetContent();
            content.Wait();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(content.Result);

            var node = xml.DocumentElement.SelectSingleNode("descendant::body");

            var weathers = new List<Weather>();

            foreach (XmlElement childNode in node.ChildNodes)
            {
                weathers.Add(Weather.CreateInstance(childNode));
            }

            return string.Join("\n", weathers);
        }

        // todo: refactoring
        private async Task<string> GetContent()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(@"http://www.kma.go.kr/wid/queryDFSRSS.jsp?zone=4161025000");

            if (!response.IsSuccessStatusCode)
                return "날씨 응답 실패";

            return await response.Content.ReadAsStringAsync();
        }

        public override string ToString()
        {
            return $"{CommandName}";
        }
    }
}
