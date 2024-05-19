using System.Globalization;
using System.Text.Json.Serialization;
using Chat.Context;
using Chat.Models;
using Chat.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChatContext>(options => options.UseNpgsql(connection))
    .AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true; // Требование к минимум 1 цифре
        options.Password.RequireLowercase = true; // Требование к минимум 1 букве нижнего регистра
        options.Password.RequireUppercase = true; // Требование к минимум 1 букве верхнего регистра
        options.Password.RequiredLength = 6; // Минимальная длина пароля
    })

    .AddEntityFrameworkStores<ChatContext>();;

builder.Services.AddScoped<IdentityErrorDescriber, RuLocalizationDescriber>();
ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("ru");


var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await AdminInitializer.SeedAdminUser(userManager, roleManager);
}
catch (Exception exception)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(exception, "Произошла ошибка при добавлений администратора");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();