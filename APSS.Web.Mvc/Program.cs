using Microsoft.EntityFrameworkCore;
using APSS.Infrastructure.Repositores.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

#region Services

// Database context
builder.Services.AddDbContext<ApssDbContext>(cfg =>
    cfg.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"])
);

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