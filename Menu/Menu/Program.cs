using Menu.Contexts;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MenuContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnectionString"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnectionString"))
        
    )
);

        

var app = builder.Build();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
Console.WriteLine($"Connection String: {connectionString}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();