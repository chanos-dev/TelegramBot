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
    internal class OptionDeliveryEditAction : IOption, IDeliveryOption
    {
        public string OptionName { get; set; }
        public DeliveryAPI DeliveryAPI { get; set; }
        public DeliveryMarkup Markup { get; set; }

        public OptionDeliveryEditAction(string optionName, DeliveryAPI api, DeliveryMarkup markup)
        {
            this.OptionName = optionName;
            this.DeliveryAPI = api;
            this.Markup = markup;
        }

        public BotResponse Execute(List<string> optionList)
        {
            var response = new BotResponse();

            var edit = optionList.ToArray();

            if (edit.Length == 0)
            {
                response.Message = "👏 수정 메뉴 선택";
                response.Keyboard = Markup.GetEditingMenu();
            }

            return response;
        }
         
    }
}
