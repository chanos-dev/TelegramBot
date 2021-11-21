using chanosBot.API;
using chanosBot.Bot.Markup;
using chanosBot.Converter;
using chanosBot.Core;
using chanosBot.Enum;
using chanosBot.Interface;
using chanosBot.Model;
using chanosBot.Model.Delivery;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace chanosBot.Actions
{
    public class DeliveryAction : ICommand
    { 
        private DeliveryAPI DeliveryAPI { get; set; }

        private string OptionRegisterApiKey = "/등록";

        private string OptionDeliveryList = "/리스트";

        private string OptionDeliveryTracking = "/조회";

        private string OptionDeliveryEdit = "/변경";

        private DeliveryMarkup Markup { get; set; }

        public string CommandName => "/택배";

        public Option[] CommandOptions { get; }     

        public DeliveryAction()
        {
            DeliveryAPI = new DeliveryAPI();
            DeliveryAPI.LoadAPIKey();

            Markup = new DeliveryMarkup();

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
                    OptionLimitCounts = 1,
                },
                new Option()
                {
                    OptionName = OptionRegisterApiKey,
                    OptionLimitCounts = 1,
                },
                new Option()
                {
                    OptionName = OptionDeliveryTracking,
                    OptionLimitCounts = 3,
                },
                new Option()
                {
                    OptionName = OptionDeliveryEdit,
                    OptionLimitCounts = 0,
                }
            };
        }

        public BotResponse Execute(params string[] options)
        {            
            CommandOptions.ClearOptionList();
            CommandOptions.FillOptionPair(options);
            if (!CommandOptions.VerifyOptionCount(out string errorMessage))
            {
                throw new ArgumentException($"{errorMessage}\n\n{this}");
            }

            var response = new BotResponse();

            // todo : chain-of-responsibility 적용하기
            if (options.Contains(OptionRegisterApiKey))
            {
                var apiKey = CommandOptions.FindOption(OptionRegisterApiKey).OptionList.SingleOrDefault();
                response.Message = SetApiKey(apiKey);
            }
            else if (options.Contains(OptionDeliveryList))
            {
                var type = CommandOptions.FindOption(OptionDeliveryList).OptionList.SingleOrDefault();
                response.Message = "🚚 택배사 목록\n택배사를 선택해 조회 코드를 확인하세요.";
                response.Keyboard = GetDeliveryList(type);
            }
            else if (options.Contains(OptionDeliveryTracking))
            {
                var codes = CommandOptions.FindOption(OptionDeliveryTracking).OptionList.ToArray();

                if (codes.Contains("/이미지"))
                {
                    response.Message = "🔎 택배정보 조회 🔍";
                    response.File = new InputFileStream(GetImageDeliveryTracking(codes));
                }
                else
                {
                    response.Message = GetTextDeliveryTracking(codes);
                }
            }
            else if (options.Contains(OptionDeliveryEdit))
            {
                var edit = CommandOptions.FindOption(OptionDeliveryEdit).OptionList.ToArray();

                if (edit.Length == 0)
                {
                    response.Message = "👏 수정 메뉴 선택";
                    response.Keyboard = Markup.GetEditingMenu();
                } 
            }

            return response;
        }

        public BotResponse Replay(params string[] options)
        {
            CommandOptions.ClearOptionList();
            CommandOptions.FillOptionPair(options);
            if (!CommandOptions.VerifyOptionCount(out string errorMessage))
            {
                throw new ArgumentException($"{errorMessage}\n\n{this}");
            }

            var response = new BotResponse();

            if (options.Contains(OptionDeliveryEdit))
            {
                var edit = CommandOptions.FindOption(OptionDeliveryEdit).OptionList.ToArray();

                if (edit.Contains("/템플릿"))
                {
                    if (edit.Length == 1)
                    {
                        response.Message = "🧾 템플릿 선택";
                        response.Keyboard = Markup.GetTemplateMenu();
                    }
                    else
                    {                        
                        response.Message = DeliveryAPI.SetTemplateType((EnumTemplateTypeValue)int.Parse(edit.Skip(1).First()));
                    }
                }
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

        private InlineKeyboardMarkup GetDeliveryList(string type)
        { 
            var deliveryList = GetDeliveryListFromHtml();            

            if (!string.IsNullOrEmpty(type))
            {
                switch(type)
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

        private List<DeliveryCompany> GetDeliveryListFromHtml()
        {
            return DeliveryAPI.GetDeliveryListFromHtml();
        } 

        private string SetApiKey(string apiKey)
        {
            DeliveryAPI.SetAPIKey(apiKey);
            return "스마트 택배 API Key가 등록되었습니다.";
        }

        /// <summary>
        /// 도움말 표시
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"🚚 택배 커맨드 정보 🚚");            
            sb.AppendLine($"{CommandName} {OptionRegisterApiKey} [API KEY]");
            sb.AppendLine($"{CommandName} {OptionDeliveryList} [국내, 해외(기본값 : ALL)]");
            sb.Append($"{CommandName} {OptionDeliveryTracking} [조회 코드] [운송장 번호] [/이미지(옵션)]");

            return sb.ToString();
        } 
    }
}