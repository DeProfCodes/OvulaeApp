using Microsoft.Maui.Controls;

#if ANDROID
using Android.Content;
using Android.Views.InputMethods;
#endif

#if IOS || MACCATALYST
using UIKit;
#endif

namespace OvulaeApp.Helpers.UI.Behaviors
{
    public sealed class DismissKeyboardBehavior : Behavior<View>
    {
        TapGestureRecognizer? _tap;

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            _tap = new TapGestureRecognizer { Command = new Command(() => Dismiss(bindable)) };
            bindable.GestureRecognizers.Add(_tap);
        }

        protected override void OnDetachingFrom(View bindable)
        {
            base.OnDetachingFrom(bindable);
            if (_tap != null)
                bindable.GestureRecognizers.Remove(_tap);
        }

        static void Dismiss(View? bindable)
        {
            // Cross-platform: drop focus from the currently focused control within this subtree
            try 
            { 
                bindable?.Unfocus(); 
            } 
            catch 
            { 
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
#elif IOS || MACCATALYST
            try
            {
                var vc = Microsoft.Maui.ApplicationModel.Platform.GetCurrentUIViewController();
                vc?.View?.EndEditing(true); // <-- UIKit; requires `using UIKit;`
            }
            catch 
            { 
                /* no-op */ 
            }
#endif
        }
    }
}