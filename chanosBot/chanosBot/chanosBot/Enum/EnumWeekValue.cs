using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Enum
{
    
    [Flags]
    public enum EnumWeekValue
    { 
        Sun = 1 >> 0,
        Mon = 1 >> 1,
        Tue = 1 >> 2,
        Wed = 1 >> 3,
        Thu = 1 >> 4,
        Fri = 1 >> 5,
        Sat = 1 >> 6,
    }
}
