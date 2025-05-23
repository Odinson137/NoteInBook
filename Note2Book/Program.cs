using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Hubs;
using Note2Book.Interfaces;
using Note2Book.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();


services.AddDbContext<DataContext>(option => option.UseMySql(
        configuration["MySql:ConnectionString"],
        new MySqlServerVersion(new Version(8, 0, 11))
    )
);

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/User/AccessDenied";
    });
services.Configure<ElasticSearchSettings>(configuration.GetSection("ElasticSearchSettings"));
    
services.AddAutoMapper(typeof(AutoMapperProfile));
services.AddScoped<IElasticService, ElasticService>();
services.AddScoped<IBookElasticService, BookElasticService>();
services.AddScoped<IActivityService, ActivityService>();
services.AddScoped<Seed>();
services.AddSignalR();


var app = builder.Build();

await app.Services.CreateScope().ServiceProvider.GetRequiredService<Seed>().Seeding();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapHub<ForumHub>("/forumHub");

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = new FileExtensionContentTypeProvider
    {
        Mappings =
        {
            // Добавляем поддержку EPUB
            [".epub"] = "application/epub+zip"
        }
    }
});

app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();