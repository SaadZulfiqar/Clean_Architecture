using DataLoadTool.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace DataLoadTool.Application.Utilities
{
    public static class Common
    {
        public static void ValidateLoginArguments(Login login, bool isSuperUser = true)
        {
            if (string.IsNullOrWhiteSpace(login.Email))
            {
                throw new ArgumentException("Email is required.");
            }

            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(login.Email))
            {
                throw new ArgumentException("Invalid email format.");
            }

            if (string.IsNullOrWhiteSpace(login.Password))
            {
                throw new ArgumentException("Password is required.");
            }

            if (!isSuperUser && string.IsNullOrWhiteSpace(login.Tenant_id))
            {
                throw new ArgumentException("Tenant Id is required.");
            }
        }
    }
}
