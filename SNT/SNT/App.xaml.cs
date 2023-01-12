﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SNT.Navigation;
using System.Collections.Generic;
using SNT.Themes;
using Xamarin.Essentials;
using SNT.Models;
using Plugin.FirebasePushNotification;

namespace SNT
{
    public partial class App : Application
    {
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
    }
}
