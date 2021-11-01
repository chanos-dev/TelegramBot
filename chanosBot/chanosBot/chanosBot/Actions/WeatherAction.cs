using chanosBot.Core;
using chanosBot.Interface;
using chanosBot.Model;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Telegram.Bot.Types.InputFiles;

namespace chanosBot.Actions
{
    public class WeatherAction : ICommand
    {
        private string[] WeatherSplitItems { get; }

        private string AUTO_COMMAND_OPTION = "/자동설정";

        private string WeatherURL => "https://search.naver.com/search.naver?query=";

        public string CommandName => "/날씨";

        public Option[] CommandOptions { get; }

        public WeatherAction()
        {
            CommandOptions = new[]
            {
                new Option() 
                { 
                    OptionName = CommandName, 
                    OptionLimitCounts = 0 
                },
                new Option() 
                { 
                    OptionName = AUTO_COMMAND_OPTION, 
                    OptionLimitCounts = 1 
                },
            };

            WeatherSplitItems = new[]
            {
                "오전",
                "오후",
                "최저기온",
            };
        }

        public BotResponse Execute(params string[] options)
        {
            // options => "성남시", "수정구"
            // options => "성남시", "수정구", "/자동설정", "1130"

            if (options.Skip(1).Count() == 0)
                throw new ArgumentException($"지역명은 필수 입니다.\n예) {this}");

            CommandOptions.FillOptionPair(options);
            CommandOptions.VerifyOptionCount();             
            
            var location = string.Join(" ", CommandOptions.FindOption(CommandName).OptionList);

            var url = $"{WeatherURL}{location} 날씨";            

            var htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(url);

            var node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='status_wrap']");

            if (node is null)
                throw new ArgumentException($"올바른 지역명을 입력해주세요.");

            var sb = new StringBuilder();            

            sb.AppendLine($"오늘의 {location} 날씨");
            sb.AppendLine(GetSplitWeatherGraphic(node.SelectSingleNode("//div[@class='weather_graphic']")));
            sb.AppendLine(GetSplitWeekList(node.SelectSingleNode("//ul[@class='week_list']")));
            sb.AppendLine(GetSplitTemperatureInfo(node.SelectSingleNode("//div[@class='temperature_info']")));
            sb.AppendLine(GetSplitReportCardWrap(node.SelectSingleNode("//div[@class='report_card_wrap']")));

            AutoCommand autoCommand = null;

            Option auto = CommandOptions.FindOption(AUTO_COMMAND_OPTION);

            if (auto.HasOption)
            {
                autoCommand = new AutoCommand();

                var strTime = auto.OptionList.First();

                var hour = strTime.Substring(0, 2);
                var minute = strTime.Substring(2, 2); 

                if (!string.IsNullOrEmpty(hour) &&
                    !string.IsNullOrEmpty(minute))
                {
                    if (Time.VerifyTime(hour, minute, out Time time))
                        autoCommand.Time = time;
                    else
                        throw new ArgumentException("올바른 시간을 입력해주세요.");
                }
                else
                    autoCommand.Time = Time.Empty();

                autoCommand.Command = this;
                autoCommand.Options = new[] { CommandName }.Concat(CommandOptions.FindOption(CommandName).OptionList.ToArray()).ToArray();
            }

            return new BotResponse()
            {
                Message = sb.ToString(),
                File = new InputOnlineFile(new Uri("http://photo.hankooki.com/newsphoto/v001/2020/10/05/eyoree20201005071944_O_03_C_1.jpg"))
                {
                    FileName = "출처 : 데일리한국 - 한국아이 닷컴, 월드크리닝",
                },
                AutoCommand = autoCommand,
            };
        }

        #region Split Weather Methods
        private IEnumerable<string> GetSplitText(string text, char separator)
        {
            return text.Split(' ').Where(item => !string.IsNullOrEmpty(item));
        }

        private string GetSplitWeatherGraphic(HtmlNode node)
        {
            if (node is null)
                return string.Empty;

            var nodeItems = GetSplitText(node.InnerText, ' ');

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

            var nodeItems = GetSplitText(node.InnerText, ' ');

            if (nodeItems.Count() < 1)
                return string.Empty;

            var sb = new StringBuilder();

            nodeItems = nodeItems.Skip(2).Select(item =>
            {
                if (item == "오전" || item == "오후")
                    return $"{item} 강수확률";

                return item;
            }).TakeWhile(item => item != "내일"); 

            var items = new List<string>();
            foreach (var nodeItem in nodeItems)
            {
                if (WeatherSplitItems.Any(split => nodeItem.Contains(split)))
                {
                    if (items.Count > 0)
                    {
                        sb.Append(string.Join(" ", items));
                        items.Clear();
                    }

                    items.Add($"\n{nodeItem}");
                }
                else
                {
                    items.Add(nodeItem);
                }
            }

            if (items.Count > 0)
                sb.Append(string.Join(" ", items));

            // 첫 줄바꿈 제거
            sb.Remove(0, 1);
            return sb.ToString();
        }

        private string GetSplitTemperatureInfo(HtmlNode node)
        {
            if (node is null)
                return string.Empty;

            var nodeItems = GetSplitText(node.InnerText, ' ');

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

            var nodeItems = GetSplitText(node.InnerText, ' ');

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
        #endregion

        /// <summary>
        /// 도움말 표시
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{CommandName} [지역]");
            sb.Append($"{CommandName} [지역] /자동설정 [시간(24시표기:0830)]");

            return sb.ToString();
        }
    }
}
