var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseWebSockets(); // Enable WebSocket middleware
app.MapControllers();

app.Run();