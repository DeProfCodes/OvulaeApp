using Android.App;
using Android.Content.PM;
using Android.Content;
using Microsoft.Maui.Authentication;

namespace OvulaeApp.Platforms.Android
{
    // NoHistory avoids leaving a blank screen in the recent apps
    [Activity(Exported = true, NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "ovulae",           
        DataHost = "paystack",           
        DataPathPrefix = "/callback"     
    )]
    public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
    {
        // Intentionally empty. Base class wires the result back to WebAuthenticator.
    }

}
