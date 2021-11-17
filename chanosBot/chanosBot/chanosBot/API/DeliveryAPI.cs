using chanosBot.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chanosBot.API
{
    public class DeliveryAPI : BaseAPI
    {
        private EnumTemplateTypeValue TemplateType { get; set; } = EnumTemplateTypeValue.Tropical;

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

            t.ApartmentState = System.Threading.ApartmentState.STA;
            t.Start();
            t.Join();

            return ms;
        } 
    }
}