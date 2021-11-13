using System.Net;

namespace chanosBot.API
{
    public class APIResponse
    {
        public string Result { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }

}
