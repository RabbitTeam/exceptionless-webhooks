namespace Exceptionless.WebHook.DingTalk.Messages
{
    public class DingTalkRequestMessage
    {
        public MessageType Type => Data.Type;
        public DingTalkMessageBase Data { get; set; }
    }
}