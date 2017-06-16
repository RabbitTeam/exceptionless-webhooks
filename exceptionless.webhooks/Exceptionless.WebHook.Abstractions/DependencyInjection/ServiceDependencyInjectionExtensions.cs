using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Exceptionless.WebHook.Abstractions.DependencyInjection
{
    public static class ServiceDependencyInjectionExtensions
    {
        public static IServiceCollection AddServiceRegister(this IServiceCollection services, Func<string, bool> func)
        {
            var context = DependencyContext.Default;
            var assemblies = context
                .RuntimeLibraries
                .Where(i => func(i.Name))
                .Select(i => Assembly.Load(new AssemblyName(i.Name)))
                .ToArray();

            return services.AddServiceRegister(assemblies);
        }

        public static IServiceCollection AddServiceRegister(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            //获取程序集中所有实现IServiceRegister接口的类型
            var types = assemblies.SelectMany(i => i.GetExportedTypes())
                .Where(i =>
                {
                    var info = i.GetTypeInfo();
                    return info.IsClass && !info.IsAbstract && typeof(IServiceRegister).IsAssignableFrom(i);
                });

            foreach (var type in types)
            {
                var constructor = type.GetConstructor(new Type[0]);
                var instance = (IServiceRegister)constructor.Invoke(new object[0]);
                instance.Register(services);
            }

            return services;
        }

        public static IServiceCollection AddInterfaceDependency(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            //获取程序集中所有实现IDependency接口的类型
            var types = assemblies.SelectMany(i => i.GetExportedTypes())
                .Where(i =>
                {
                    var info = i.GetTypeInfo();
                    return info.IsClass && !info.IsAbstract && typeof(IDependency).IsAssignableFrom(i);
                });

            return services.AddInterfaceDependency(types);
        }

        public static IServiceCollection AddInterfaceDependency(this IServiceCollection services, IEnumerable<Type> types)
        {
            foreach (var type in types)
                RegisterDependency(type, services);

            return services;
        }


        public static IServiceCollection AddInterfaceDependency(this IServiceCollection services, Func<string, bool> func)
        {
            var context = DependencyContext.Default;

            Console.WriteLine(context
                .RuntimeLibraries.Count);
            foreach (var contextRuntimeLibrary in context
                .RuntimeLibraries)
            {
                Console.WriteLine(contextRuntimeLibrary.Name);
            }
            var assemblies = context
                .RuntimeLibraries
                .Where(i => func(i.Name))
                .Select(i => Assembly.Load(new AssemblyName(i.Name)))
                .ToArray();

            //获取程序集中所有实现IDependency接口的类型
            var types = assemblies.SelectMany(i => i.GetExportedTypes())
                .Where(i =>
                {
                    var info = i.GetTypeInfo();
                    return info.IsClass && !info.IsAbstract && typeof(IDependency).IsAssignableFrom(i);
                });

            foreach (var type in types)
                RegisterDependency(type, services);

            return services;
        }

        #region Private Method

        private static void RegisterDependency(Type type, IServiceCollection services)
        {
            foreach (var serviceDescriptor in GetServiceDescriptors(type))
            {
                if (serviceDescriptor != null)
                    services.Add(serviceDescriptor);
            }
        }

        private static IEnumerable<ServiceDescriptor> GetServiceDescriptors(Type implementationType)
        {
            var interfaces = implementationType
                .GetInterfaces()
                .Where(i => typeof(IDependency).IsAssignableFrom(i))
                .ToArray();

            //没有任何标识依赖的接口
            if (!interfaces.Any())
                yield break;

            //处理类本身实现的接口
            var baseDependency = interfaces.FirstOrDefault();
            var defaultDependencys = new[] { typeof(IDependency), typeof(ISingletonDependency), typeof(ITransientDependency) };
            if (defaultDependencys.Contains(baseDependency))
                yield return GetServiceDescriptor(implementationType, implementationType);

            //处理其它服务类型
            foreach (var interfaceType in interfaces.Where(i => !defaultDependencys.Contains(i)))
            {
                yield return GetServiceDescriptor(interfaceType, implementationType);
            }
        }

        private static ServiceDescriptor GetServiceDescriptor(Type serviceType, Type implementationType)
        {
            ServiceDescriptor descriptor = null;
            if (typeof(ISingletonDependency).IsAssignableFrom(serviceType))
            {
                descriptor = ServiceDescriptor.Singleton(serviceType, implementationType);
            }
            else if (typeof(ITransientDependency).IsAssignableFrom(serviceType))
            {
                descriptor = ServiceDescriptor.Transient(serviceType, implementationType);
            }
            else if (typeof(IDependency).IsAssignableFrom(serviceType))
            {
                descriptor = ServiceDescriptor.Scoped(serviceType, implementationType);
            }
            return descriptor;
        }

        #endregion Private Method
    }
}