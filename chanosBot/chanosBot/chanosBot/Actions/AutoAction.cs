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
    public class AutoAction : ICommand
    {
        public string CommandName => "/자동설정";

        public Option[] CommandOptions { get; }

        public AutoAction()
        {

        }

        public BotResponse Execute(params string[] options)
        {
            CommandOptions.FillOptionPair(options);
            if (!CommandOptions.VerifyOptionCount(out string errorMessage))
            {
                throw new ArgumentException($"{errorMessage}\n\n{this}");
            }

            return new BotResponse()
            {
                Message = "",
            };
        }
    }
}
