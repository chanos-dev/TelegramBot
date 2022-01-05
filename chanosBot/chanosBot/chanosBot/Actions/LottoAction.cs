using chanosBot.Core;
using chanosBot.Interface;
using chanosBot.Model;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Actions
{
    public class LottoAction : ICommand
    {
        private string OptionPrize = "/정답";

        private string SearchURL => "https://search.naver.com/search.naver?query=";

        private string LottoURL => "https://www.dhlottery.co.kr/common.do?method=getLottoNumber&drwNo=";

        public string CommandName => "/로또";

        public Option[] CommandOptions { get; }

        public LottoAction()
        {
            CommandOptions = new[]
            {
                new Option()
                {
                    OptionName = CommandName,
                    OptionLimitCounts = 1
                }, 
                new Option()
                {
                    OptionName = OptionPrize,
                    OptionLimitCounts = 1,
                }
            };
        }

        public BotResponse Execute(params string[] options)
        {
            CommandOptions.ClearOptionList();
            CommandOptions.FillOptionPair(options);
            if (!CommandOptions.VerifyOptionCount(out string errorMessage))
            {
                throw new ArgumentException($"{errorMessage}\n\n{this}");
            }

            var response = new BotResponse();

            if (options.Contains(OptionPrize))
            {
                var prize = CommandOptions.FindOption(OptionPrize).OptionList.SingleOrDefault();
                response.Message = GetPrizeNumber(prize);
            }
            else
            {
                var inputCount = CommandOptions.FindOption(CommandName).OptionList.SingleOrDefault();

                if (inputCount is null)
                    throw new ArgumentException(this.ToString());

                response.Message = GetLottoList(inputCount);
            }

            return response;
        }

        private string GetPrizeNumber(string prize)
        {
            var time = prize;
            var htmlWeb = new HtmlWeb();
            var url = string.Empty;

            if (string.IsNullOrEmpty(time))
            {
                url = $"{SearchURL}로또 당첨번호";
                
                HtmlDocument searchHtmlDoc = htmlWeb.Load(url);
                var node = searchHtmlDoc.DocumentNode.SelectSingleNode("//a[@class='_lotto-btn-current']");
                
                time = string.Join("", node.InnerText.TakeWhile(text => text != '회'));
            }

            url = $"{LottoURL}{time}";
            
            HtmlDocument lottoHemlDoc = htmlWeb.Load(url);

            var lotto = JsonConvert.DeserializeObject<Lotto>(lottoHemlDoc.DocumentNode.InnerText);

            return lotto.ToString();
        }

        private string GetLottoList(string inputCount)
        {
            var count = 1;

            if (!string.IsNullOrEmpty(inputCount))
            {
                if (!int.TryParse(inputCount, out count))
                    throw new ArgumentException(this.ToString());
            }

            var sb = new StringBuilder();
            var random = new Random();
            var lotto = new HashSet<int>();

            for (int idx = 0; idx < count; idx++)
            {
                while (lotto.Count < 6)
                {
                    lotto.Add(random.Next(1, 46));
                }

                sb.AppendLine($"🎱 순번 {idx + 1} : {string.Join(",", lotto.OrderBy(l => l))}");
                lotto.Clear();
            }

            return sb.ToString();
        }

        /// <summary>
        /// 도움말 표시
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"💲 로또 커맨드 정보 💲");
            sb.AppendLine($"{CommandName} [숫자]");
            sb.Append($"{CommandName} {OptionPrize} [회차(기본값 : 최근)]");

            return sb.ToString(); 
        }

        public BotResponse Replay(params string[] options)
        {
            throw new NotImplementedException();
        }
    }
}
