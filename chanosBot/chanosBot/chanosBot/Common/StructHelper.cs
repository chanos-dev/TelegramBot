using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Common
{
    public static class StructHelper
    {
        public static bool CompareTimeSpanHourMinutePair(TimeSpan t1, TimeSpan t2)
        {
            return t1.Hours == t2.Hours && t1.Minutes == t2.Minutes;
        }
    }
}
