using Exceptionless.WebHook.Abstractions.DependencyInjection;
using Exceptionless.WebHook.DingTalk.Messages;
using Exceptionless.WebHook.DingTalk.Utilitys;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Exceptionless.WebHook.DingTalk.Services
{
    public class MessageService : ISingletonDependency
    {
        private readonly ILogger<MessageService> _logger;

        public MessageService(ILogger<MessageService> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(Uri url, DingTalkRequestMessage message)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"准备向 {url} 发送数据");

            var type = StringUtilitys.ToCamelCase(message.Type.ToString());
            var data = new Dictionary<string, object>
            {
                {"msgtype", type},
                {type, message.Data}
            };

            var json = JsonConvert.SerializeObject(data);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"send dingtalk json:{json}");

            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                    var responseContent = await result.Content.ReadAsStringAsync();
                    _logger.LogDebug(responseContent);

                    var response = JsonConvert.DeserializeObject<DingTalkResponseMessage>(responseContent);
                    if (response.ErrorCode > 0)
                        _logger.LogError($"钉钉返回错误消息：{responseContent}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, $"向钉钉发送消息时失败，发送的数据：{json}。");
            }
        }
    }
}