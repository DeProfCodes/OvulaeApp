using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Switch to main theme before calling base.OnCreate
            SetTheme(Resource.Style.AppTheme);

            base.OnCreate(savedInstanceState);

            //CrossFirebase.Initialize(this);

            Window.SetSoftInputMode(SoftInput.AdjustResize);

            LocalStorageService.MobileDeviceType = MobileDeviceType.Android;

            var referrerService = new InstallReferrerService();
            referrerService.GetReferrer(this, refCode =>
            {
                Console.WriteLine($"Install Referrer: {refCode}");
                LocalStorageService.AffiliateJoinCode = refCode;
            });

        }
    }
}
