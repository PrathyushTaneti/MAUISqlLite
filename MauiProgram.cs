using MAUISqlLite.Data;
using MAUISqlLite.Pages;
using MAUISqlLite.ViewModels;
using Microsoft.Extensions.Logging;

namespace MAUISqlLite
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DbContext>();
            builder.Services.AddTransient<ProductsViewModel>();
            builder.Services.AddTransient<MainPage>();
            return builder.Build();
        }
    }
}
