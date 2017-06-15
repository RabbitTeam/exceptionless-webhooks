using Exceptionless.WebHook.DingTalk.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Exceptionless.WebHook.DingTalk
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDingTalkWebHook(this IServiceCollection services)
        {
            return services.AddSingleton<MessageService, MessageService>();
        }
    }
}