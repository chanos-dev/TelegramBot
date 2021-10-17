using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Core
{
    internal static class ByteHelper
    {
        internal static long GetKBToByte(int number) => number * 1024;
        internal static long GetMBToByte(int number) => GetKBToByte(number) * 1024;

        internal static long GetGBToByte(int number) => GetMBToByte(number) * 1024;
    }
}
