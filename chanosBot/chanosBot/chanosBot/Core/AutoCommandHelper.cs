using chanosBot.Common;
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

        public delegate void DelegateSendMessage(AutoCommand autoCommand);
        public event DelegateSendMessage EventSendMessage;

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

                findAutoCommand.Options = autoCommand.Options;
                findAutoCommand.Time = autoCommand.Time;
                autoCommand.IsSent = false;

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
                    if (EventSendMessage is null)
                        continue;

                    var curTime = DateTime.Now.TimeOfDay;

                    // TODO : IsSent process - Refactoring
                    AutoCommands.ToList().ForEach(command =>
                    {
                        if (!command.IsSent)
                            return;

                        if (!StructHelper.CompareTimeSpanHourMinutePair((TimeSpan)command.Time, curTime))
                            command.IsSent = false;
                    });

                    var startCommands = AutoCommands.Where(command =>
                    {
                        return StructHelper.CompareTimeSpanHourMinutePair((TimeSpan)command.Time, curTime) && !command.IsSent;
                    });

                    foreach (var startCommand in startCommands)
                    {
                        EventSendMessage.Invoke(startCommand);
                        startCommand.IsSent = true;

                        Log.Logger.Information($"Start command {startCommand} automatically.");
                    }
                    
                    Thread.Sleep(5000);
                }
            });
        }
    }
}
