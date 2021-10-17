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
        string Execute(params string[] options);
    }
}
