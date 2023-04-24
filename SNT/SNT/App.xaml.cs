using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SNT.Navigation;
using System.Collections.Generic;
using SNT.Themes;
using Xamarin.Essentials;
using SNT.Models;
using Plugin.FirebasePushNotification;
using SNT.Repositories;
using System.Diagnostics;
using System.Net;
using Xamarin.CommunityToolkit.Extensions;
using System.Threading.Tasks;

namespace SNT
{
    public partial class App : Application
    {
        LoginRepository loginRepository = new LoginRepository();
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage())
            {
                BarBackgroundColor = (Color)Application.Current.Resources["PrimaryLight"]
            };

            CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;
        }

        private void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"TOKEN: {e.Token}");
            SendPushToken(e.Token);
        }

        protected override void OnStart()
        {
            if (Preferences.Get("DarkTheme", false)) App.Current.UserAppTheme = OSAppTheme.Dark;
            else App.Current.UserAppTheme = OSAppTheme.Light;
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            if (Preferences.Get("DarkTheme", false)) App.Current.UserAppTheme = OSAppTheme.Dark;
            else App.Current.UserAppTheme = OSAppTheme.Light;
        }
        
        protected async void SendPushToken(string token)
        {
            DataRepository dataRepository = new DataRepository();
            string userId = await SecureStorage.GetAsync("userId");
            if (userId == null) return;
            dataRepository.SendPushToken(token, userId);
        }

        private async Task checkLogin()
        {
            string token = await SecureStorage.GetAsync("token");
            string response = "false";
            if (token != null)
            {
                Debug.WriteLine("Пытаюсь проверить логин....");
                response = await loginRepository.checkForLogin(token);
                if (response == "true")
                {
                    MainPage = new Home();
                }
            }
        }



        private async void proceedCode(HttpStatusCode code)
        {
            if (code == HttpStatusCode.OK)
            {
                checkLogin();
                MainPage = new Home();
            }
        }

        private void loginSuccess()
        {
            MainPage = new Home();
        }

        private void NotifyServerError()
        {
            Debug.WriteLine("Ошибка сервера");
        }
    }
}
