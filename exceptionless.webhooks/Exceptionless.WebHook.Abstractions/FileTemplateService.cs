using Exceptionless.WebHook.Abstractions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exceptionless.WebHook.Abstractions
{
    public class FileTemplateService : ISingletonDependency, IDisposable
    {
        #region Field

        private readonly IMemoryCache _memoryCache;
        private readonly PhysicalFileProvider _physicalFileProvider;

        #endregion Field

        #region Constructor

        public FileTemplateService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _physicalFileProvider = new PhysicalFileProvider(AppContext.BaseDirectory);
        }

        #endregion Constructor

        #region Public Method

        public async Task<string> GetContent(string templateFile, object model)
        {
            var template = await GetTemplate(templateFile);
            if (string.IsNullOrEmpty(template))
                return template;

            var properties = model.GetType().GetProperties();
            var builder = new StringBuilder(template);
            foreach (var property in properties)
            {
                var key = "{" + property.Name + "}";
                if (template.Contains(key))
                    builder.Replace(key, property.GetValue(model).ToString());
            }
            return builder.ToString();
        }

        #endregion Public Method

        #region Private Method

        private async Task<string> GetTemplate(string templateFile)
        {
            return await _memoryCache.GetOrCreateAsync(templateFile, async entry =>
            {
                entry.AddExpirationToken(_physicalFileProvider.Watch(templateFile));
                var fileInfo = _physicalFileProvider.GetFileInfo(templateFile);
                if (!fileInfo.Exists)
                    return string.Empty;

                using (var reader = new StreamReader(fileInfo.CreateReadStream()))
                {
                    return await reader.ReadToEndAsync();
                }
            });
        }

        #endregion Private Method

        #region IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _memoryCache?.Dispose();
            _physicalFileProvider?.Dispose();
        }

        #endregion IDisposable
    }
}