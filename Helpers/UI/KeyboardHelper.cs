using Microsoft.Maui;                       // IApplication, IWindow
using Microsoft.Maui.Controls;              // Page, View

#if ANDROID
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
#endif

#if IOS || MACCATALYST
using UIKit;
using Microsoft.Maui.ApplicationModel;      // Platform.GetCurrentUIViewController()
#endif


namespace OvulaeApp.Helpers.UI
{
    public static class KeyboardHelper
    {
        public static void Dismiss()
        {
            // Best effort unfocus
            try
            {
                var page = Application.Current?.Windows?.FirstOrDefault()?.Page as VisualElement;
                page?.Unfocus();
            }
            catch 
            { 
                /* no-op */ 
            }

#if ANDROID
            try
            {
                var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                var view = activity?.CurrentFocus ?? activity?.Window?.DecorView?.RootView;
                if (activity != null && view != null)
                {
                    var imm = (InputMethodManager)activity.GetSystemService(Context.InputMethodService)!;
                    imm.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.None);
                    view.ClearFocus();
                }
            }
            catch 
            {
                /* no-op */ 
            }
#elif IOS 
            try
            {
                var vc = Platform.GetCurrentUIViewController();
                vc?.View?.EndEditing(true);     // dismisses keyboard
            }
            catch 
            { 
                /* no-op */ 
            }
#endif
        }
    }
}
