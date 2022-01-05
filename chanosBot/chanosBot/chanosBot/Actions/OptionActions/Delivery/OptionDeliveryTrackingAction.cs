using chanosBot.API;
using chanosBot.Bot.Markup;
using chanosBot.Interface;
using chanosBot.Model;
using chanosBot.Model.Delivery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InputFiles;

namespace chanosBot.Actions.OptionActions.Delivery
{
    internal class OptionDeliveryTrackingAction : IOption, IDeliveryOption
    {
        public string OptionName { get; set; }
        public DeliveryAPI DeliveryAPI { get; set; }
        public DeliveryMarkup Markup { get; set; }

        public OptionDeliveryTrackingAction(string optionName, DeliveryAPI api, DeliveryMarkup markup)
        {
            this.OptionName = optionName;
            this.DeliveryAPI = api;
            this.Markup = markup;
        }

        public BotResponse Execute(List<string> optionList)
        {
            var response = new BotResponse();

            var codes = optionList.ToArray();

            if (codes.Contains("/이미지"))
            {
                response.Message = "🔎 택배정보 조회 🔍";
                response.File = new InputFileStream(GetImageDeliveryTracking(codes));
            }
            else
            {
                response.Message = GetTextDeliveryTracking(codes);
            }

            return response;
        }

        private Stream GetImageDeliveryTracking(string[] codes)
        {
            if (codes.Length < 2)
                throw new ArgumentException($"[조회 코드] [운송장 코드]의 정보가 부족합니다.\n\n{this}");

            var stream = DeliveryAPI.GetImageDeliveryTracking(codes[0], codes[1]);

            if (stream is null)
                throw new WebException("택배 정보를 조회할 수 없습니다.\n\n잠시 후 재시도 해주세요.");

            return stream;
        }

        private string GetTextDeliveryTracking(string[] codes)
        {
            if (codes.Length < 2)
                throw new ArgumentException($"[조회 코드] [운송장 코드]의 정보가 부족합니다.\n\n{this}");

            var response = DeliveryAPI.GetTextDeliveryTracking(codes[0], codes[1]).Result;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var error = JsonConvert.DeserializeObject<DeliveryErrorMessage>(response.Result);
                throw new WebException(error.Msg);
            }

            return response.Result;
        }
    }
}
