using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Tazmania.Mobile.Services;
using Tazmania.Mobile.ViewModels;
using Tazmania.Mobile.Views;
using UraniumUI;

namespace Tazmania.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                   .ConfigureFonts(fonts =>
                   {
                       fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                       fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                       fonts.AddMaterialIconFonts();
                   })
                   .UseUraniumUI()
                   .UseUraniumUIMaterial();

            builder.Services.AddTransient<IOView>();
            builder.Services.AddTransient<IOGateView>();
            builder.Services.AddTransient<SecurityView>();
            builder.Services.AddTransient<IrrigationView>();
            builder.Services.AddTransient<HeatingView>();
            builder.Services.AddTransient<SettingView>();

            builder.Services.AddTransient<IOViewModel>();
            builder.Services.AddTransient<IOGateViewModel>();
            builder.Services.AddTransient<SecurityViewModel>();
            builder.Services.AddTransient<IrrigationViewModel>();
            builder.Services.AddTransient<HeatingViewModel>();
            builder.Services.AddTransient<SettingViewModel>();
            
            builder.Services.AddSingleton<IORestService>();
            builder.Services.AddSingleton<HeatingRestService>();
            builder.Services.AddSingleton<SecurityRestService>();
            builder.Services.AddSingleton<IrrigationRestService>();
            builder.Services.AddSingleton<SettingRestService>();
            builder.Services.AddSingleton<AuthenticationService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}