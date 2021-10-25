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
        internal string ChatID { get; set; }

        internal string UserID { get; set; }

        internal EnumWeekValue Week { get; set; }

        internal int Hour { get; set; }

        internal int Minute { get; set; }

        internal ICommand Command { get; set; }
    }
}
