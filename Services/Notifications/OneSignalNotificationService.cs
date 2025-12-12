//using OneSignalSDK.DotNet;
using OvulaeApp.Helpers.Functions.Notifications;
using OvulaeApp.Helpers.Notifications;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Views.Dashboard;
using OvulaeApp.Views.MenopauseTracker.Dashboard;
using OvulaeApp.Views.OvulationTracker.Dashboard;
using OvulaeApp.Views.PeriodTracker.Dashboard;
using OvulaeApp.Views.PregnancyTracker.Dashboard;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Services.Notifications
{
    public class OneSignalNotificationService : IOneSignalNotificationService
    {
        public OneSignalNotificationService()
        {

        }

        public async Task InitializeOneSignal()
        {
            try
            {
                await RegisterOneSignalUser(LocalStorageService.UserDetails.UserId);
                //var notifications = OneSignal.Notifications;

                /* Foreground notification
                notifications.WillDisplay += (sender, args) =>
                {
                    var notification = args.Notification;
                    var data = notification.AdditionalData;

                    // Check if this is your custom type
                    if (data != null && data.TryGetValue("type", out var type) && type == "doctor_notes_update")
                    {
                        var moduleName = data.TryGetValue("moduleName", out var mod) ? mod?.ToString() : "";
                        var entryId = data.TryGetValue("entryId", out var eid) ? eid?.ToString() : "";

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

                        // Prevent default system notification if you want
                        //args.PreventDefault();
                    }
                };

                // Notification tapped
                notifications.Clicked += (sender, args) =>
                {
                    var notification = args.Notification;
                    var data = notification.AdditionalData; // <- correct property

                    if (Application.Current?.MainPage != null)
                    {
                        MainThread.BeginInvokeOnMainThread(async () => await HandleNotification(data, tapped: true));
                    }
                };
                */

            }
            catch
            {
                
            }
        }

        /// <summary>
        /// Optional: you can still call this after user login if their external ID changes
        /// </summary>
        private async Task RegisterOneSignalUser(string userId)
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
                if (status != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.PostNotifications>();
                }
            }
            if (!string.IsNullOrEmpty(userId))
            {
                //OneSignal.User.AddAlias("user_id", userId);
                //OneSignal.Login(userId); 
            }
        }

        private async Task HandleNotification(IDictionary<string, object> data, bool tapped = false)
        {
            if (data != null && data.TryGetValue("type", out var typeObj) && typeObj?.ToString() == "doctor_notes_update")
            {
                var moduleName = data.TryGetValue("moduleName", out var mod) ? mod?.ToString() : "";
                var entryId = data.TryGetValue("entryId", out var eid) ? eid?.ToString() : "";

                if (!tapped)
                {
                    bool open = await Application.Current.MainPage.DisplayAlert(
                        "Doctor Notes",
                        $"Your doctor added new notes under {moduleName}.",
                        "Open",
                        "Later"
                    );

                    if (!open)
                        return;
                }
                await NavigateToLogDetail(moduleName, entryId);
            }
        }

        private async Task NavigateToLogDetail(string moduleName, string entryId)
        {
            var navigatePage = NavigationsHelper.GetDayLogPageNameFromModuleName(moduleName);
            await Shell.Current.GoToAsync($"{navigatePage}?entryId={entryId}");
        }
    }
}
