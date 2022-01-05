using chanosBot.API;
using chanosBot.Bot.Markup;
using chanosBot.Core;
using chanosBot.Interface;
using chanosBot.Model;
using chanosBot.Model.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace chanosBot.Actions.OptionActions.Delivery
{
    internal class OptionDeliveryListAction : IOption, IDeliveryOption
    {
        public string OptionName { get; set; }
        public DeliveryAPI DeliveryAPI { get; set; }
        public DeliveryMarkup Markup { get; set; }

        public OptionDeliveryListAction(string optionName, DeliveryAPI api, DeliveryMarkup markup)
        {
            this.OptionName = optionName;
            this.DeliveryAPI = api;
            this.Markup = markup;
        }

        public BotResponse Execute(List<string> optionList)
        {
            var response = new BotResponse();

            var type = optionList.SingleOrDefault();

            if (type is null)
            {
                type = "국내";
            }

            response.Message = "🚚 택배사 목록\n택배사를 선택해 조회 코드를 확인하세요.";
            response.Keyboard = GetDeliveryList(type);

            return response;
        }

        private InlineKeyboardMarkup GetDeliveryList(string type)
        {
            var deliveryList = GetDeliveryListFromHtml();

            if (!string.IsNullOrEmpty(type))
            {
                switch (type)
                {
                    case "국내":
                        deliveryList = deliveryList.Where(delivery => delivery.Location == DeliveryLocationTypeEnum.Internal).ToList();
                        break;
                    case "해외":
                        deliveryList = deliveryList.Where(delivery => delivery.Location == DeliveryLocationTypeEnum.External).ToList();
                        break;
                    default:
                        throw new ArgumentException($"{type} 옵션의 값이잘못 됐습니다.\n\n{this}");
                }
            }

            return Markup.GetDeliveryList(deliveryList);
        }

        private List<DeliveryCompany> GetDeliveryListFromHtml() => DeliveryAPI.GetDeliveryListFromHtml();
    }
}
