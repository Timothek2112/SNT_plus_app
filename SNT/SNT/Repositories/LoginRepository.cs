using SNT.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;
using MonkeyCache.FileStore;
using Plugin.Connectivity;
using SNT.Repositories;
using Xamarin.Forms;
using Plugin.FirebasePushNotification;

namespace SNT.Repositories
{
    public class LoginRepository
    {
        DataRepository dataRepository = new DataRepository();
        HttpClient client = new HttpClient();
        NetworkAccess currentInternetAccess;
        CaсheRepository cache = new CaсheRepository();
        public async Task<HttpStatusCode> login(string login, string password)
        {
            var url = Addresses.login;
            Dictionary<string, string> loginData = new Dictionary<string, string>
            {
                { "login", login },
                { "password", password }
            };
            string request = JsonSerializer.Serialize(loginData);
            
            if (!isLoginDataValid(login, password))
                throw new Exception("Логин или пароль содержат недопустимые символы");
            
            StringContent stringContent = new StringContent(request, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, stringContent);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return HttpStatusCode.InternalServerError;
            else if(response.StatusCode == HttpStatusCode.Unauthorized)
                return HttpStatusCode.Unauthorized;
            else if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

            string responseString = await response.Content.ReadAsStringAsync();
            LoginResponse responseDeserialized = JsonSerializer.Deserialize<LoginResponse>(responseString);
            List<UchastkiModel> uchastki = await dataRepository.GetUsersUchastki(responseDeserialized.userid);
            
            cache.SaveLoginInfoInSecureStorage (
                responseDeserialized.userid,
                responseDeserialized.token,
                responseDeserialized.SntId,
                uchastki
            );

            return HttpStatusCode.OK;
        }

        

        private bool isLoginDataValid(string login, string password)
        {
            if(login == null || login == "")
                return false;

            if (password == null && password == "")
                return false;

            return true;
        }

        public async Task<string> checkForLogin(string token)
        {
            string url = Addresses.checkLogin;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.GetAsync(url);
            string responseString = await response.Content.ReadAsStringAsync();
            SendToken();
            return responseString;
        }

        protected async void SendToken()
        {
            DataRepository dataRepository = new DataRepository();
            string userId = await SecureStorage.GetAsync("userId");
            if (userId == null) return;
            dataRepository.SendPushToken(CrossFirebasePushNotification.Current.Token, userId);
        }
    }
}
