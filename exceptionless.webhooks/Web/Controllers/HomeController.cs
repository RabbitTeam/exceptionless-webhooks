using Exceptionless.WebHook.Abstractions;
using Exceptionless.WebHook.DingTalk.Messages;
using Exceptionless.WebHook.DingTalk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly MessageService _messageService;
        private readonly FileTemplateService _markdownTemplateService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MessageService messageService, FileTemplateService markdownTemplateService, ILogger<HomeController> logger)
        {
            _messageService = messageService;
            _markdownTemplateService = markdownTemplateService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<string> Index(string accessToken)
        {
            string json;
            using (var reader = new StreamReader(Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }
            var model = JsonConvert.DeserializeObject<ExceptionlessEventModel>(json);
            _logger.LogInformation(JsonConvert.SerializeObject(model));

            var content = await _markdownTemplateService.GetContent("markdownTemplate.md", model);

            await _messageService.SendAsync(
            new Uri($"https://oapi.dingtalk.com/robot/send?access_token={accessToken}"),
            new DingTalkRequestMessage
            {
                Data = new MarkdownDingTalkMessage("Exceptionless", content)
            });
            return "";
        }
    }
}