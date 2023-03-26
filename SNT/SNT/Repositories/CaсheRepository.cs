using MonkeyCache.FileStore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xamarin.Essentials;
using SNT.Repositories;

namespace SNT.Repositories
{
    public class CaсheRepository
    {
        string barrel = "barrel";
        public CaсheRepository()
        {
            Barrel.ApplicationId = "barrel";
        }

        public bool CacheStringData(string url, string data)
        {
            try
            {
                Barrel.Current.Add<string>(url, data, TimeSpan.FromMinutes(1));
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public bool LongCacheStringData(string url, string data)
        {
            try
            {
                Barrel.Current.Add<string>(url, data, TimeSpan.FromHours(1));
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public string GetStringFromCache(string url)
        {
            try
            {
                if(!Barrel.Current.IsExpired(url) && Barrel.Current.Exists(url))
                {
                    return Barrel.Current.Get<string>(url);
                }
                return "nodata";
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Ошибка при попытке загрузить данные из кэша по url: {url} \n Ошибка: {ex.Message}");
                return ex.Message;
            }
        } 

        public bool isExpired(string url)
        {
            try
            {
                return Barrel.Current.IsExpired(url);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Ошибка проверки актуальности данных кэша. Ошибка: {ex.Message}");
                return true;
            }
        }

        public async void SaveLoginInfoInSecureStorage(int userId, string token, int sntId, List<UchastkiModel> uchastki)
        {
            DataRepository dataRepository = new DataRepository();
            await SecureStorage.SetAsync("userId", userId.ToString());
            await SecureStorage.SetAsync("token", token);
            await SecureStorage.SetAsync("sntId", sntId.ToString());
            foreach (UchastkiModel uchastok in uchastki)
            {
                int id = await dataRepository.GetUchastokId(int.Parse(uchastok.uchastok), sntId);
                await SecureStorage.SetAsync(uchastok.uchastok.ToString(), id.ToString());
            }
        }

        public bool isExist(string url)
        {
            try
            {
                return Barrel.Current.Exists(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка проверки существования данных кэша. Ошибка: {ex.Message}");
                return true;
            }
        }

        public bool ReturnStringIfExistAndActual(string url, out string data)
        {
            bool isexist = isExist(url);
            bool isexpired = isExpired(url);
            if(isExist(url) && !isExpired(url))
            {
                data = Barrel.Current.Get<string>(url);
                return true;
            }
            else
            {
                data = null;
                return false;
            }
        }
    }
}
