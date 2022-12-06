using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SNT.Navigation;
using System.Text.Json;
using System.Net.Http;
using System.Diagnostics;
using SNT.Models;
using Xamarin.Essentials;

namespace SNT
{
    public partial class LoginPage : ContentPage
    {
        HttpClient client = new HttpClient();
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public async void OnLoginClick(object sender, EventArgs e)
        {
            Application.Current.MainPage = new FlyoutMenu();
            return; //Убрать
            string login = loginEntry.Text;
            string password = passwordEntry.Text;
            Dictionary<string, string> loginData = new Dictionary<string, string>();
            
            loginData.Add("login", login);
            loginData.Add("password", password);

            string request = JsonSerializer.Serialize(loginData);

            if(!isLoginDataValid(login, password))
            {
                NotifyLoginInsuccessful();
                return;
            }

            StringContent stringContent = new StringContent(request, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("http://10.0.2.2:7000/auth/login", stringContent);

            if (!response.IsSuccessStatusCode)
            {
                NotifyServerError();
                return;
            }

            string responseString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(responseString);
            LoginResponse responseDeserialized = JsonSerializer.Deserialize<LoginResponse>(responseString);
            Debug.WriteLine(responseDeserialized.userid);
            Debug.WriteLine(responseDeserialized.token);
        }

        private bool isLoginDataValid(string login, string password)
        {
            return true; // to be added
        }

        private void NotifyLoginInsuccessful()
        {
            Debug.WriteLine("Логин не удался");
        }

        private void NotifyServerError()
        {
            Debug.WriteLine("Ошибка сервера");
        }
    }
}
