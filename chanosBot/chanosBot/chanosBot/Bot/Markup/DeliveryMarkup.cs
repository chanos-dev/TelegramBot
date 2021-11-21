using chanosBot.Model.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace chanosBot.Bot.Markup
{
    public class DeliveryMarkup : BaseMarkup
    {
        public InlineKeyboardMarkup GetDeliveryList(List<DeliveryCompany> deliveryList)
        {
            var buttonCollection = new List<List<InlineKeyboardButton>>();

            var buttons = new List<InlineKeyboardButton>();

            foreach (var delivery in deliveryList)
            {
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = delivery.Name,
                    CallbackData = $"🏢{delivery.Name}의 조회 코드 : {delivery.Code}",
                });

                if (buttons.Count == 3)
                {
                    buttonCollection.Add(buttons);
                    buttons = new List<InlineKeyboardButton>();
                }
            }

            if (buttons.Count != 0)
                buttonCollection.Add(buttons);

            return new InlineKeyboardMarkup(buttonCollection);
        }

        public InlineKeyboardMarkup GetTemplateMenu()
        {
            var buttonCollection = new List<List<InlineKeyboardButton>>()
            {
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton()
                    {
                        Text = "Cyan",
                        CallbackData = "/택배 /변경 /템플릿 1",
                    },
                    new InlineKeyboardButton()
                    {
                        Text = "Pink",
                        CallbackData = "/택배 /변경 /템플릿 2",
                    },
                    new InlineKeyboardButton()
                    {
                        Text = "Gray",
                        CallbackData = "/택배 /변경 /템플릿 3",
                    },
                },
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton()
                    {
                        Text = "Tropical",
                        CallbackData = "/택배 /변경 /템플릿 4",
                    },
                    new InlineKeyboardButton()
                    {
                        Text = "Sky",
                        CallbackData = "/택배 /변경 /템플릿 5",
                    },
                },
            };

            return new InlineKeyboardMarkup(buttonCollection);
        }

        public InlineKeyboardMarkup GetEditingMenu()
        {
            var buttonCollection = new List<List<InlineKeyboardButton>>()
            {
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton()
                    {
                        Text = "템플릿 변경",
                        CallbackData = "/택배 /변경 /템플릿",
                    },
                },
            };

            return new InlineKeyboardMarkup(buttonCollection);
        }
    }
}
