using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OvulaeApp.Services.UserInterface.Components;
using OvulaeApp.Views.Authentication;

namespace OvulaeApp.ViewModels.Shared
{
    public class NavigationsViewModel
    {
        public ISpinnerService? SpinnerService { get; set; }

        public ICommand NavigateToLoginCommand => new Command(async () =>
        {
            if (SpinnerService != null)
                await SpinnerService.ShowSpinnerAsync();

            await Shell.Current.GoToAsync(nameof(LoginPage));

            if (SpinnerService != null)
                await SpinnerService.HideSpinner();
        });

        public ICommand NavigateToSignUpCommand => new Command(async () =>
        {
            if (SpinnerService != null)
                await SpinnerService.ShowSpinnerAsync();

            await Shell.Current.GoToAsync(nameof(SignUpEmailPage));

            if (SpinnerService != null)
                await SpinnerService.HideSpinner();
        });

        public ICommand GoBackCommand => new Command(async () =>
        {
            if (SpinnerService != null)
                await SpinnerService.ShowSpinnerAsync();

            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                await Shell.Current.Navigation.PopAsync();
            }

            if (SpinnerService != null)
                await SpinnerService.HideSpinner();
        });
    }
}
