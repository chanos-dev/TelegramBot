using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Converter
{
    public class SingleValueJsonConverter : JsonConverter
    {
        private string[] Roots { get; set; }

        public SingleValueJsonConverter(string[] roots)
        {
            this.Roots = roots;
        }

        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (JToken.ReadFrom(reader) is JToken token)
            {
                var singleObject = GetChildObject(token);

                return singleObject.ToObject(objectType);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private JToken GetChildObject(JToken token)
        {
            try
            {
                foreach (var root in Roots)
                {
                    token = token[root];
                }
            }
            catch
            {
                throw;
            }

            return token;
        }
    }
}
