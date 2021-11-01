using chanosBot.Core;
using chanosBot.Interface;
using chanosBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Actions
{
    public class LottoAction : ICommand
    {
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
            };
        }

        public BotResponse Execute(params string[] options)
        { 
            CommandOptions.FillOptionPair(options);
            CommandOptions.VerifyOptionCount();

            var inputCount = CommandOptions.FindOption(CommandName).OptionList.SingleOrDefault();

            var count = 1;

            if (!string.IsNullOrEmpty(inputCount))
            {
                if (!int.TryParse(inputCount, out count))
                    throw new ArgumentException($"{inputCount} 옵션의 값이잘못 됐습니다.\n{this}");
            }

            var sb = new StringBuilder();
            var random = new Random();
            var lotto = new HashSet<int>();

            for(int idx = 0; idx < count; idx++)
            {
                while(lotto.Count < 6)
                {
                    lotto.Add(random.Next(1, 46));
                }

                sb.AppendLine($"{idx + 1} : {string.Join(",", lotto.OrderBy(l => l))}");
                lotto.Clear();
            }

            return new BotResponse()
            {
                Message = sb.ToString(),
            };
        }

        /// <summary>
        /// 도움말 표시
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{CommandName} [숫자(기본값 : 1)]";
        }
    }
}
