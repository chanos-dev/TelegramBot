using chanosBot.API;
using chanosBot.Bot.Markup;
using chanosBot.Interface;
using chanosBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Actions.OptionActions.Delivery
{
    internal class OptionRegisterApiKeyAction : IOption, IDeliveryOption
    {
        public string OptionName { get; set; }
        public DeliveryAPI DeliveryAPI { get; set; }
        public DeliveryMarkup Markup { get; set; }

        public OptionRegisterApiKeyAction(string optionName, DeliveryAPI api, DeliveryMarkup markup)
        {
            this.OptionName = optionName;
            this.DeliveryAPI = api;
            this.Markup = markup;
        } 

        public BotResponse Execute(List<string> optionList)
        {
            var response = new BotResponse();

            var apiKey = optionList.SingleOrDefault();

            response.Message = SetApiKey(apiKey);

            return response;
        }

        private string SetApiKey(string apiKey)
        {
            DeliveryAPI.SetAPIKey(apiKey);
            return "스마트 택배 API Key가 등록되었습니다.";
        }
    }
}
