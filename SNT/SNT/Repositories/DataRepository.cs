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
using Xamarin.Essentials;
using System.Linq;

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

        public async Task<int> GetUchastokId(int uchastok, int sntId)
        {
            string url = Addresses.getUchastokId + uchastok + "/" + sntId;
            HttpResponseMessage response = await client.GetAsync(url);
            string responseString = await response.Content.ReadAsStringAsync();
            UchastokResponse snt = JsonConvert.DeserializeObject<UchastokResponse>(responseString);
            return snt.SntId;
        }

        public async Task<List<PaymentPokazanieModel>> GetStateForYear(int yearFixed, int uchastokId, int sntId, State state)
        {
            Dictionary<string, int> requestData = new Dictionary<string, int>
            {
                { "startPeriodY", 1 },
                { "endPeriodY", DateTime.Now.Year % 100 },
                { "startPeriodM", 1 },
                { "endPeriodM", 12 },
                { "uchastokId", await GetUchastokId(uchastokId, sntId) },
                { "SntId", sntId },
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

            StringContent requestBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, requestBody);
            string responseString = await response.Content.ReadAsStringAsync();
            List<PaymentPokazanieModel> result = JsonConvert.DeserializeObject<List<PaymentPokazanieModel>>(responseString);
            
            return result;
        }

        public async Task<List<RateModel>> GetRates(int sntId)
        {
            Dictionary<string, int> requestData = new Dictionary<string, int>
            {
                { "startPeriodY", 1 },
                { "endPeriodY", 99 },
                { "startPeriodM", 1 },
                { "endPeriodM", 12 },
                { "SntId", sntId },
            };
            string url = Addresses.ratesForPeriod;
            string responseString = null;
            StringContent requestBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, requestBody);
            responseString = await response.Content.ReadAsStringAsync();
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
            HttpResponseMessage response = await client.GetAsync(url);
            responseString = await response.Content.ReadAsStringAsync();
            List<UchastkiModel> uchastki = JsonConvert.DeserializeObject<List<UchastkiModel>>(responseString);
            return uchastki;
        }

        public async Task<List<ElectricityCardModel>> GetElectricityDebtCards(int year, int uchastok, int sntId)
        {
            List<ElectricityCardModel> cards = new List<ElectricityCardModel>();
            try
            {
                List<DebtModel> debts = await CalculateRepository.GetDebtForYear(year, uchastok, sntId);
                for (int i = 0; i < 12; i++)
                {
                    cards.Add(new ElectricityCardModel());
                    cards[i].period = i + 1 < 10 ? "0" + $"{i + 1}.{year}" : $"{i + 1}.{year}";
                    cards[i].debt = debts[i].electricityActualDebt;
                    cards[i].payment = debts[i].payment.electricity;
                    cards[i].SetAmount();
                }
            }
            catch
            {
                Debug.WriteLine("Ошибка подсчета долгов");
            }
            return cards;
        }

        public async Task<List<ElectricityCardModel>> GetWaterDebtCards(int year, int uchastok, int sntId)
        {
            List<ElectricityCardModel> cards = new List<ElectricityCardModel>();
            try
            {
                List<DebtModel> debts = await CalculateRepository.GetDebtForYear(year, uchastok, sntId);
                for (int i = 0; i < 12; i++)
                {
                    cards.Add(new ElectricityCardModel());
                    cards[i].period = i + 1 < 10 ? "0" + $"{i + 1}.{year}" : $"{i + 1}.{year}";
                    cards[i].debt = debts[i].waterPokazanieActualDebt;
                    cards[i].payment = debts[i].payment.water;
                    cards[i].SetAmount();
                }
            }
            catch
            {
                Debug.WriteLine("Ошибка подсчета долгов");
            }
            return cards;
        }

        public async Task<List<ElectricityCardModel>> GetPenalityDebtCards(int year, int uchastok, int sntId)
        {
            List<ElectricityCardModel> cards = new List<ElectricityCardModel>();
            try
            {
                List<DebtModel> debts = await CalculateRepository.GetDebtForYear(year, uchastok, sntId);
                for (int i = 0; i < 12; i++)
                {
                    cards.Add(new ElectricityCardModel());
                    cards[i].period = i + 1 < 10 ? "0" + $"{i + 1}.{year}" : $"{i + 1}.{year}";
                    cards[i].debt = debts[i].penality;
                    cards[i].payment = debts[i].payment.penality;
                    cards[i].SetAmount();
                }
            }
            catch { Debug.WriteLine("Ошибка подсчета долгов"); }
            return cards;
        }

        public async Task<List<ElectricityCardModel>> GetMembershipDebtCards(int year, int uchastok, int sntId)
        {
            List<ElectricityCardModel> cards = new List<ElectricityCardModel>();
            try
            {
                List<DebtModel> debts = await CalculateRepository.GetDebtForYear(year, uchastok, sntId);
                for (int i = 0; i < 12; i++)
                {
                    cards.Add(new ElectricityCardModel());
                    cards[i].period = i + 1 < 10 ? "0" + $"{i + 1}.{year}" : $"{i + 1}.{year}";
                    cards[i].debt = debts[i].membership;
                    cards[i].payment = debts[i].payment.membership;
                    cards[i].SetAmount();
                }
            }
            catch
            {
                Debug.WriteLine("Ошибка подсчета долгов");
            }
            return cards;
        }

        public async Task<List<ElectricityCardModel>> GetTargetDebtCards(int year, int uchastok, int sntId)
        {
            List<ElectricityCardModel> cards = new List<ElectricityCardModel>();
            try
            {
                List<DebtModel> debts = await CalculateRepository.GetDebtForYear(year, uchastok, sntId);
                for (int i = 0; i < 12; i++)
                {
                    cards.Add(new ElectricityCardModel());
                    cards[i].period = i + 1 < 10 ? "0" + $"{i + 1}.{year}" : $"{i + 1}.{year}";
                    cards[i].debt = debts[i].target;
                    cards[i].payment = debts[i].payment.target;
                    cards[i].SetAmount();
                }
            }
            catch { Debug.WriteLine("Ошибка подсчета долгов"); }
            return cards;
        }

        public async Task<bool> CreateElectricityPokazanie(int uchastok, int year, int month, float electricity, int sntId)
        {
            HttpResponseMessage response = null;
            try
            {
                string url = Addresses.createPokazanie;
                Dictionary<string, string> requestData = new Dictionary<string, string>
                {
                    { "uchastokId", await SecureStorage.GetAsync(uchastok.ToString()) },
                    { "electricity", electricity.ToString() },
                    { "month", month.ToString() },
                    { "year", year.ToString() },
                    { "SntId", sntId.ToString() }
                };
                StringContent requestBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
                response = await client.PostAsync(url, requestBody);
            }
            catch
            {
                Debug.WriteLine("Ошибка создания показания");
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateWaterPokazanie(int uchastok, int year, int month, float water, int sntId)
        {
            string url = Addresses.createPokazanie;
            Dictionary<string, string> requestData = new Dictionary<string, string>
            {
                { "uchastokId", await SecureStorage.GetAsync(uchastok.ToString()) },
                { "water", water.ToString() },
                { "month", month.ToString() },
                { "year", year.ToString() },
                { "SntId", sntId.ToString() }
            };
            StringContent requestBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, requestBody);
            return response.IsSuccessStatusCode;
        }

        public async Task<DebtCardModel> GetDebts(int uchastk, int sntId)
        {
            DebtCardModel cards = null;
            string url = Addresses.getDebts;
            
            try
            {
                Dictionary<string, int> requestData = new Dictionary<string, int>
                {
                    { "uchastokId", int.Parse(await SecureStorage.GetAsync(uchastk.ToString())) },
                    { "SntId", sntId }
                };
                
                string serialized = System.Text.Json.JsonSerializer.Serialize(requestData);
                StringContent requestBody = new StringContent(serialized, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, requestBody);
                string responseString = await response.Content.ReadAsStringAsync();
                cards = JsonConvert.DeserializeObject<DebtCardModel>(responseString);
            }
            catch { Debug.WriteLine("Ошибка подсчета долгов"); }
            return cards;
        }

        public async Task<List<NewsModel>> getNews(int count, int sntId)
        {
            string url = Addresses.getNews + count + "/" + sntId;
            HttpResponseMessage response = await client.GetAsync(url);
            string responseString = await response.Content.ReadAsStringAsync();
            List<NewsModel> news = JsonConvert.DeserializeObject<List<NewsModel>>(responseString);
            return news;
        }

        public async Task<List<PokazanieCardModel>> GetWaterPokazania(int year,int uchastok, int sntId)
        {
            List<PaymentPokazanieModel> pokazania = await this.GetStateForYear(year, uchastok, sntId, State.Pokazania);
            List<PokazanieCardModel> pokazaniaCards = new List<PokazanieCardModel>();
            List<PokazanieModel> calculatedPokazania =  await CalculateRepository.CalculatePokazania(pokazania, year);
            for (int i = 1; i <= 12; i++)
            {
                PokazanieModel currentPokazanie = calculatedPokazania[i-1];
                
                pokazaniaCards.Add(new PokazanieCardModel());
                pokazaniaCards[pokazaniaCards.Count - 1].pokazanie = currentPokazanie.rawWater;
                pokazaniaCards[pokazaniaCards.Count - 1].forPeriod = currentPokazanie.water;
                pokazaniaCards[pokazaniaCards.Count - 1].period = i < 10 ? "0" + $"{i}.{year}" : $"{i}.{year}";

            }
            return pokazaniaCards;
        }

        public async Task<List<PokazanieCardModel>> GetElectricityPokazania(int year, int uchastok, int sntId)
        {
            List<PaymentPokazanieModel> pokazania = await this.GetStateForYear(year, uchastok, sntId, State.Pokazania);
            List<PokazanieCardModel> pokazaniaCards = new List<PokazanieCardModel>();
            List<PokazanieModel> calculatedPokazania = await CalculateRepository.CalculatePokazania(pokazania, year);
            for (int i = 1; i <= 12; i++)
            {
                PokazanieModel currentPokazanie = calculatedPokazania[i - 1];

                pokazaniaCards.Add(new PokazanieCardModel());
                pokazaniaCards[pokazaniaCards.Count - 1].pokazanie = currentPokazanie.rawElectricity;
                pokazaniaCards[pokazaniaCards.Count - 1].forPeriod = currentPokazanie.electricity;
                pokazaniaCards[pokazaniaCards.Count - 1].period = i < 10 ? "0" + $"{i}.{year}" : $"{i}.{year}";

            }
            return pokazaniaCards;
        }

        public async Task<List<AppealModel>> GetAppeals(int userId)
        {
            HttpResponseMessage response = await client.GetAsync(Addresses.getAppeals + userId);
            string responseString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(responseString);
            List<AppealModel> appeals = JsonConvert.DeserializeObject<List<AppealModel>>(responseString);
            appeals.ForEach(appeal => appeal.BringToShowForm());
            return appeals;

        }

        public async Task<bool> PostAppeal(int userId, AppealModel appeal, int sntId)
        {
            Dictionary<string, string> requestDictionary = new Dictionary<string, string>
            {
                { "userId", userId.ToString() },
                { "theme", appeal.themeRaw },
                { "text", appeal.mainTextRaw },
                { "date", appeal.date },
                { "SntId", sntId.ToString() },
            };
            
            string serialized = System.Text.Json.JsonSerializer.Serialize(requestDictionary);
            StringContent requestBody = new StringContent(serialized, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(Addresses.postAppeal, requestBody);
            return response.IsSuccessStatusCode;
        }

        public async void SendPushToken(string token, string userId)
        {
            Dictionary<string, string> requestData = new Dictionary<string, string>
            {
                { "token", token },
                { "userId", userId.ToString() },
            };

            string serialized = System.Text.Json.JsonSerializer.Serialize(requestData);
            StringContent requestBody = new StringContent(serialized, Encoding.UTF8, "application/json");
            await client.PostAsync(Addresses.sendPushToken, requestBody);
        }
    }
}
