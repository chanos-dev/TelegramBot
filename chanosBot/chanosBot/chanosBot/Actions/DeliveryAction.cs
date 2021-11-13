using chanosBot.Core;
using chanosBot.Interface;
using chanosBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace chanosBot.Actions
{
    public class DeliveryAction : ICommand
    {
        private string ApiKey;

        private string OptionRegisterApiKey = "/등록";

        private string OptionDeliveryList = "/리스트";

        public string CommandName => "/택배";

        public Option[] CommandOptions { get; }

        public DeliveryAction()
        {
            CommandOptions = new[]
            {
                new Option()
                {
                    OptionName = CommandName,
                    OptionLimitCounts = 0
                },
                new Option()
                {
                    OptionName = OptionDeliveryList,
                    OptionLimitCounts = 0,
                },
                new Option()
                {
                    OptionName = OptionRegisterApiKey,
                    OptionLimitCounts = 1,
                }
            };
        }

        public BotResponse Execute(params string[] options)
        {
            if (string.IsNullOrEmpty(ApiKey))
                throw new Exception("스마트택배 API Key가 등록되지 않았습니다.");

            CommandOptions.ClearOptionList();
            CommandOptions.FillOptionPair(options);
            CommandOptions.VerifyOptionCount();

            var response = new BotResponse();

            // todo : chain-of-responsibility 적용하기
            if (options.Contains(OptionRegisterApiKey))
            {
                var apiKey = CommandOptions.FindOption(OptionRegisterApiKey).OptionList.SingleOrDefault();
                response.Message = SetApiKey(apiKey);
            }
            else if (options.Contains(OptionDeliveryList))
            {
                response.Message = "택배사 목록";
                response.Keyboard = GetDeliveryList();
            }

            return response;
        }

        private InlineKeyboardMarkup GetDeliveryList()
        {
            var buttons = new InlineKeyboardButton[1][];

            return new InlineKeyboardMarkup(buttons);

        }

        private string SetApiKey(string apiKey)
        {
            this.ApiKey = apiKey;
            return "스마트 택배 API Key가 등록되었습니다.";
        }

        /// <summary>
        /// 도움말 표시
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{CommandName} [택배사]");

            return sb.ToString();
        }
    }
}
