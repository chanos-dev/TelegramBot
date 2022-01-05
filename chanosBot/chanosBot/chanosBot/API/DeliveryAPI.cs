using chanosBot.Converter;
using chanosBot.Enum;
using chanosBot.Model.Delivery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chanosBot.API
{
    public class DeliveryAPI : BaseAPI
    {
        private EnumTemplateTypeValue TemplateType { get; set; } = EnumTemplateTypeValue.Tropical;

        private string DeliveryListURL => "https://info.sweettracker.co.kr/v2/api-docs";

        private string DeliveryTrackingURL => $"https://info.sweettracker.co.kr/tracking/{TemplateType:d}";

        protected override string BaseURL => "http://info.sweettracker.co.kr";

        protected override string[] MiddleURL { get; set; }

        public string SetTemplateType(EnumTemplateTypeValue templateTypeValue)
        {
            var prevType = TemplateType;

            TemplateType = templateTypeValue;

            return $"템플릿 타입 변경 완료\n{prevType} -> {templateTypeValue}";
        }

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

        public async Task<APIResponse> GetTextDeliveryTracking(string code, string invoice)
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

        public Stream GetImageDeliveryTracking(string code, string invoice)
        {
            ContentType = "application/x-www-form-urlencoded";

            QueryParameters = new Dictionary<string, string>()
            {
                ["t_key"] = APIKey,
                ["t_code"] = code,
                ["t_invoice"] = invoice,
            };

            // TODO : STA 관련 내용 알아보기...
            var ms = new MemoryStream();

            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                try
                {
                    byte[] postData = Encoding.UTF8.GetBytes(QueryParametersToString);

                    using (var browser = new WebBrowser())
                    {
                        browser.Navigate(DeliveryTrackingURL, targetFrameName: null, postData: postData, additionalHeaders: $"Content-Type: {ContentType}");

                        while (browser.ReadyState != WebBrowserReadyState.Complete)
                            Application.DoEvents();

                        browser.Size = new Size(450, browser.Document.Body.ScrollRectangle.Height);

                        using (var bitmap = new Bitmap(browser.Document.Body.ScrollRectangle.Width, browser.Document.Body.ScrollRectangle.Height))
                        {
                            browser.DrawToBitmap(bitmap, new Rectangle(0, 0, browser.Document.Body.ScrollRectangle.Width, browser.Document.Body.ScrollRectangle.Height));

                            bitmap.Save(ms, ImageFormat.Jpeg);
                        }

                        //return ms;
                    }
                }
                catch(Exception ex)
                {

                }
            });

            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();

            return ms;
        } 

        public List<DeliveryCompany> GetDeliveryListFromHtml()
        {
            using (var webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                var html = webClient.DownloadString(DeliveryListURL);

                var roots = new[]
                {
                    "info",
                    "description",
                };

                var info = JsonConvert.DeserializeObject<string>(html, new SingleValueJsonConverter(roots));

                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
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
        }
    }
}