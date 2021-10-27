using chanosBot.Interface;
using chanosBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Actions
{
    internal class ActionController : ICommand
    {
        private const char MESSAGE_SEPARATOR = ' ';

        private List<ICommand> Commands { get; set; }

        public string CommandName => "/도움말";

        public Option[] CommandOptions => throw new NotImplementedException();

        public ActionController()
        { 
            Commands = new List<ICommand>()
            {
                this,
                new LottoAction(),
                new WeatherAction(),
            };
        }

        internal BotResponse GetExecuteMessage(string message)
        {
            var items = message.Split(MESSAGE_SEPARATOR);

            var inputCommand = items[0];
            var options = items;// items.Skip(1).ToArray();

            var findCommand = Commands.Where(command => command.CommandName == inputCommand).SingleOrDefault();
            
            if (findCommand is null)
                throw new ArgumentException($"지원하지 않는 명령어 입니다. ({message})");

            return findCommand.Execute(options);
        }

        public BotResponse Execute(params string[] options)
        {
            return new BotResponse()
            {
                Message = string.Join(Environment.NewLine, Commands.Where(command => command.CommandName != this.CommandName)),
            };
        }
    }
}
