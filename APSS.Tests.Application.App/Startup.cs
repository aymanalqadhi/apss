using APSS.Tests.Infrastructure.Repositories.EntityFramework.Util;

using Microsoft.Extensions.DependencyInjection;

namespace APSS.Tests.Application.App;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient(_ => TestUnitOfWork.Create());
    }
}
