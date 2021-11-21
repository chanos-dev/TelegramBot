using chanosBot.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.API
{
    public abstract class BaseAPI
    {
        private AESCrypto Crypto { get; set; }
        private string CryptoKey { get; set; }

        private string APIPath => $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Bot\Keys";

        protected virtual string APIFileName => "api.tbchan";

        private string FileFullPath => Path.Combine(APIPath, APIFileName);

        protected string APIKey { get; set; }

        public bool IsEmptyKey => string.IsNullOrEmpty(APIKey);

        protected string ContentType { get; set; }

        protected abstract string BaseURL { get; }

        protected abstract string[] MiddleURL { get; set; }

        protected string MethodName { get; set; }

        private string APIURL => string.Join("/", Enumerable.Concat(new[] { BaseURL }, MiddleURL).Concat(new[] { MethodName }));

        protected Dictionary<string, string> QueryParameters { get; set; }

        protected string QueryParametersToString => string.Join("&", QueryParameters.Select(qp => $"{qp.Key}={qp.Value}"));

        public BaseAPI() 
        {
            Crypto = new AESCrypto();
            CryptoKey = Crypto.CreateKey("chanostelegram1998margeletsonahc");

            if (!Directory.Exists(APIPath))
                Directory.CreateDirectory(APIPath);
        }

        public BaseAPI(string key) : base()
        {
            SetAPIKey(key);
        }
        
        public void SetAPIKey(string apiKey)
        {
            this.APIKey = apiKey;

            File.WriteAllText(FileFullPath, Crypto.Encrypt(CryptoKey, apiKey));
        }

        public void LoadAPIKey()
        {
            if (File.Exists(FileFullPath))
            {
                var data = File.ReadAllText(FileFullPath).Trim();

                this.APIKey = Crypto.Decrypt(CryptoKey, data);
            }
        }

        protected async Task<APIResponse> Get()
        {
            try
            {
                APIResponse response = new APIResponse();

                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage httpresponse = await httpClient.GetAsync(GetAPIURLWithParameter());

                    //httpresponse.EnsureSuccessStatusCode();
                    response.Result = httpresponse.Content.ReadAsStringAsync().Result;
                    response.StatusCode = httpresponse.StatusCode;
                }

                return response;
            }
            finally
            {
                QueryParameters = null;
            }
        } 

        private string GetAPIURLWithParameter()
        {
            if (QueryParameters is null)
                return APIURL;

            return $"{APIURL}?{QueryParametersToString}";
        }
    }
}
