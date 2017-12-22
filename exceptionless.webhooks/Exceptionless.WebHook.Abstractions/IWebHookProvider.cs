using Rabbit.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Exceptionless.WebHook.Abstractions
{
    public interface IWebHookProvider : ISingletonDependency
    {
        string Name { get; }

        Task ProcessAsync(ExceptionlessEventModel model, IDictionary<string, string> parameters);
    }
}