using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class UdpServiceExtensions
{
    public static IServiceCollection AddUdpService(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddUdpService(configuration, _ => { });
    }
    
    public static IServiceCollection AddUdpService(this IServiceCollection services, IConfiguration configuration, Action<UdpServiceOptions> configure)
    {
        services.AddOptions<UdpServiceOptions>()
            .Configure(opt =>
            {
                opt.Port =
                    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("UDP_PORT"))
                        ? ushort.Parse(Environment.GetEnvironmentVariable("UDP_PORT")!)
                        : (ushort)new Uri((configuration["urls"]
                                           ?? configuration["ASPNETCORE_URLS"]!)
                            .Split(';', StringSplitOptions.RemoveEmptyEntries)[0]).Port;
                configure(opt);
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<IUdpService, UdpService>();
        
        return services;
    }
}
