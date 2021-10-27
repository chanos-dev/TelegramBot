using chanosBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Interface
{
    public interface ICommand
    {
        string CommandName { get; }
        Option[] CommandOptions { get; }
        BotResponse Execute(params string[] options);
    }
}
