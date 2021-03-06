using APSS.Application.App;
using APSS.Domain.Services;
using APSS.Tests.Infrastructure.Repositories.EntityFramework.Util;

using Microsoft.Extensions.DependencyInjection;

namespace APSS.Tests.Application.App;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped(_ => TestUnitOfWork.Create());

        services.AddTransient<IAccountsService, AccountsService>();
        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<ILogsService, DatabaseLogsService>();
        services.AddTransient<IPermissionsService, PermissionsService>();
        services.AddTransient<IPopulationService, PopulationService>();
        services.AddTransient<ISurveysService, SurveysService>();
        services.AddTransient<ILandService, LandService>();
        services.AddTransient<IAnimalService, AnimalService>();
    }
}