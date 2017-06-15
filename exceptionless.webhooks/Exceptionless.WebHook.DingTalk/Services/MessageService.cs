using Exceptionless.WebHook.DingTalk.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Exceptionless.WebHook.DingTalk.Services
{
    public class MessageService
    {
        private readonly ILogger<MessageService> _logger;

        public MessageService(ILogger<MessageService> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(Uri url, DingTalkMessage message)
        {
            var type = message.Type.ToString().ToLower();
            var data = new Dictionary<string, object>
            {
                {"msgtype", type},
                {type, message.Data}
            };
//            data.msgtype = type;
//            data[type] = message.Data;

            var json = JsonConvert.SerializeObject(data);
            using (var client = new HttpClient())
            {
//                client.DefaultRequestHeaders.Add("Content-Type", "application/json;charset=utf-8");
                var result = await client.PostAsync(url, new StringContent(json,Encoding.UTF8, "application/json"));
                var response = await result.Content.ReadAsStringAsync();
                _logger.LogInformation(response);
            }
        }
    }
}