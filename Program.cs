using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReverseProxyLoadBalance.Context;
using ReverseProxyLoadBalance.Entities;
using ReverseProxyLoadBalance.Implements;
using ReverseProxyLoadBalance.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Spectre.Console;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReverseProxyLoadBalance;

public class Program
{
    public static void Main(String[] args)
    {
        MensagemInicial();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseUrls("http://localhost:5000");

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .Build();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
             .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware", LogEventLevel.Warning)
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .Enrich.WithExceptionDetails()
            .CreateLogger();

        builder.Host.UseSerilog(Log.Logger);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("InMemoryDb"));

        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        Byte[] secretJwtToken = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtToken:Secret").Value ?? String.Empty);
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretJwtToken),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddTransient<IPasswordHasher<User>, BCryptPasswordHasher>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.RespectNullableAnnotations = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            });

        builder.Services.AddAuthorization();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        WebApplication app = builder.Build();

        //app.UseSerilogRequestLogging();

        app.UseAuthentication();
        app.UseAuthorization();

        //app.UseHttpsRedirection();

        app.MapReverseProxy();

        if (app.Environment.IsDevelopment())
        {
            app.SeedDatabase();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        try
        {
            app.Run();
        }
        finally
        {
            Log.Information("Server shutting down...");
            Log.CloseAndFlush();
        }

    }

    public static void MensagemInicial()
    {
        AnsiConsole.Markup("[red]  ____        _                           [/][cyan]   ____                      [/]\r\n[red] | __ )  __ _| | __ _ _ __   ___ ___ _ __ [/][cyan]  |  _ \\ _ __ _____  ___   _ [/]\r\n[red] |  _ \\ / _` | |/ _` | '_ \\ / __/ _ \\ '__|[/][cyan]  | |_) | '__/ _ \\ \\/ / | | |[/]\r\n[red] | |_) | (_| | | (_| | | | | (_|  __/ |   [/][cyan]  |  __/| | | (_) >  <| |_| |[/]\r\n[red] |____/ \\__,_|_|\\__,_|_| |_|\\___\\___|_| [white]and[/] [/][cyan]|_|   |_|  \\___/_/\\_\\\\__, |[/]\r\n[red]  [/][cyan]\t\t\t\t\t\t\t          |___/ [/]\r\n");
        AnsiConsole.MarkupLine("By: Potato\r\n");
    }
}
