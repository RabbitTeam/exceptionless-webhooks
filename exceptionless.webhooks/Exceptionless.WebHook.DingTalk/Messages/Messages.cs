using Newtonsoft.Json;

namespace Exceptionless.WebHook.DingTalk.Messages
{
    public abstract class DingTalkMessageBase
    {
        public abstract MessageType Type { get; }

        protected abstract string ToContentJson();
    }

    public class MarkdownDingTalkMessage : DingTalkMessageBase
    {
        public MarkdownDingTalkMessage(string title, string text)
        {
            Title = title;
            Text = text;
        }

        public MarkdownDingTalkMessage()
        {
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        #region Overrides of DingTalkMessageBase

        public override MessageType Type { get; } = MessageType.Markdown;

        protected override string ToContentJson()
        {
            return JsonConvert.SerializeObject(new
            {
                title = Title,
                text = Text
            });
        }

        #endregion Overrides of DingTalkMessageBase
    }

    public class ActionCardDingTalkMessage : DingTalkMessageBase
    {
        public ActionCardDingTalkMessage(string title, string text, string singleTitle, string singleUrl, bool btnOrientation = false, bool hideAvatar = false)
        {
            Title = title;
            Text = text;
            SingleTitle = singleTitle;
            SingleUrl = singleUrl;
            BtnOrientation = btnOrientation;
            HideAvatar = hideAvatar;
        }

        public ActionCardDingTalkMessage()
        {
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("singleTitle")]
        public string SingleTitle { get; set; }

        [JsonProperty("singleURL")]
        public string SingleUrl { get; set; }

        [JsonProperty("btnOrientation")]
        public bool BtnOrientation { get; set; }

        [JsonProperty("hideAvatar")]
        public bool HideAvatar { get; set; }

        #region Overrides of DingTalkMessageBase

        public override MessageType Type { get; } = MessageType.ActionCard;

        protected override string ToContentJson()
        {
            return JsonConvert.SerializeObject(new
            {
                title = Title,
                text = Text,
                singleTitle = SingleTitle,
                singleUrl = SingleUrl,
                btnOrientation = BtnOrientation ? 1 : 0,
                hideAvatar = HideAvatar ? 1 : 0
            });
        }

        #endregion Overrides of DingTalkMessageBase
    }

    public class LinkMessageDingTalkMessage : DingTalkMessageBase
    {
        public LinkMessageDingTalkMessage(string title, string text, string messageUrl, string picUrl = null)
        {
            Title = title;
            Text = text;
            MessageUrl = messageUrl;
            PicUrl = picUrl;
        }

        public LinkMessageDingTalkMessage()
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

        #region Overrides of DingTalkMessageBase

        public override MessageType Type { get; } = MessageType.Link;

        protected override string ToContentJson()
        {
            return JsonConvert.SerializeObject(new
            {
                title = Title,
                text = Text,
                messageUrl = MessageUrl,
                picUrl = PicUrl
            });
        }

        #endregion Overrides of DingTalkMessageBase
    }
}