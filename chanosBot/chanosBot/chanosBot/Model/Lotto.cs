using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Model
{
    internal class Lotto
    {
        [JsonProperty("totSellamnt")]
        public long TotSellamnt { get; set; }

        [JsonProperty("returnValue")]
        public string ReturnValue { get; set; }

        [JsonProperty("drwNoDate")]
        public string DrwNoDate { get; set; }

        [JsonProperty("firstWinamnt")]
        public long FirstWinamnt { get; set; }

        [JsonProperty("drwtNo6")]
        public int DrwtNo6 { get; set; }

        [JsonProperty("drwtNo4")]
        public int DrwtNo4 { get; set; }

        [JsonProperty("firstPrzwnerCo")]
        public int FirstPrzwnerCo { get; set; }

        [JsonProperty("drwtNo5")]
        public int DrwtNo5 { get; set; }

        [JsonProperty("bnusNo")]
        public int BnusNo { get; set; }

        [JsonProperty("firstAccumamnt")]
        public long FirstAccumamnt { get; set; }

        [JsonProperty("drwNo")]
        public int DrwNo { get; set; }

        [JsonProperty("drwtNo2")]
        public int DrwtNo2 { get; set; }

        [JsonProperty("drwtNo3")]
        public int DrwtNo3 { get; set; }

        [JsonProperty("drwtNo1")]
        public int DrwtNo1 { get; set; }        

        public override string ToString()
        {
            if (DrwNo == 0)
                return "회차 정보가 존재하지 않습니다.";

            var sb = new StringBuilder();

            sb.AppendLine($"{DrwNo}회차 당첨번호 ({DrwNoDate})");
            sb.AppendLine("");
            sb.AppendLine($"당첨번호 : {DrwtNo1}, {DrwtNo2}, {DrwtNo3}, {DrwtNo4}, {DrwtNo5}, {DrwtNo6} + ({BnusNo}) ");
            sb.AppendLine("");
            sb.AppendLine($"총 금액 : {TotSellamnt:#,##0}");
            sb.AppendLine($"1등 금액 : {FirstAccumamnt:#,##0}");
            sb.AppendLine($"당첨게임 수 : {FirstPrzwnerCo:#,##0}");
            sb.AppendLine($"1게임당 당첨금액 : {FirstWinamnt:#,##0}");

            return sb.ToString();
        }
    }
}
