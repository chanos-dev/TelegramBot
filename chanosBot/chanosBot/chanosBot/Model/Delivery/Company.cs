using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Model.Delivery
{
    public class DeliveryCollection
    {
        [JsonProperty("Company")]
        public List<DeliveryCompany> Companies { get; set; }
    }

    public class DeliveryCompany
    {
        public string Code { get; set; }
        public string Name { get; set; }
    } 
}
