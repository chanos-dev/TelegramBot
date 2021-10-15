using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Core
{
    internal static class ByteHelper
    {
        internal static long GetKB(int number)
        {
            return number * 1024;
        }
        internal static long GetMB(int number)
        {
            return GetKB(number) * 1024;
        }

        internal static long GetGB(int number)
        {
            return GetMB(number) * 1024;
        }
    }
}
