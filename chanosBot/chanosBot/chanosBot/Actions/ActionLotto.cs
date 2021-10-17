using chanosBot.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Actions
{
    internal class ActionLotto : ICommand
    {
        public string CommandName => "/로또";

        public string Execute(params string[] options)
        {
            if (options.Length > 1)
                throw new ArgumentException($"Too many options.\ne.g) {CommandName} count[default 1]");

            var count = 1;

            if (options.Length == 1 &&
                int.TryParse(options[0], out int result) &&
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

            return sb.ToString();
        }

        /// <summary>
        /// 도움말 표시
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "/로또 {number}";
        }
    }
}
