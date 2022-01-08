using chanosBot.Actions.OptionActions.Delivery;
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

            // 커맨드 우선순위에 따라 인덱스 조절            
            CommandOptions = new[]
            { 
                new Option()
                {
                    OptionName = OptionRegisterApiKey,
                    OptionLimitCounts = 1,
                    OptionAction = new OptionRegisterApiKeyAction(OptionRegisterApiKey, DeliveryAPI, Markup),
                },
                new Option()
                {
                    OptionName = OptionDeliveryList,
                    OptionLimitCounts = 1,
                    OptionAction = new OptionDeliveryListAction(OptionDeliveryList, DeliveryAPI, Markup),
                }, 
                new Option()
                {
                    OptionName = OptionDeliveryTracking,
                    OptionLimitCounts = 3,
                    OptionAction = new OptionDeliveryTrackingAction(OptionDeliveryTracking, DeliveryAPI, Markup),
                },
                new Option()
                {
                    OptionName = OptionDeliveryEdit,
                    OptionLimitCounts = 0,
                    OptionAction = new OptionDeliveryEditAction(OptionDeliveryEdit, DeliveryAPI, Markup),
                },
                new Option()
                {
                    OptionName = CommandName,
                    OptionLimitCounts = 0
                },
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

            foreach(var command in CommandOptions)
            {
                if (!options.Contains(command.OptionName))
                {
                    continue;
                }

                if (command.OptionAction is null)
                {
                    throw new ArgumentException(this.ToString());
                }

                return command.OptionAction.Execute(command.OptionList);
            }

            throw new ArgumentException($"{errorMessage}\n\n{this}");
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

        /// <summary>
        /// 도움말 표시
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"🚚 택배 커맨드 정보 🚚");            
            sb.AppendLine($"{CommandName} {OptionRegisterApiKey} [API KEY]");
            sb.AppendLine($"{CommandName} {OptionDeliveryList} [국내, 해외(기본값 : 국내)]");
            sb.AppendLine($"{CommandName} {OptionDeliveryTracking} [조회 코드] [운송장 번호] [/이미지(옵션)]");
            sb.Append($"{CommandName} {OptionDeliveryEdit}");

            return sb.ToString();
        } 
    }
}