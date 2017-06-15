using Newtonsoft.Json;

namespace Exceptionless.WebHook.DingTalk.Messages
{
    public enum MessageType
    {
        Link
    }

    public class DingTalkMessage
    {
        public MessageType Type { get; set; }
        public object Data { get; set; }
    }

    public class LinkMessage
    {
        public LinkMessage(string title, string text, string messageUrl, string picUrl = null)
        {
            Title = title;
            Text = text;
            MessageUrl = messageUrl;
            PicUrl = picUrl;
        }

        public LinkMessage()
        {
        }

        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("messageUrl")]
        public string MessageUrl { get; set; }
        [JsonProperty("picUrl")]
        public string PicUrl { get; set; }
    }
}