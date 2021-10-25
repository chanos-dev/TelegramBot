using chanosBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Interface
{
    internal interface ICommand
    {
        string CommandName { get; }
        string[] Options { get; }
        BotResponse Execute(params string[] options);
    }
}
