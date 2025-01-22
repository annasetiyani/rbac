
using System.Net.Mail;

namespace Rbac.Core.Services.Extentions
{
    public static class ValidatorExtention
    {
        public static bool IsValidEmail(string emailAddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailAddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
