using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.UI
{
    public static class ViewExtensions
    {
        public static Task WidthRequestTo(this VisualElement element, double toWidth, uint length, Easing easing)
        {
            var tcs = new TaskCompletionSource<bool>();
            double from = element.WidthRequest;

            var animation = new Animation(v => element.WidthRequest = v, from, toWidth);
            animation.Commit(
                element,
                "WidthAnim",
                16,
                length,
                easing,
                finished: (v, c) => tcs.SetResult(true)
            );

            return tcs.Task;
        }
    }
}
