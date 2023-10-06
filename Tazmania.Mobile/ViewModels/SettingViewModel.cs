using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Mobile.Models;
using Tazmania.Mobile.Services;

namespace Tazmania.Mobile.ViewModels
{
    public class SettingViewModel : BaseViewModel
    {
        string baseUrl;
        string email;
        string password;

        public string BaseUrl
        {
            get { return baseUrl; }
            set { baseUrl = value; OnPropertyChanged(nameof(BaseUrl)); }
        }

        public string EMail 
        {
            get { return email; }
            set { email = value; SaveLogin.ChangeCanExecute(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value; SaveLogin.ChangeCanExecute(); }
        }

        public Command SaveLogin { get; set; }


        public SettingViewModel(SettingRestService settingRestService, AuthenticationService authenticationService)
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    BaseUrl = settingRestService.BaseUrl;

                    await Task.Delay(5000);
                }
            });

            SaveLogin = new Command(async _ =>
            {
                await authenticationService.SetCredential(new UserModel() { Mail = EMail, Password = Password });
                
                EMail = string.Empty; 
                Password = string.Empty;

                _ = Task.Run(async () =>
                {
                    // attendo 2 secondo prima di chiudere altrimenti SetCredential non funziona
                    await Task.Delay(2000);
                    Application.Current.Quit();
                });

            }, _ =>
            {
                return !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password);
            });
        }
    }
}
