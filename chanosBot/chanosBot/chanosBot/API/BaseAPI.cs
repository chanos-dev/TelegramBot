using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.API
{
    public abstract class BaseAPI
    {
        protected string APIKey { get; set; }

        public bool IsEmptyKey => string.IsNullOrEmpty(APIKey);

        protected string ContentType { get; set; }

        protected abstract string BaseURL { get; }

        protected abstract string[] MiddleURL { get; set; }

        protected string MethodName { get; set; }

        private string APIURL => string.Join("/", Enumerable.Concat(new[] { BaseURL }, MiddleURL).Concat(new[] { MethodName }));

        protected Dictionary<string, string> QueryParameters { get; set; }

        protected string QueryParametersToString => string.Join("&", QueryParameters.Select(qp => $"{qp.Key}={qp.Value}"));

        public BaseAPI() { } 

        public BaseAPI(string key)
        {
            this.APIKey = key;
        }

        public void SetAPIKey(string key)
        {
            this.APIKey = key;
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
