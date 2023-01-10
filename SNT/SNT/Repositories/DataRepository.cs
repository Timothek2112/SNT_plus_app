using SNT.Models;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using System.ComponentModel;
using SNT.Navigation;
using Xamarin.Forms;
using MonkeyCache.FileStore;

namespace SNT.Repositories
{
    public class UchastkiModel
    {
        public string uchastok { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string userId { get; set; }
        
        public UchastkiModel(string uchastok, string createdAt, string updatedAt, string userId) {
            this.uchastok = uchastok;
            this.createdAt = createdAt;
            this.updatedAt = updatedAt;
            this.userId = userId;
        }
        
    }
    public enum State
    {
        Pokazania,
        Payments
    }

    class DataRepository
    {
        HttpClient client = new HttpClient();
        CaсheRepository cache = new CaсheRepository();
        public async Task<List<PaymentPokazanieModel>> GetStateForYear(int yearFixed, int uchastokId, State state)
        {
            Dictionary<string, int> requestData = new Dictionary<string, int>
            {
                { "startPeriodY", 1 },
                { "endPeriodY", DateTime.Now.Year % 100 },
                { "startPeriodM", 1 },
                { "endPeriodM", 12 },
                { "uchastokId", uchastokId }
            };
            string url = null;
            switch (state)
            {
                case State.Pokazania:
                    url = Addresses.pokazaniaForPeriod;
                    break;

                case State.Payments:
                    url = Addresses.paymentsForPeriod;
                    break;
            }

            string responseString = null;

            if (!cache.ReturnStringIfExistAndActual(url + uchastokId.ToString() + state.ToString(), out responseString))
            {
                StringContent requestBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, requestBody);
                responseString = await response.Content.ReadAsStringAsync();
                cache.CacheStringData(url + uchastokId.ToString() + state.ToString(), responseString);
                Debug.WriteLine($"Сохранены кэшированные данные по {url + uchastokId.ToString()}" + state.ToString());
            }
            else
            {
                Debug.WriteLine($"Загруженны кэшированные данные по {url + uchastokId.ToString()}" + state.ToString());
            }
            
            List<PaymentPokazanieModel> result = JsonConvert.DeserializeObject<List<PaymentPokazanieModel>>(responseString);
            return result;
        }

        public async Task<List<RateModel>> GetRates()
        {
            Dictionary<string, int> requestData = new Dictionary<string, int>
            {
                { "startPeriodY", 1 },
                { "endPeriodY", 99 },
                { "startPeriodM", 1 },
                { "endPeriodM", 12 },
            };
            string url = Addresses.ratesForPeriod;

            StringContent requestBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, requestBody);
            string responseString = await response.Content.ReadAsStringAsync();
            List<RateModel> result = null;
            if (responseString != null)
            {
                result = JsonConvert.DeserializeObject<List<RateModel>>(responseString);
            }
            return result;
        }

        public async Task<List<UchastkiModel>> GetUsersUchastki(int userId)
        {
            string responseString = null;
            string url = Addresses.getUsersUchastki + userId;

            if (!cache.isExpired(url) && cache.isExist(url)) {
                responseString = cache.GetStringFromCache(url);
                Debug.WriteLine("Загружены кэшированные данные по " + url);
            }
            else
            {
                HttpResponseMessage response = await client.GetAsync(url);
                responseString = await response.Content.ReadAsStringAsync();
                cache.CacheStringData(url, responseString);
                Debug.WriteLine("Сохранен кэш по " + url);
            }

            List<UchastkiModel> uchastki = JsonConvert.DeserializeObject<List<UchastkiModel>>(responseString);
            return uchastki;
        }

        public async Task<List<ElectricityCardModel>> GetElectricityDebtCards(int year, int uchastok)
        {
            List<DebtModel> debts = await CalculateRepository.GetDebtForYear(year, uchastok);
            List<ElectricityCardModel> cards = new List<ElectricityCardModel>();
            for (int i = 0; i < 12; i++)
            {
                cards.Add(new ElectricityCardModel());
                cards[i].period = i+1 < 10 ? "0" + $"{i+1}.{year}" : $"{i+1}.{year}";
                cards[i].debt = debts[i].electricityActualDebt;
                cards[i].payment = debts[i].payment.electricity;
                cards[i].SetAmount();
            }
            return cards;
        }

