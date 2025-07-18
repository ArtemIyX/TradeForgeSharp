using Microsoft.Extensions.Logging;
using TradeForge.Services;
using TradeForge.SymbolManager.Services.Impl;
using TradeForge.SymbolManager.Services.Interfaces;


namespace TradeForge
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#else
#endif

            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddScoped<ISymbolManager, SymbolManagerService>();
            
            var app = builder.Build();
            
            return app;
        }
    }
}