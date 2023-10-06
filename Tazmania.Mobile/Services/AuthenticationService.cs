using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Mobile.Models;

namespace Tazmania.Mobile.Services
{
    public class AuthenticationService
    {
        public async Task SetCredential(UserModel user)
        {
            if (!string.IsNullOrEmpty(user.Mail) && !string.IsNullOrEmpty(user.Password))
            {
                await SecureStorage.Default.SetAsync("mail", user.Mail);
                await SecureStorage.Default.SetAsync("password", user.Password);
            }
        }

        public async Task<UserModel> GetCredential()
        {
            return new UserModel()
            {
                Mail = await SecureStorage.Default.GetAsync("mail"),
                Password = await SecureStorage.Default.GetAsync("password")
            };
        }
    }
}
