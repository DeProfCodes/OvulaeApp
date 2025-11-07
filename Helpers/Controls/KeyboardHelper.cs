using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.Controls
{
    public static class KeyboardHelper2
    {
        public static void DismissKeyboard(this ContentPage page)
        {
            if (page?.Content != null)
            {
                TryUnfocusElement(page.Content);
            }
        }

        /// <summary>
        /// Forcefully dismiss the soft keyboard by shifting focus.
        /// </summary>
        public static void DismissKeyboard()
        {
            try
            {
                var dummyEntry = new Entry
                {
                    IsVisible = false,
                    HeightRequest = 0,
                    WidthRequest = 0
                };

                var currentPage = Application.Current?.MainPage?.Navigation?.NavigationStack.LastOrDefault();
                if (currentPage == null)
                    return;

                if (currentPage is ContentPage contentPage)
                {
                    // Add dummy to visual tree
                    contentPage.Content.FindByName<Layout>("RootLayout")?.Children.Add(dummyEntry);

                    dummyEntry.Focus();
                    dummyEntry.Unfocus();

                    contentPage.Content.FindByName<Layout>("RootLayout")?.Children.Remove(dummyEntry);
                }
            }
            catch
            {
                
            }
        }

        private static bool TryUnfocusElement(IElement element)
        {
            try
            {
                if (element is Entry entry && entry.IsFocused)
                {
                    entry.Unfocus();
                    return true;
                }

                // Handle Layouts (StackLayout, Grid, etc.)
                if (element is Layout layout)
                {
                    foreach (var child in layout.Children)
                    {
                        if (TryUnfocusElement(child))
                            return true;
                    }
                }

                // Handle ContentView
                if (element is ContentView contentView && contentView.Content != null)
                {
                    if (TryUnfocusElement(contentView.Content))
                        return true;
                }

                // Handle ScrollView
                if (element is ScrollView scrollView && scrollView.Content != null)
                {
                    if (TryUnfocusElement(scrollView.Content))
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
