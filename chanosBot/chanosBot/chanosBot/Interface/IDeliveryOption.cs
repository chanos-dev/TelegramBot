using chanosBot.API;
using chanosBot.Bot.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Interface
{
    internal interface IDeliveryOption
    {
        DeliveryAPI DeliveryAPI { get; set; } 

        DeliveryMarkup Markup { get; set; }
    }
}
