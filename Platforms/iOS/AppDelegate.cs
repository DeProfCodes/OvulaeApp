using BranchSDK;
using Foundation;
using OvulaeApp.Services.LocalDataService;
//using Plugin.Firebase.CloudMessaging;
//using Plugin.Firebase.Core.Platforms.iOS;
using UIKit;

namespace OvulaeApp
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate, IBranchSessionInterface
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            try
            {
                Branch.EnableLogging = true; // turn off in release
                BranchIOS.Init("key_live_fxwnSISmLMaw01sWCZ6xMopcDDdna5R8", launchOptions, this);

                //CrossFirebase.Initialize();
                //FirebaseCloudMessagingImplementation.Initialize();

                return base.FinishedLaunching(application, launchOptions);
            }
            catch
            {
                return false;
            }
        }

        // Universal Links
        public override bool ContinueUserActivity(UIApplication app, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            try
            {
                return BranchIOS.getInstance().ContinueUserActivity(userActivity);
            }
            catch
            {
                return false;
            }
        }

        // Custom scheme fallback
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            try
            {
                return BranchIOS.getInstance().OpenUrl(url);
            }
            catch
            {
                return false;
            }
        }

        // Branch will deliver the payload here on first open after install (deferred) or direct opens
        public void InitSessionComplete(Dictionary<string, object> data)
        {
            try
            {
                if (data is null) return;

                if (data.TryGetValue("refCode", out var value) && value is string refCode && !string.IsNullOrWhiteSpace(refCode))
                {
                    LocalStorageService.AffiliateJoinCode = refCode;
                    Console.WriteLine($"[Branch] iOS refCode: {refCode}");
                }
            }
            catch
            {
                
            }
        }

        public void SessionRequestError(BranchError error)
        {
            try
            {
                Console.WriteLine($"[Branch] Error: {error?.ErrorMessage} ({error?.ErrorCode})");
            }
            catch
            {
                
            }
        }

    }
}
