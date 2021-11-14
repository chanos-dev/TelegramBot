using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.API
{
    public class DeliveryAPI : BaseAPI
    {
        protected override string BaseURL => "http://info.sweettracker.co.kr";
        protected override string[] MiddleURL { get; set; }

        public DeliveryAPI()
        {
            MiddleURL = new[]
            {
                "api",
                "v1",
            };
        } 

        public async Task<APIResponse> GetDeliveryList()
        { 
            MethodName = "companylist";

            QueryParameters = new Dictionary<string, string>()
            {
                ["t_key"] = APIKey,
            };

            var response = await Get();

            return response;
        }

        public async Task<APIResponse> GetDeliveryTracking(string code, string invoice)
        {
            MethodName = "trackingInfo";

            QueryParameters = new Dictionary<string, string>()
            {
                ["t_key"] = APIKey,
                ["t_code"] = code,
                ["t_invoice"] = invoice,
            };

            var response = await Get();

            return response;
        }
    }
}