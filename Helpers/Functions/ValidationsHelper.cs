using System.Net.Mail;

namespace OvulaeApp.Helpers.Functions
{
    public class ValidationsHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.Trim();
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                try
                {
                    var addr = new MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public static bool IsStrongPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                return password.Length >= 8 &&
                       password.Any(char.IsUpper) &&
                       password.Any(char.IsLower) &&
                       password.Any(char.IsDigit) &&
                       password.Any(ch => !char.IsLetterOrDigit(ch));
            }
            return false;
        }

        public static bool IsValidPhoneNumber(string phoneNumber) 
        {
            string digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (digits.StartsWith("0"))
                digits = digits.TrimStart('0');

            if (digits.Length > 15)
                digits = digits.Substring(0, 15);

            return digits.Length >= 8 && digits.Length <= 15;
        }
    }
}
