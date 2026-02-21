using Serilog;
using TradeForge.Backend.Data.Extensions;
using TradeForge.Backend.Middleware;
using TradeForge.Backend.Services.Ws;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddWsActionHandlers(typeof(Program).Assembly);
{
    builder.Services.AddSingleton<IWsErrorSender, WsErrorSender>();
    builder.Services.AddSingleton<IWsHubService, WsHubService>();
    builder.Services.AddSingleton<WsMessageChannel>();
    builder.Services.AddHostedService<WsMessageService>();
}

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseWebSockets(); // Enable WebSocket middleware
app.MapControllers();

try
{
    app.Run();
}
finally
{
    await Log.CloseAndFlushAsync();
}