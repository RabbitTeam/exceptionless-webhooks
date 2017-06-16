using Exceptionless.WebHook.Abstractions;
using Exceptionless.WebHook.DingTalk.Messages;
using Exceptionless.WebHook.DingTalk.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Exceptionless.WebHook.DingTalk
{
    public class DingTalkWebHookProvider : IWebHookProvider
    {
        private readonly FileTemplateService _fileTemplateService;
        private readonly MessageService _messageService;

        public DingTalkWebHookProvider(FileTemplateService fileTemplateService, MessageService messageService)
        {
            _fileTemplateService = fileTemplateService;
            _messageService = messageService;
        }

        #region Implementation of IWebHookProvider

        public string Name { get; } = "DingTake";

        public async Task ProcessAsync(ExceptionlessEventModel model, IDictionary<string, string> parameters)
        {
            parameters.TryGetValue("accessToken", out string accessToken);
            var content = await _fileTemplateService.GetContent("markdownTemplate.md", model);

            await _messageService.SendAsync(
                new Uri($"https://oapi.dingtalk.com/robot/send?access_token={accessToken}"),
                new DingTalkRequestMessage
                {
                    Data = new MarkdownDingTalkMessage("Exceptionless 有新的事件", content)
                });
        }

        #endregion Implementation of IWebHookProvider
    }
}