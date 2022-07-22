using Microsoft.EntityFrameworkCore;
using APSS.Application.App;
using APSS.Domain.Repositories;
using APSS.Domain.Services;
using APSS.Infrastructure.Repositores.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

#region Services

var svc = builder.Services;

// Database context
svc.AddDbContext<ApssDbContext>(cfg =>
    cfg.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"])
);

// Unit-of-work
svc.AddScoped<IUnitOfWork, ApssUnitOfWork>();

// Services
svc.AddScoped<ILogsService, DatabaseLogsService>();
svc.AddScoped<IUsersService, UsersService>();
svc.AddScoped<IPermissionsService, PermissionsService>();
svc.AddScoped<IAccountsService, AccountsService>();
svc.AddScoped<IAnimalService, AnimalService>();
svc.AddScoped<ILandService, LandService>();
svc.AddScoped<IPopulationService, PopulationService>();
svc.AddScoped<ISurveysService, SurveysService>();

#endregion Services

var app = builder.Build();

// Environmen-dependent settings
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();