using chanosBot.Enum;
using chanosBot.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Model
{    
    internal class AutoCommand
    { 
        internal long ChatID { get; set; }

        internal int UserID { get; set; }

        internal EnumWeekValue Week { get; set; }

        internal Time Time { get; set; } 

        internal ICommand Command { get; set; }
    }
}
