var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddMvc();

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