using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Views.MenopauseTracker.Dashboard;
using OvulaeApp.Views.OvulationTracker.Dashboard;
using OvulaeApp.Views.PeriodTracker.Dashboard;
using OvulaeApp.Views.PregnancyTracker.Dashboard;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Models.User;
using OvulaeShared.Services.APIs.Users;
//using Plugin.Firebase.CloudMessaging;

namespace OvulaeApp.Services.Notifications
{
    public class FcmNotificationService : IFcmNotificationService
    {
        private readonly IUsersApi _usersApi;

        public FcmNotificationService(IUsersApi usersApi)
        {
            _usersApi = usersApi;
        }

        public async Task InitializeFirebase()
        {
            /*
            var fcm = CrossFirebaseCloudMessaging.Current;

            // 1️⃣ Subscribe to TokenChanged event
            fcm.TokenChanged += async (sender, args) =>
            {
                var token = args.Token;
                Console.WriteLine($"✅ FCM Token received: {token}");

                var userId = LocalStorageService.UserDetails?.UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    await _usersApi.CreateUserToken(new UserMobileToken
                    {
                        UserId = userId,
                        Token = token
                    });
                }
            };

            // 2️⃣ Notification received while app is in foreground
            fcm.NotificationReceived += (sender, args) =>
            {
                var notification = args.Notification;
                var data = notification.Data;

                if (data != null && data.TryGetValue("type", out var type) && type == "doctor_notes_update")
                {
                    var moduleName = data.TryGetValue("moduleName", out var mod) ? mod : "";
                    var entryId = data.TryGetValue("entryId", out var eid) ? eid : "";

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        bool open = await Application.Current.MainPage.DisplayAlert(
                            "Doctor Notes",
                            $"Your doctor added new notes under {moduleName}.",
                            "Open",
                            "Later"
                        );

                        if (open)
                        {
                            await NavigateToLogDetail(moduleName, entryId);
                        }
                    });
                }
            };

            // 3️⃣ Notification tapped (from background or system tray)
            fcm.NotificationTapped += (sender, args) =>
            {
                var notification = args.Notification;
                var data = notification.Data;

                if (data != null && data.TryGetValue("type", out var type) && type == "doctor_notes_update")
                {
                    var moduleName = data.TryGetValue("moduleName", out var mod) ? mod : "";
                    var entryId = data.TryGetValue("entryId", out var eid) ? eid : "";

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await NavigateToLogDetail(moduleName, entryId);
                    });
                }
            };


            // 4️⃣ Optional: error handler
            fcm.Error += (sender, args) =>
            {
                Console.WriteLine($"❌ Firebase Error: {args}");
            };

            // 5️⃣ Check permission and validity
            await fcm.CheckIfValidAsync();

            // 6️⃣ Fetch current token
            var currentToken = await fcm.GetTokenAsync();
            if (!string.IsNullOrEmpty(currentToken))
            {
                Console.WriteLine($"🔹 Current Token: {currentToken}");
                var userId = LocalStorageService.UserDetails?.UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    await _usersApi.CreateUserToken(new UserMobileToken
                    {
                        UserId = userId,
                        Token = currentToken
                    });
                }
            }
            */
        }

        private async Task NavigateToLogDetail(string moduleName, string entryId)
        {
            var moduleType = moduleName != null ? EnumHelper.GetEnumValueFromName<ModuleType>(moduleName) : ModuleType.PeriodTracker;
            switch (moduleType)
            {
                case ModuleType.Pregnancy:
                    await Shell.Current.GoToAsync($"{nameof(PregnancyDashboardDayLoggerPage)}?entryId={entryId}");
                    break;
                case ModuleType.PeriodTracker:
                    await Shell.Current.GoToAsync($"{nameof(PeriodDashboardDayLoggerPage)}?entryId={entryId}");
                    break;
                case ModuleType.Ovulation:
                    await Shell.Current.GoToAsync($"{nameof(OvulationDashboardDayLoggerPage)}?entryId={entryId}");
                    break;
                case ModuleType.MenopauseTracker:
                    await Shell.Current.GoToAsync($"{nameof(MenopauseDashboardDayLoggerPage)}?entryId={entryId}");
                    break;
                default:
                    await Application.Current.MainPage.DisplayAlert("Notification", "New doctor notes available.", "OK");
                    break;
            }
        }
    }
}
