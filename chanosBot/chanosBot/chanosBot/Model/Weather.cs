using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace chanosBot.Model
{
    public class Weather
    {
        internal int Hour { get; set; }
        internal int Day { get; set; }
        internal float Temp { get; set; }
        internal float Tmx { get; set; }
        internal float Tmn { get; set; }
        internal int Sky { get; set; }
        internal int Pty { get; set; }
        internal string WfKor { get; set; }
        internal string WfEn { get; set; }
        internal int Pop { get; set; }
        internal float R12 { get; set; }
        internal float S12 { get; set; }
        internal float Ws { get; set; }
        internal int Wd { get; set; }
        internal string WdKor { get; set; }
        internal string WdEn { get; set; }
        internal int Reh { get; set; }
        internal float R06 { get; set; }
        internal float S06 { get; set; }

        public static Weather CreateInstance(XmlElement node)            
        {
            return new Weather()
            {
                Hour = int.Parse(node.SelectSingleNode("hour").InnerText),
                Day = int.Parse(node.SelectSingleNode("day").InnerText),
                Temp = float.Parse(node.SelectSingleNode("temp").InnerText),
                Tmx = float.Parse(node.SelectSingleNode("tmx").InnerText),
                Tmn = float.Parse(node.SelectSingleNode("tmn").InnerText),
                Sky = int.Parse(node.SelectSingleNode("sky").InnerText),
                Pty = int.Parse(node.SelectSingleNode("pty").InnerText),
                WfKor = node.SelectSingleNode("wfKor").InnerText,
                WfEn = node.SelectSingleNode("wfEn").InnerText,
                Pop = int.Parse(node.SelectSingleNode("pop").InnerText),
                R12 = float.Parse(node.SelectSingleNode("r12").InnerText),
                S12 = float.Parse(node.SelectSingleNode("s12").InnerText),
                Ws = float.Parse(node.SelectSingleNode("ws").InnerText),
                Wd = int.Parse(node.SelectSingleNode("wd").InnerText),
                WdKor = node.SelectSingleNode("wdKor").InnerText,
                WdEn = node.SelectSingleNode("wdEn").InnerText,
                Reh = int.Parse(node.SelectSingleNode("reh").InnerText),
                R06 = float.Parse(node.SelectSingleNode("r06").InnerText),
                S06 = float.Parse(node.SelectSingleNode("s06").InnerText),
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"시간 : {this.Hour}");
            sb.AppendLine($"날씨 : {this.WfKor}");

            return sb.ToString();
        }
    }
}
