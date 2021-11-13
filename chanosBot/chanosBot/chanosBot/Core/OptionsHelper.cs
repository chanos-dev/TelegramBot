using chanosBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Core
{
    internal static class OptionsHelper
    {
        internal static void FillOptionPair(this Option[] options, string[] commands)
        {
            Option curOption = null;

            foreach (var command in commands)
            {
                var findOption = options.Where(option => option.OptionName == command).SingleOrDefault();

                if (findOption is Option op)
                {
                    curOption = op;
                    op.OptionList.Clear();
                }
                else
                    curOption?.OptionList.Add(command);
            }
        }

        internal static void VerifyOptionCount(this Option[] options)
        {
            foreach (var option in options)
            {
                //if (option.OptionLimitCounts == 0)
                //    continue;

                if (option.OptionLimitCounts < option.OptionList.Count)
                    throw new ArgumentException($"({option.OptionName})의 옵션이 너무 많습니다.\n잘못된 옵션 : {string.Join(" ", option.OptionList)}");
            }
        }

        internal static Option FindOption(this Option[] options, string key)
        {
            return options.Where(option => option.OptionName == key).FirstOrDefault();
        }

        internal static void ClearOptionList(this Option[] options)
        {
            foreach (var option in options)
                option.OptionList.Clear();
        }
    }
}
