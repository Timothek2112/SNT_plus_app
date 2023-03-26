using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SNT.Navigation;
using Xamarin.Essentials;
using System.Text.Json;
using System.Net.Http;
using Xamarin.CommunityToolkit.Extensions;
using System.Diagnostics;
using SNT.Models;
using SNT.Repositories;
using System.Net;

namespace SNT
{
    public partial class LoginPage : ContentPage
    {
        LoginRepository loginRepository = new LoginRepository();
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;
            try
            {
                checkLogin();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ОШИБКА ПРОВЕРКИ ЛОГИНА" + ex.Message);
                this.DisplayToastAsync("Ошибка подключения к серверу");
            }
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
                    await Navigation.PushAsync(new Home());
                }
                else
                {
                    this.DisplayToastAsync("Вы не авторизованы, пожалуйста, войдите в аккаунт");
                    Debug.WriteLine("Не удалось войти");
                }
            }
        }

        public async void OnLoginClick(object sender, EventArgs e)
        {
            HttpStatusCode code = HttpStatusCode.Unauthorized;
            try
            {
                code = await loginRepository.login(loginEntry.Text, passwordEntry.Text);
                proceedCode(code);
            }
            catch (Exception ex) { this.DisplayToastAsync("Ошибка входа " + code.ToString() ); }
        }
        
        private async void proceedCode(HttpStatusCode code)
        {
            if (code == HttpStatusCode.OK)
            {
                this.DisplayToastAsync("Вход выполнен успешно");
                checkLogin();
                await Navigation.PushAsync(new Home());
            }
            else if (code == HttpStatusCode.Unauthorized)
            {
                this.DisplayToastAsync("Неверный логин или пароль");
            }
            else if (code == HttpStatusCode.InternalServerError)
            {
                this.DisplayToastAsync("Ошибка севера, попробуйте позже");
            }
        }

        private void loginSuccess()
        {
            Navigation.PushAsync(new Home());
        }

        private void NotifyServerError()
        {
            Debug.WriteLine("Ошибка сервера");
        }
    }
}
