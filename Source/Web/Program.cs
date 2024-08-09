using Application.Actors;
using Application.Interfaces;
using Domain.DefaultImplementations;
using Domain.Entities;
using Domain.Interfaces;
using EntityStorageServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Json;
using Web.Middlewares;
using User = Application.Actors.User;

namespace Web;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        await TestBuild();
    }

    private static async Task TestBuild()
    {
        var builder = WebApplication.CreateBuilder();
        
        builder.Services.AddControllersWithViews().AddNewtonsoftJson();

        builder.Services.AddScoped<IEntityStorageService<Driver>, EntityFrameworkEntityStorageService<Driver>>();
        builder.Services.AddScoped<IEntityStorageService<Truck>, EntityFrameworkEntityStorageService<Truck>>();
        builder.Services.AddScoped<IEntityStorageService<Domain.Entities.User>, EntityFrameworkEntityStorageService<Domain.Entities.User>>();
        builder.Services.AddScoped<IEntityStorageService<Branch>, EntityFrameworkEntityStorageService<Branch>>();
        builder.Services.AddScoped<IEntityStorageService<Order>, EntityFrameworkEntityStorageService<Order>>();
        builder.Services.AddScoped<ICryptographicService, DefaultCryptographicService>();
        builder.Services.AddScoped<IGeolocationService, DefaultGeolocationService>();
        builder.Services.AddScoped<Administrator>();
        builder.Services.AddDbContext<TransportCompanyContext>((serviceProvider, options) =>
        {
            var connectionString = serviceProvider.GetService<IConfiguration>()!.GetConnectionString("MySql")!;

            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
        
        var app = builder.Build();

        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Index}/{id?}");

        await app.RunAsync();
    }
    
    /*private static async Task RealeseBuild()
    {
        Log.Logger = new LoggerConfiguration()
            //.MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
            
            .WriteTo.Async(a => a.File(new JsonFormatter(), @"Logs\TransportCompanyRemake.log", retainedFileCountLimit: 4, rollOnFileSizeLimit: true, fileSizeLimitBytes: 5_368_709_120))// По умолчанию в файлы логируются обычные сообщения, но это можно исправить, передав в качестве первого параметра один из трех форматтеров - JsonFormatter, CompactJsonFormatter или RenderedCompactJsonFormatter - сравни результаты каждого из них в одном контексте
            
            .CreateLogger();
        
        try
        {
            Log.Information("Starting host build");
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddSerilog();
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(b =>// Этот метод глобально конфигурирует все авторизационные Cookie. Если в момент выдачи авторизационного Cookie клиенту для него не будет явно сконфигурирован какой-нибудь аспект с помощью AuthenticationProperties, то для этого аспекта будет принята глобальная конфигурация именно отсюда. Немного кастомной терминологии для удобства: дата истечения Cookie - дата, когда срок действия самого Cookie истечет и клиент его удалит; дата истечения авторизации Cookie - дата, после которой будет невозможно авторизоваться с помощью этого Cookie. Вывод из всего этого: Cookie может перестать быть авторизационным "пропуском" и стать, по сути, кучкой бесполезных байтов, но при этом он может все еще храниться на клиенте и продолжать приходить на сервер
                {
                    //b.SlidingExpiration = true;// По умолчанию true. Если true, то фреймворк автоматически сам будет обновлять срок действия авторизации всех получаемых Cookie, если он истек наполовину или больше. Если false, то срок действия авторизации Cookie обновляться не будет. Это свойство никак не влияет на Cookie.MaxAge (срок действия самого Cookie)
                    //b.ExpireTimeSpan = TimeSpan.FromMinutes(2);// Срок действия авторизации Cookie. По умолчанию он, вроде, "Сессионный" - юзер закрыл браузер, Cookie стал бесполезен
                    b.Cookie.MaxAge = TimeSpan.FromDays(30);// Срок действия самого Cookie. По умолчанию он, вроде, "Сессионный" - юзер закрыл браузер, Cookie удалился
                    //b.EventsType = typeof(SomethingEvent);// Вероятно, это свойство определяет обработчик, который будет вызываться после/перед события/событием "Создание Cookie"
                    
                });
            builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, RedirectAfterFailedAuthentication>();
            builder.Services.AddScoped<UserInteractor>(serviceProvider =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<TransportCompanyContext>();
                var connectionString = serviceProvider.GetService<IConfiguration>()!.GetConnectionString("MySql")!;

                var options = optionsBuilder.UseLazyLoadingProxies().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .Options;

                return new UserInteractor(new UserDao(new TransportCompanyContext(options), serviceProvider.GetRequiredService<ILogger<UserDao>>()), new OrderDao(new TransportCompanyContext(options)), new TruckDao(new TransportCompanyContext(options)));
            });
            builder.Services.AddDbContext<TransportCompanyContext>((serviceProvider, options) =>
            {
                var connectionString = serviceProvider.GetService<IConfiguration>()!.GetConnectionString("MySql")!;

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();
            
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionLoggingMiddleware>();

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");
            
            Log.Information("Success to build host. Starting web application");

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to build host");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }*/
}