using Microsoft.Extensions.DependencyInjection;

namespace Exceptionless.WebHook.Abstractions.DependencyInjection
{
    public interface IServiceRegister
    {
        void Register(IServiceCollection services);
    }
}