        public async Task<List<ElectricityCardModel>> GetWaterDebtCards(int year, int uchastok)
        {
            List<DebtModel> debts = await CalculateRepository.GetDebtForYear(year, uchastok);
            List<ElectricityCardModel> cards = new List<ElectricityCardModel>();
            for (int i = 0; i < 12; i++)
            {
                cards.Add(new ElectricityCardModel());
                cards[i].period = i + 1 < 10 ? "0" + $"{i + 1}.{year}" : $"{i + 1}.{year}";
                cards[i].debt = debts[i].waterPokazanieActualDebt;
                cards[i].payment = debts[i].payment.water;
                cards[i].SetAmount();
            }
            return cards;
        }

        public async Task<List<ElectricityCardModel>> GetPenalityDebtCards(int year, int uchastok)
        {
            List<DebtModel> debts = await CalculateRepository.GetDebtForYear(year, uchastok);
            List<ElectricityCardModel> cards = new List<ElectricityCardModel>();
            for (int i = 0; i < 12; i++)
            {
                cards.Add(new ElectricityCardModel());
                cards[i].period = i + 1 < 10 ? "0" + $"{i + 1}.{year}" : $"{i + 1}.{year}";
                cards[i].debt = debts[i].penality;
                cards[i].payment = debts[i].payment.penality;
                cards[i].SetAmount();
            }
            return cards;
        }

        public async Task<List<ElectricityCardModel>> GetMembershipDebtCards(int year, int uchastok)
        {
            List<DebtModel> debts = await CalculateRepository.GetDebtForYear(year, uchastok);
            List<ElectricityCardModel> cards = new List<ElectricityCardModel>();
            for (int i = 0; i < 12; i++)
            {
                cards.Add(new ElectricityCardModel());
                cards[i].period = i + 1 < 10 ? "0" + $"{i + 1}.{year}" : $"{i + 1}.{year}";
                cards[i].debt = debts[i].membership;
                cards[i].payment = debts[i].payment.membership;
                cards[i].SetAmount();
            }
            return cards;
        }

        public async Task<List<ElectricityCardModel>> GetTargetDebtCards(int year, int uchastok)
        {
            List<DebtModel> debts = await CalculateRepository.GetDebtForYear(year, uchastok);
            List<ElectricityCardModel> cards = new List<ElectricityCardModel>();
            for (int i = 0; i < 12; i++)
            {
                cards.Add(new ElectricityCardModel());
                cards[i].period = i + 1 < 10 ? "0" + $"{i + 1}.{year}" : $"{i + 1}.{year}";
                cards[i].debt = debts[i].target;
                cards[i].payment = debts[i].payment.target;
                cards[i].SetAmount();
            }
            return cards;
        }

        public async Task<bool> CreateElectricityPokazanie(int uchastok, int year, int month, float electricity)
        {
            string url = Addresses.createPokazanie;
            Dictionary<string, string> requestData = new Dictionary<string, string>
            {
                { "uchastokId", uchastok.ToString() },
                { "electricity", electricity.ToString() },
                { "month", month.ToString() },
                { "year", year.ToString() }
            };
            StringContent requestBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, requestBody);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateWaterPokazanie(int uchastok, int year, int month, float water)
        {
            string url = Addresses.createPokazanie;
            Dictionary<string, string> requestData = new Dictionary<string, string>
            {
                { "uchastokId", uchastok.ToString() },
                { "water", water.ToString() },
                { "month", month.ToString() },
                { "year", year.ToString() }
            };
            StringContent requestBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, requestBody);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateTargetPokazanie(int uchastok, int year, int month, float target)
        {
            string url = Addresses.createPokazanie;
            Dictionary<string, string> requestData = new Dictionary<string, string>
            {
                { "uchastokId", uchastok.ToString() },
                { "target", target.ToString() },
                { "month", month.ToString() },
                { "year", year.ToString() }
            };
            StringContent requestBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, requestBody);
            return response.IsSuccessStatusCode;
        }

        public async Task<DebtCardModel> GetDebts(int uchastk)
        {
            string url = Addresses.getDebts;
            string responseString;
            Dictionary<string, int> requestData = new Dictionary<string, int>
            {
                { "uchastokId", uchastk }
            };

            if (cache.ReturnStringIfExistAndActual(url, out responseString))
            {
                Debug.WriteLine("Загружены кэшированные данные по " + url);
            }
            else
            {
                string serialized = System.Text.Json.JsonSerializer.Serialize(requestData);
                Debug.WriteLine(serialized);
                StringContent requestBody = new StringContent(serialized, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, requestBody);
                responseString = await response.Content.ReadAsStringAsync();
                cache.CacheStringData(url, responseString);
                Debug.WriteLine("Загружены кэшированные данные по " + url);
            }

            DebtCardModel cards = JsonConvert.DeserializeObject<DebtCardModel>(responseString);
            return cards;
        }
    }
}
