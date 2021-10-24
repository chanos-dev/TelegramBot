using chanosBot.Interface;
using chanosBot.Model;
using HtmlAgilityPack;
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
        private string WeatherURL => "https://search.naver.com/search.naver?query=";

        public string CommandName => "/날씨";

        public string Execute(params string[] options)
        {
            if (options.Length == 0)
                throw new ArgumentException($"지역명이 없습니다.\n예) {this.ToString()}");

            var location = string.Join(" ", options);

            var url = $"{WeatherURL}{location} 날씨";

            var sb = new StringBuilder();

            var htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument =  htmlWeb.Load(url);

            var node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='status_wrap']");

            if (node is null)
                throw new ArgumentException($"올바른 지역명을 입력해주세요.");

            sb.AppendLine($"오늘의 {location} 날씨");
            sb.AppendLine(GetSplitWeatherGraphic(node.SelectSingleNode("//div[@class='weather_graphic']")));
            sb.AppendLine(GetSplitWeekList(node.SelectSingleNode("//ul[@class='week_list']")));
            sb.AppendLine(GetSplitTemperatureInfo(node.SelectSingleNode("//div[@class='temperature_info']")));
            sb.AppendLine(GetSplitReportCardWrap(node.SelectSingleNode("//div[@class='report_card_wrap']")));

            return sb.ToString();
        }

        private IEnumerable<string> GetSplitText(string text, char separator)
        {
            return text.Split(' ').Where(item => !string.IsNullOrEmpty(item));
        }

        private string GetSplitWeatherGraphic(HtmlNode node)
        {
            if (node is null)
                return string.Empty;

            var nodeItems = node.InnerText.Split(' ').Where(item => !string.IsNullOrEmpty(item));

            if (nodeItems.Count() < 1)
                return string.Empty;

            var sb = new StringBuilder();

            sb.AppendLine(string.Join(" ", nodeItems.Skip(1)));
            sb.Append($"날씨 {nodeItems.Take(1).First()}");

            return sb.ToString(); ;
        } 

        private string GetSplitWeekList(HtmlNode node)
        {
            if (node is null)
                return string.Empty;

            var nodeItems = node.InnerText.Split(' ').Where(item => !string.IsNullOrEmpty(item));

            if (nodeItems.Count() < 1)
                return string.Empty;

            var sb = new StringBuilder();

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

        private string GetSplitTemperatureInfo(HtmlNode node)
        {
            if (node is null)
                return string.Empty;

            var nodeItems = node.InnerText.Split(' ').Where(item => !string.IsNullOrEmpty(item));

            if (nodeItems.Count() < 1)
                return string.Empty;

            var sb = new StringBuilder();

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

        private string GetSplitReportCardWrap(HtmlNode node)
        {
            if (node is null)
                return string.Empty;

            var nodeItems = node.InnerText.Split(' ').Where(item => !string.IsNullOrEmpty(item));

            if (nodeItems.Count() < 1)
                return string.Empty;

            var sb = new StringBuilder();

            try
            {
                for (int idx = 0; idx < nodeItems.Count(); idx += 2)
                    sb.AppendLine(string.Join(" : ", nodeItems.Skip(idx).Take(2)));
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 도움말 표시
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{CommandName} [지역]";
        }
    }
}
