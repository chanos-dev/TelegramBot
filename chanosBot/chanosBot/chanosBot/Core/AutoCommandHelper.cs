using chanosBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Core
{
    internal class AutoCommandHelper
    {
        internal HashSet<AutoCommand> AutoCommand { get; set; }

        public AutoCommandHelper()
        {
            AutoCommand = new HashSet<AutoCommand>();
        }

        internal void Run()
        {
            Task.Factory.StartNew(() =>
            {
                while(true)
                {

                }
            });
        }
    }
}
