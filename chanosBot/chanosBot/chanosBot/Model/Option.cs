using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Model
{
    public class Option
    {
        internal string OptionName { get; set; }

        internal int OptionLimitCounts { get; set; }

        internal bool HasOption => OptionList.Count > 0;

        internal List<string> OptionList { get; set; } = new List<string>();
    }
}
