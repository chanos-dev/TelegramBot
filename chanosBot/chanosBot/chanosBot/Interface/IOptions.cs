using chanosBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Interface
{
    internal interface IOption
    {
        string OptionName { get; set; }

        BotResponse Execute(List<string> optionList);
    }
}
