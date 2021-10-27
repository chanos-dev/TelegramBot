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
            CommandOptions = null;
        }

        public BotResponse Execute(params string[] options)
        {
            if (options.Length > 2)
                throw new ArgumentException($"옵션이 너무 많습니다.\n예) {this.ToString()}");

            var count = 1;

            if (options.Length == 2 &&
                int.TryParse(options[1], out int result) &&
                result > 0)
                count = result;

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
