using TradeForge.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

{
    builder.Services.AddSingleton<IWsHubService, WsHubService>();
    builder.Services.AddSingleton<WsMessageChannel>();
    builder.Services.AddHostedService<WsMessageService>();
}

var app = builder.Build();

app.UseWebSockets(); // Enable WebSocket middleware
app.MapControllers();

app.Run();