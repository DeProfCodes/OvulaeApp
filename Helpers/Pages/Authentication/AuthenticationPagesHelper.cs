using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Shapes;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Enums.Auth;
using OvulaeShared.Enums.User;
using OvulaeShared.Models;
using OvulaeShared.Models.User;
using OvulaeShared.ViewModel.Account;

namespace OvulaeApp.Helpers.Pages.Authentication
{
    public static class AuthenticationPagesHelper
    {
        public static void ToggleActionButtonButton(Button actionButton, bool isValidFields)
        {
            if (actionButton != null)
            {
                actionButton.IsEnabled = isValidFields;
                actionButton.Opacity = isValidFields ? 1 : 0.3;
            }
        }

        public static void UpdatePasswordStrenthUI(Ellipse ellipse, Label label, bool isValid)
        {
            try
            {
                if (isValid)
                {
                    ellipse.Fill = new SolidColorBrush(Color.FromArgb("#CB6CE6"));
                    label.TextColor = Color.FromArgb("#CB6CE6");
                }
                else
                {
                    ellipse.Fill = new SolidColorBrush(Colors.Transparent);
                    label.TextColor = Color.FromArgb("#B8B6BE");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UI error: {ex.Message}");
            }
        }

        public static void SetLoggedInUser(UserModel model)
        {
            try
            {
                LocalStorageService.UserDetails.UserId = model.UserId;
                LocalStorageService.UserDetails.Firstname = model.Firstname;
                LocalStorageService.UserDetails.Lastname = model.Lastname;
                LocalStorageService.UserDetails.Email = model.Email;
                LocalStorageService.UserDetails.Username = model.Username;
                LocalStorageService.UserDetails.PhoneNumber = model.PhoneNumber;
                LocalStorageService.UserDetails.AccountStatus = model.AccountStatus;
                LocalStorageService.UserDetails.CountryCode = model.CountryCode;
                LocalStorageService.UserDetails.UserRole = model.UserRole;
                LocalStorageService.Authenticated = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Local Storage service errorL {ex.Message}");
            }
        }

        public static void SetLoggedInUserFromSignUp(RegisterViewModel registerVm)
        {
            try
            {
                LocalStorageService.UserDetails.Firstname = registerVm.Firstname;
                LocalStorageService.UserDetails.Lastname = registerVm.Lastname;
                LocalStorageService.UserDetails.Email = registerVm.Email;
                LocalStorageService.UserDetails.PhoneNumber = registerVm.PhoneNumber;
                LocalStorageService.UserDetails.CountryCode = registerVm.CountryCode;
                LocalStorageService.UserDetails.UserRole = UserRoleType.Client;
                LocalStorageService.Authenticated = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Local Storage service errorL {ex.Message}");
            }
        }

        public static void SetPasswordResetObject(string otp, PasswordResetType type, string email = "", string countryCode = "", string phoneNumber = "")
        {
            try
            {
                LocalStorageService.PasswordReset.OTP = otp;
                LocalStorageService.PasswordReset.Type = type;
                LocalStorageService.PasswordReset.Email = email;
                LocalStorageService.PasswordReset.CountryCode = countryCode;
                LocalStorageService.PasswordReset.PhoneNumber = phoneNumber;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Local Storage service errorL {ex.Message}");
            }
        }
    }
}
