using Newtonsoft.Json;

namespace Exceptionless.WebHook.DingTalk.Messages
{
    public class DingTalkResponseMessage
    {
        [JsonProperty("errmsg")]
        public string Message { get; set; }

        [JsonProperty("errcode")]
        public int ErrorCode { get; set; }
    }
}