using chanosBot.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Actions
{
    internal class WeatherAction : ICommand
    {
        public string CommandName => "/날씨";

        public string Execute(params string[] options)
        {
            if (options.Length > 1)
                throw new ArgumentException($"Too many options.\ne.g) {CommandName} location[default all]");

            var count = 1;

            if (options.Length == 1 &&
                int.TryParse(options[0], out int result) &&
                result > 0)
                count = result;
            
            return "";
        }
    }
}
