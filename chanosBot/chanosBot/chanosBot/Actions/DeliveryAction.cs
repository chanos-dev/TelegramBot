using chanosBot.API;
using chanosBot.Converter;
using chanosBot.Core;
using chanosBot.Interface;
using chanosBot.Model;
using chanosBot.Model.Delivery;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace chanosBot.Actions
{
    public class DeliveryAction : ICommand
    {
        private string DeliveryListURL => "https://info.sweettracker.co.kr/v2/api-docs";
        private DeliveryAPI DeliveryAPI { get; set; }

        private string OptionRegisterApiKey = "/등록";

        private string OptionDeliveryList = "/리스트";

        public string CommandName => "/택배";

        public Option[] CommandOptions { get; }

        public DeliveryAction()
        {
            DeliveryAPI = new DeliveryAPI();

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
                response.Message = "택배사 목록";
                response.Keyboard = GetDeliveryList(type);
            }

            return response;
        }

        private InlineKeyboardMarkup GetDeliveryList(string type)
        {
            //var response = DeliveryAPI.GetDeliveryList().Result;

            //if (response.StatusCode != HttpStatusCode.OK)
            //{                
            //    var error = JsonConvert.DeserializeObject<DeliveryErrorMessage>(response.Result);
            //    throw new WebException(error.Msg);
            //}

            // get delivery company list 
            // NOTE : 택배 리스트를 전부 못 갖고 오는 경우가 있따. 리스트 별도 저장?
            //var companies = JsonConvert.DeserializeObject<DeliveryCollection>(response.Result);

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

            var buttonCollection = new List<List<InlineKeyboardButton>>();

            var buttons = new List<InlineKeyboardButton>();

            foreach (var delivery in deliveryList)
            {
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = delivery.Name,
                    CallbackData = delivery.Code,
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

        private List<DeliveryCompany> GetDeliveryListFromHtml()
        {
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            var html = webClient.DownloadString(DeliveryListURL);

            var roots = new[]
            {
                "info",
                "description",
            };

            var info = JsonConvert.DeserializeObject<string>(html, new SingleValueJsonConverter(roots));

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(info);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//table[@class='table table-bordered']/tbody");

            var deliveryCompanies = new List<DeliveryCompany>();

            foreach (var node in nodes)
            {
                DeliveryLocationTypeEnum locationType = DeliveryLocationTypeEnum.Internal;

                // DHL이 있으면 해외로 판단
                if (node.InnerText.Contains("DHL"))
                    locationType = DeliveryLocationTypeEnum.External;

                var findNodes = node.ChildNodes.Where(trNode => trNode.Name == "tr").SelectMany(n =>
                {
                    var tdList = n.ChildNodes.Where(tdNode => tdNode.Name == "td").ToList();

                    List<string> companiesList = new List<string>();

                    for (int idx = 0; idx < tdList.Count; idx += 2)
                    {
                        companiesList.Add($"{tdList[idx].InnerText}&{tdList[idx + 1].InnerText}");
                    }

                    return companiesList;
                });


                // 찾은 택배사 add
                foreach (var findNode in findNodes)
                {
                    var items = findNode.Split('&');

                    if (items.Length != 2)
                        continue;

                    deliveryCompanies.Add(new DeliveryCompany()
                    {
                        Name = items[0],
                        Code = items[1],
                        Location = locationType
                    });
                }
            }

            return deliveryCompanies;
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
            sb.AppendLine($"{CommandName} {OptionRegisterApiKey} [API KEY]");
            sb.Append($"{CommandName} {OptionDeliveryList} [국내, 해외(기본값 : ALL)]");

            return sb.ToString();
        }
    }
}
