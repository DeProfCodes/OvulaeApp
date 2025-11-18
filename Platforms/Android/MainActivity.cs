using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using CommunityToolkit.Maui.Views;
using OneSignalSDK.DotNet;
using OvulaeApp.Helpers.Notifications;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.Platform;
using OvulaeShared.Enums.User;
//using Plugin.Firebase.Core.Platforms.Android;

namespace OvulaeApp
{
    // ✅ Add this attribute to support deep linking
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable}, DataScheme = "ovulae", DataHost = "payment-success" )]

    [Activity(
        Theme = "@style/SplashTheme", // Use splash theme initially
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)
    ]
    public class MainActivity : MauiAppCompatActivity
    {
        const string TAG = "OvulaeMainActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Switch to main theme before calling base.OnCreate
            SetTheme(Resource.Style.AppTheme);

            base.OnCreate(savedInstanceState);

            //OneSignal.Initialize("740b7148-dfdb-4ad6-964b-e4414b304e41");

            Window.SetSoftInputMode(SoftInput.AdjustResize);

            LocalStorageService.MobileDeviceType = MobileDeviceType.Android;

            // debug: check any extras for launches (cold start)
            try
            {
                CheckForNotificationIntent(Intent, isColdStart: true);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, $"CheckForNotificationIntent (OnCreate) error: {ex}");
            }

            var referrerService = new InstallReferrerService();
            referrerService.GetReferrer(this, refCode =>
            {
                Console.WriteLine($"Install Referrer: {refCode}");
                LocalStorageService.AffiliateJoinCode = refCode;
            });

        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            try
            {
                // Handle when activity already exists and you tap a notification
                CheckForNotificationIntent(intent, isColdStart: false);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, $"CheckForNotificationIntent (OnNewIntent) error: {ex}");
            }
        }

        // Robust extractor: tries direct extras, then common OneSignal keys, then any JSON values
        private void CheckForNotificationIntent(Intent intent, bool isColdStart)
        {
            if (intent == null || intent.Extras == null)
            {
                Log.Info(TAG, "No intent extras");
                return;
            }

            var extras = intent.Extras;
            // Log all keys & values for debugging (remove in production)
            foreach (var key in extras.KeySet())
            {
                try
                {
                    var val = extras.Get(key);
                    Log.Info(TAG, $"Intent extra - {key} = {val}");
                }
                catch { }
            }

            // 1) Try direct keys first (module, entryId) - case-insensitive
            string module = null;
            string entryId = null;

            if (extras.ContainsKey("module"))
                module = extras.GetString("module");
            if (extras.ContainsKey("moduleName"))
                module ??= extras.GetString("moduleName");
            if (extras.ContainsKey("entryId"))
                entryId = extras.GetString("entryId");
            if (extras.ContainsKey("entryid"))
                entryId ??= extras.GetString("entryid");

            // 2) If not found, try common OneSignal keys that may contain JSON
            if (module == null || entryId == null)
            {
                string[] possibleJsonKeys = new[] { "onesignal_data", "custom", "data", "payload", "notification", "additionalData" };
                foreach (var key in possibleJsonKeys)
                {
                    if (!extras.ContainsKey(key)) continue;
                    var json = extras.GetString(key);
                    if (string.IsNullOrEmpty(json)) continue;
                    try
                    {
                        // Try parse JSON and look for module/moduleName and entryId
                        var doc = System.Text.Json.JsonDocument.Parse(json);
                        if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object)
                        {
                            if (doc.RootElement.TryGetProperty("module", out var mprop))
                                module ??= mprop.GetString();
                            if (doc.RootElement.TryGetProperty("moduleName", out var mn))
                                module ??= mn.GetString();
                            if (doc.RootElement.TryGetProperty("entryId", out var eprop))
                                entryId ??= eprop.GetString();
                            if (doc.RootElement.TryGetProperty("entryid", out var eprop2))
                                entryId ??= eprop2.GetString();

                            // OneSignal sometimes nests your data under "custom" -> "a" or "data" etc.
                            if ((module == null || entryId == null) && doc.RootElement.TryGetProperty("a", out var aProp))
                            {
                                // try nested
                                foreach (var p in aProp.EnumerateObject())
                                {
                                    if (p.NameEquals("module") || p.NameEquals("moduleName")) module ??= p.Value.GetString();
                                    if (p.NameEquals("entryId") || p.NameEquals("entryid")) entryId ??= p.Value.GetString();
                                }
                            }

                            if (module != null && entryId != null)
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warn(TAG, $"Failed to parse intent json key {key}: {ex.Message}");
                    }
                }
            }

            // 3) Final fallback: try Any extras whose value is JSON and contains fields
            if ((module == null || entryId == null))
            {
                foreach (var key in extras.KeySet())
                {
                    try
                    {
                        var s = extras.GetString(key);
                        if (string.IsNullOrEmpty(s)) continue;
                        if (!(s.TrimStart().StartsWith("{") || s.TrimStart().StartsWith("["))) continue;
                        
                        var doc = System.Text.Json.JsonDocument.Parse(s);
                        
                        if (doc.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object) 
                            continue;

                        if (doc.RootElement.TryGetProperty("module", out var mprop))
                            module ??= mprop.GetString();

                        if (doc.RootElement.TryGetProperty("moduleName", out var mn))
                            module ??= mn.GetString();

                        if (doc.RootElement.TryGetProperty("entryId", out var eprop))
                            entryId ??= eprop.GetString();

                        if (module != null && entryId != null) 
                            break;
                    }
                    catch 
                    {

                    }
                }
            }

            if (!string.IsNullOrEmpty(module) && !string.IsNullOrEmpty(entryId))
            {
                Log.Info(TAG, $"Saving PendingNavigationCache: module={module}, entryId={entryId}, isColdStart={isColdStart}");
                PendingNavigationCache.Save(module, entryId);
            }
            else
            {
                Log.Info(TAG, $"No module/entryId discovered in intent extras.");
            }
        }
    }
}
