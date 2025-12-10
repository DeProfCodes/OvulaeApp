using CommunityToolkit.Maui;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.Jobs;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.ChatBot;
using OvulaeApp.Services.LocalDataService.DietServices;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.Services.LocalDataService.MenopauseServices;
using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeApp.Services.LocalDataService.SymptomsServices;
using OvulaeApp.Services.LocalDataService.TipsServices;
using OvulaeApp.Services.LocalDataService.Updates;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Services.Notifications;
using OvulaeShared.Services.APIs.Affiliates;
using OvulaeShared.Services.APIs.Authentication;
using OvulaeShared.Services.APIs.Messaging;
using OvulaeShared.Services.APIs.ModuleServices;
using OvulaeShared.Services.APIs.Users;
using OvulaeShared.Services.Email;
using OvulaeShared.Services.Payments.Paystack;
using Plugin.LocalNotification;
using Shiny;
using Shiny.Jobs;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SkiaSharp.Views.Maui.Handlers;
using OvulaeApp.Services;
using OvulaeShared.Services.APIs.Payments;
using OvulaeApp.Services.LocalDataService.Subscription;
using OvulaeShared.Services.APIs.Interface;
using OvulaeApp.Services.Payments;
using Microsoft.Maui.LifecycleEvents;
using OvulaeShared.Services.Module.OvulationServices;
using OvulaeShared.Services.Module.PeriodTrackerServices;
using OvulaeShared.Services.Module.CycleServices;
using OneSignalSDK.DotNet;



#if ANDROID
using Microsoft.Maui.Handlers;
using Android.Content.Res;
//using Plugin.Firebase.Core.Platforms.Android;
#endif

#if IOS
using UIKit;
using OvulaeApp.Services.Payments;
using Microsoft.Maui.Handlers;

#endif

namespace OvulaeApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Fredoka-Regular.ttf", "Fredoka");
                    fonts.AddFont("Fredoka-Bold.ttf", "FredokaBold");
                    fonts.AddFont("Chewy-Regular.ttf", "ChewyFont");
                });

            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<GifLoaderView, SKCanvasViewHandler>();
            });


            builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android =>
                {
                    android.OnCreate((activity, bundle) =>
                    {
                        //CrossFirebase.Initialize(activity);
                    });
                });
#elif IOS
            
#endif
            });


            //Jobs
            
            /*
            builder.Services.UseJobs();
            builder.Services.RegisterJob<DailyCycleJob>(...);
            */

            //Services
            //API
            builder.Services.AddSingleton<IAuthenticationApi, AuthenticationApi>();
            builder.Services.AddSingleton<IMessagingApi, MessagingApi>();
            builder.Services.AddSingleton<IUsersApi, UsersApi>();
            builder.Services.AddSingleton<IOvulaeEmailService, OvulaeEmailService>();
            builder.Services.AddSingleton<IAffiliatesApi, AffiliatesApi>();
            builder.Services.AddSingleton<IPaystackApi, PaystackApi>();
            builder.Services.AddSingleton<IModuleLogsApi, ModuleLogsApi>();
            builder.Services.AddSingleton<IPaymentsApi, PaymentsApi>();
            builder.Services.AddSingleton<IWebInterfaceApiService, WebInterfaceApiService>();

            //Local
            builder.Services.AddSingleton<ILocalDbService, LocalDbService>();
            builder.Services.AddSingleton<IUpdatesService, UpdatesService>(); 
            builder.Services.AddSingleton<IPregnancyService, PregnancyService>();
            builder.Services.AddSingleton<IDietService, DietService>();
            builder.Services.AddSingleton<IEducationService, EducationService>();
            builder.Services.AddSingleton<ITipsService, TipsService>();
            builder.Services.AddSingleton<ISymptomsService, SymptomsService>();
            builder.Services.AddSingleton<IOvulationService, OvulationService>();
            builder.Services.AddSingleton<IPeriodTrackerService, PeriodTrackerService>();
            builder.Services.AddSingleton<IMenopauseService, MenopauseService>();
            builder.Services.AddSingleton<IUserLocalService, UserLocalService>();
            builder.Services.AddSingleton<IChatBotService, ChatBotService>();
            builder.Services.AddSingleton<ICycleService, CycleService>();
            builder.Services.AddSingleton<IModuleLogsService, ModuleLogsService>();
            builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();

            builder.Services.AddSingleton<IOneSignalNotificationService, OneSignalNotificationService>();
            //ios
            builder.Services.AddSingleton<ISubscriptionPaymentService, SubscriptionPaymentService>();
#if IOS
            builder.Services.AddSingleton<IInAppPurchaseService, InAppPurchaseService>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if ANDROID

            builder.Services.AddLogging(configure =>
            {
                configure.AddDebug(); // Logs to Debug output
                configure.SetMinimumLevel(LogLevel.Debug);
            });

            EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            });

            // 🔥 Remove underline from Picker
            PickerHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            });

            EditorHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            });
#endif

#if IOS

            EntryHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
            {
                var nativeEntry = handler.PlatformView;
                nativeEntry.BorderStyle = UITextBorderStyle.None;
                nativeEntry.BackgroundColor = UIColor.Clear;
            });

            PickerHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
            {
                if (handler.PlatformView is UITextField textField)
                {
                    // Remove border
                    textField.BorderStyle = UITextBorderStyle.None;
            
                    // Remove background and shadow
                    textField.BackgroundColor = UIColor.Clear;
                    textField.Layer.BorderWidth = 0;
                    textField.Layer.BorderColor = UIColor.Clear.CGColor;
                    textField.Layer.ShadowOpacity = 0;
                }
            });

            DatePickerHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
            {
                if (handler.PlatformView is UITextField textField)
                {
                    textField.BorderStyle = UITextBorderStyle.None;
                    textField.BackgroundColor = UIColor.Clear;
                    textField.Layer.BorderWidth = 0;
                    textField.Layer.BorderColor = UIColor.Clear.CGColor;
                    textField.Layer.ShadowOpacity = 0;
                }
            });

            ScrollViewHandler.Mapper.AppendToMapping("DismissOnDrag", (handler, view) =>
            {
                if (handler.PlatformView is UIScrollView sv)
                    sv.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;
            });

#endif



            //AppCenter.Start("android=53bb380d-8764-4de3-a700-c9aa2d253141;",
            //        typeof(Analytics), typeof(Crashes));
            /*
#if ANDROID
            AppCenter.Start("android=53bb380d-8764-4de3-a700-c9aa2d253141", typeof(Analytics), typeof(Crashes));
#elif IOS
            AppCenter.Start("ios=218da0f5-c1d7-42ba-afa8-33ac30cc1903", typeof(Analytics), typeof(Crashes));
#endif
            */
            /*
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception ex = e.ExceptionObject as Exception ?? new Exception(e.ExceptionObject.ToString());
                //Crashes.TrackError(ex, new Dictionary<string, string> { { "Type", "UnhandledException" } });
                Console.WriteLine($"$Root MAUI - > Unhandled Exception: {ex}");
            };




            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                //Crashes.TrackError(e.Exception, new Dictionary<string, string> { { "Type", "UnobservedTaskException" } });
                e.SetObserved();
            };
            */

            OneSignal.Initialize("740b7148-dfdb-4ad6-964b-e4414b304e41");

            var app = builder.Build();

            // Set the service provider globally
            AppServiceHelper.Services = app.Services;
            ServiceHelper.ServiceProvider = app.Services;

            return app;
        }

        //static partial void Registerplatformhandlers(MauiAppBuilder builder)
    }
}


//dotnet publish -f net9.0-android -c Release
