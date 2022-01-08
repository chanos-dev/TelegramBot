using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Model.Delivery
{
    public class TrackingDetail
    {
        [JsonProperty("time")]
        public object Time { get; set; }

        [JsonProperty("timeString")]
        public string TimeString { get; set; }

        [JsonProperty("code")]
        public object Code { get; set; }

        [JsonProperty("where")]
        public string Where { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("telno")]
        public string Telno { get; set; }

        [JsonProperty("telno2")]
        public string Telno2 { get; set; }

        [JsonProperty("remark")]
        public object Remark { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("manName")]
        public string ManName { get; set; }

        [JsonProperty("manPic")]
        public string ManPic { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"배송시간 : {TimeString}");
            sb.AppendLine($"현재위치 : {Where}");
            sb.AppendLine($"배송상태 : {Kind}\n");

            return sb.ToString();
        }
    }
}
