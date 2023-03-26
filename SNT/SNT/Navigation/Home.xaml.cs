using MonkeyCache.FileStore;
using SNT.Models;
using SNT.Navigation;
using SNT.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SNT
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        DataRepository dataRepository = new DataRepository();
        List<int> uchastok = new List<int>();

        public class Refresh : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Execute(object parameter)
            {
                Home current = (Home) parameter;
                current.ForceGetAndSetDebt();
            }
        }

        public Home()
        {
            InitializeComponent();
            RefreshView.CommandParameter = this;
            RefreshView.Command = new Refresh();
            //Setup();
        }

        /*private async void Setup()
        {
            await SetUpUchastokPicker();
            new Thread(() => GetAndSetDebt((int)UchastokPicker.SelectedItem, false)).Start();
        }*/

        private void Clicked(object sender, EventArgs e)
        {
            if (App.Current.UserAppTheme == OSAppTheme.Dark)
            {
                App.Current.UserAppTheme = OSAppTheme.Light;
                Preferences.Set("DarkTheme", false);
            }
            else
            {
                App.Current.UserAppTheme = OSAppTheme.Dark;
                Preferences.Set("DarkTheme", true);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; //Ничего не делаем при нажатии кнопки назад
        }

        private async void WaterButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Electricity(true));
            }
            catch { }
        }

        private async void ElectricityButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Electricity(false));
            }
            catch { }
        }

        private async void PokazaniaButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Pokazania());
            }
            catch { }
        }

        public async void GetAndSetDebt(bool force_reload)
        {
            List<DebtCardModel> debts = new List<DebtCardModel>();

            try
            {
                int sntId = int.Parse(await SecureStorage.GetAsync("sntId"));
                int userId = int.Parse(await SecureStorage.GetAsync("userId"));
                List<UchastkiModel> uchastki = await dataRepository.GetUsersUchastki(userId);
                
                foreach (var item in uchastki)
                {
                    debts.Add(await dataRepository.GetDebts(int.Parse(item.uchastok), sntId));
                    
                }
            }
            catch
            {
                Device.BeginInvokeOnMainThread(() => this.DisplayToastAsync("Ошибка загрузки данных, проверьте интернет соединение"));
            }
            finally
            {
                Debug.WriteLine(debts);
                Debts.ItemsSource = debts;
                Device.BeginInvokeOnMainThread(() => RefreshView.IsRefreshing = false);
            }
        }

        public async void ForceGetAndSetDebt()
        {
            GetAndSetDebt(true);
        }

        private void SetDebtLabelText(DebtCardModel debtCard)
        {

        }

        /*private async Task<bool> SetUpUchastokPicker()
        {
            var userId = await SecureStorage.GetAsync("userId");
            List<UchastkiModel> uchastki = await dataRepository.GetUsersUchastki(int.Parse(userId));
            foreach (UchastkiModel u in uchastki)
            {
                uchastok.Add(int.Parse(u.uchastok));
            }
            UchastokPicker.ItemsSource = uchastok;
            UchastokPicker.SelectedIndex = 0;
            return true;
        }*/

        /*private void UchastokPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetAndSetDebt((int)UchastokPicker.SelectedItem, true);
        }*/

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Penality(Penality.Which.penality));
        }

        private async void Membership_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Penality(Penality.Which.membership));
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Penality(Penality.Which.target));
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new News());
        }

        private async void Button_Clicked_3(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Appeals());
        }
    }
}