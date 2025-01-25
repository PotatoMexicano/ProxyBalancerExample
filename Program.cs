using Serilog;
using Spectre.Console;

namespace ReverseProxyLoadBalance;

public class Program
{
    public static void Main(String[] args)
    {
        MensagemInicial();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:6000");

        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        builder.Host.UseSerilog();

        builder.Services.AddControllers();

        WebApplication app = builder.Build();

        app.UseHttpsRedirection();

        app.MapReverseProxy();

        app.Run();

    }

    public static void MensagemInicial()
    {
        AnsiConsole.Markup("[red]  ____        _                           [/][cyan]   ____                      [/]\r\n[red] | __ )  __ _| | __ _ _ __   ___ ___ _ __ [/][cyan]  |  _ \\ _ __ _____  ___   _ [/]\r\n[red] |  _ \\ / _` | |/ _` | '_ \\ / __/ _ \\ '__|[/][cyan]  | |_) | '__/ _ \\ \\/ / | | |[/]\r\n[red] | |_) | (_| | | (_| | | | | (_|  __/ |   [/][cyan]  |  __/| | | (_) >  <| |_| |[/]\r\n[red] |____/ \\__,_|_|\\__,_|_| |_|\\___\\___|_| [white]and[/] [/][cyan]|_|   |_|  \\___/_/\\_\\\\__, |[/]\r\n[red]  [/][cyan]\t\t\t\t\t\t\t          |___/ [/]\r\n");
        AnsiConsole.MarkupLine("By: Potato\r\n");
    }
}
