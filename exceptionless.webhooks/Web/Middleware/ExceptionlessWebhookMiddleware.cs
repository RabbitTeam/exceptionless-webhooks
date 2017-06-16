using Exceptionless.WebHook.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Middleware
{
    public class ExceptionlessWebhookMiddleware
    {
        #region Field

        private readonly RequestDelegate _next;
        private readonly IEnumerable<IWebHookProvider> _webHookProviders;
        private readonly ILogger<ExceptionlessWebhookMiddleware> _logger;

        #endregion Field

        #region Constructor

        public ExceptionlessWebhookMiddleware(RequestDelegate next, IEnumerable<IWebHookProvider> webHookProviders, ILogger<ExceptionlessWebhookMiddleware> logger)
        {
            _next = next;
            _webHookProviders = webHookProviders;
            _logger = logger;
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"全部处理程序数量：{_webHookProviders.Count()}");
        }

        #endregion Constructor

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;

            if (request.Method != HttpMethods.Post)
            {
                await _next(context);
                return;
            }

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("接收到 Webhook 请求。");

            try
            {
                string content;
                using (var reader = new StreamReader(request.Body))
                    content = await reader.ReadToEndAsync();

                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug($"接收到 Webhook 请求：{content}");

                var model = JsonConvert.DeserializeObject<ExceptionlessEventModel>(content);

                var providers = GetProviders(request.Query);

                var parameters = new Dictionary<string, string>(request.Query.ToDictionary(i => i.Key, i => i.Value.ToString()), StringComparer.OrdinalIgnoreCase);
                foreach (var provider in providers)
                {
                    try
                    {
                        _logger.LogDebug($"使用{provider.Name}处理中");
                        await provider.ProcessAsync(model, parameters);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(0, e, $"处理 {provider.Name} Webhook 时发生了异常。");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "处理 Webhook 失败。");
                throw;
            }
        }

        #region Private Method

        private IEnumerable<IWebHookProvider> GetProviders(IQueryCollection query)
        {
            var providers = _webHookProviders;
            if (!query.TryGetValue("webhooks", out StringValues webhooks))
                return providers;

            var useHooks = webhooks.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            providers = _webHookProviders.Where(
                i => useHooks.Any(z => string.Equals(z, i.Name, StringComparison.Ordinal)));
            return providers;
        }

        #endregion Private Method
    }
}