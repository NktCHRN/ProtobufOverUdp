using Microsoft.Extensions.DependencyInjection;

namespace Listener.Containers;

public sealed class SupportedTypesContainer : ISupportedTypesContainer
{
    public IReadOnlyList<Type> SupportedTypes => _supportedTypes;
    private readonly Type[] _supportedTypes;

    public SupportedTypesContainer(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IUdpMessageHandler>();
        var supportedTypesHashSet = new HashSet<Type>();
        foreach (var handler in handlers)
        {
            var @interface = handler.GetType()
                .GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IUdpMessageHandler<>));

            if (@interface == null)
            {
                continue;
            }
            
            var type = @interface.GetGenericArguments()[0];

            _ = supportedTypesHashSet.Add(type);
        }
        
        _supportedTypes = new Type[supportedTypesHashSet.Count];
        supportedTypesHashSet.CopyTo(_supportedTypes, 0);
    }
}
