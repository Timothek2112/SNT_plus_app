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

namespace SNT.Repositories
{
    class LoginRepository
    {
        HttpClient client = new HttpClient();
        NetworkAccess currentInternetAccess;
        public async Task<HttpStatusCode> login(string login, string password)
        {
            currentInternetAccess = Connectivity.NetworkAccess;
            var url = Addresses.login;
            Barrel.ApplicationId = "barrel";
            HttpResponseMessage response = null;
            StringContent stringContent;
            
            if (currentInternetAccess != NetworkAccess.Internet  && Barrel.Current.Exists(url) && !Barrel.Current.IsExpired(url))
            {
                Debug.WriteLine("Интернета нет, возвращаю сохранённые данные");
                //response = Barrel.Current.Get<HttpResponseMessage>(url);
            }
            else
            {
                Dictionary<string, string> loginData = new Dictionary<string, string>();
                loginData.Add("login", login);
                loginData.Add("password", password);
                string request = JsonSerializer.Serialize(loginData);

                if (!isLoginDataValid(login, password))
                    throw new Exception("Логин или пароль содержат недопустимые символы");

                stringContent = new StringContent(request, Encoding.UTF8, "application/json");
                response = await client.PostAsync(url, stringContent);
                Barrel.Current.Add<HttpResponseMessage>(url, response, TimeSpan.FromMinutes(1));
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return HttpStatusCode.InternalServerError;
            else if(response.StatusCode == HttpStatusCode.Unauthorized)
                return HttpStatusCode.Unauthorized;
            else if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

            string responseString = await response.Content.ReadAsStringAsync();
            LoginResponse responseDeserialized = JsonSerializer.Deserialize<LoginResponse>(responseString);
            
            await SecureStorage.SetAsync("userId", responseDeserialized.userid.ToString());
            await SecureStorage.SetAsync("token", responseDeserialized.token);

            return HttpStatusCode.OK;
        }

        private bool isLoginDataValid(string login, string password)
        {
            if(login != null && login != "")
            {
                if(password != null && password != "")
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<string> checkForLogin(string token)
        {
            string url = Addresses.checkLogin;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Debug.WriteLine(token);
            HttpResponseMessage response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
