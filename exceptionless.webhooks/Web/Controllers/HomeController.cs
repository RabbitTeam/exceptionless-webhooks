using Exceptionless.WebHook.Abstractions;
using Exceptionless.WebHook.DingTalk.Messages;
using Exceptionless.WebHook.DingTalk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly MessageService _messageService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MessageService messageService, ILogger<HomeController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        public async Task<string> Index(string accessToken)
        {
            var buffer = new byte[(int)Request.ContentLength];
            Request.Body.Read(buffer, 0, buffer.Length);
            var model = JsonConvert.DeserializeObject<ExceptionlessEventModel>(Encoding.UTF8.GetString(buffer));
            _logger.LogInformation(JsonConvert.SerializeObject(model));

            await _messageService.SendAsync(
                new Uri($"https://oapi.dingtalk.com/robot/send?access_token={accessToken}"),
                new DingTalkMessage
                {
                    Data = new LinkMessage($"{model.OrganizationName}_{model.ProjectName}_{model.Type}", model.OccurrenceDate.ToString(), model.Url),
                    Type = MessageType.Link
                });
            return "";
        }
    }
}