using chanosBot.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chanosBot.Core
{
    public class AutoCommandHelper
    {
        private HashSet<AutoCommand> AutoCommands { get; set; }

        public AutoCommandHelper()
        {
            AutoCommands = new HashSet<AutoCommand>(new AutoCommandComparer());
        }

        public void AddAutoCommand(AutoCommand autoCommand)
        {
            // 이미 해당 유저/방에 세팅한 커맨드 값이 있으면 time만 바꿔주기.
            if (AutoCommands.Contains(autoCommand))
            {
                var findAutoCommand = AutoCommands.Where(AutoCommand => AutoCommand == autoCommand).Single();

                findAutoCommand.Time = autoCommand.Time;

                Log.Logger.Information($"Modify Auto Command ({autoCommand})");
            }
            else
            {
                AutoCommands.Add(autoCommand);

                Log.Logger.Information($"Add Auto Command ({autoCommand})");
            }
        }

        internal void Run()
        { 
            Task.Factory.StartNew(() =>
            {
                Log.Logger.Information($"Run AutoCommand Task.");

                while (true)
                {
                    // todo: 자동설정 시간에 맞는 command 가져오기.
                    var startCommands = AutoCommands;

                    foreach (var startCommand in startCommands)
                    {
                        startCommand.Command.Execute(startCommand.Options);
                        Log.Logger.Information($"Start command {startCommand} automatically.");
                    }
                    
                    Thread.Sleep(1000);
                }
            });
        }
    }
}
