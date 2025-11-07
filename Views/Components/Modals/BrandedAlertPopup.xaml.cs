using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;

namespace OvulaeApp.Views.Components.Modals
{
    public partial class BrandedAlertPopup : Popup
    {
        private TaskCompletionSource<bool> _tcs;

        public BrandedAlertPopup(string title, string message, string closeBtnTxt = "Close")
        {
            InitializeComponent();

            PopupTitle.Text = title;
            PopupMessage.Text = message;
            CloseBtnTxt.Text = closeBtnTxt;

            CanBeDismissedByTappingOutsideOfPopup = false;

            _tcs = new TaskCompletionSource<bool>();
        }

        public Task<bool> ShowWithResultAsync()
        {
            return _tcs.Task;
        }

        private void ClosePopup(object sender, EventArgs e)
        {
            _tcs.TrySetResult(true);
            Close();
        }

        private void CloseXButtonTapped(object sender, TappedEventArgs e)
        {
            _tcs.TrySetResult(true);
            Close();
        }
    }
}