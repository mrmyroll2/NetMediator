using Microsoft.Extensions.DependencyInjection.Extensions;
using NetMediator;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNetMediator(this IServiceCollection services, params Assembly[] moduleAssemblies)
    {
        foreach (var assembly in moduleAssemblies)
        {
            var handlerInterfaceType = typeof(IRequestHandler<,>);

            var handlerTypes = assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .SelectMany(type => type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                    .Select(i => new { Interface = i, Implementation = type }));

            foreach (var handler in handlerTypes)
            {
                if (!services.Any(s => s.ServiceType == handler.Interface && s.ImplementationType == handler.Implementation))
                {
                    services.AddScoped(handler.Interface, handler.Implementation);
                }
            }
        }

        services.TryAddScoped<ISender, Sender>();

        return services;
    }
}

