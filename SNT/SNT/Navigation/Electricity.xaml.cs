using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MonkeyCache.FileStore;
using SNT.Models;
using SNT.Repositories;
using SNT.Resources;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SNT.Navigation
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Electricity : ContentPage
	{

        public class Refresh : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object sender)
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
                Electricity current = (Electricity)parameter;
                current.ForceGetAndSetData();
                
            }

        }
        bool isWater = false;
        List<int> periods = new List<int>();
        List<int> uchastk = new List<int>();
        DataRepository dataRepository = new DataRepository();
        
        public Electricity (bool isWater)
		{
			InitializeComponent ();
            this.isWater = isWater;
            
            try
            {
                refreshView.Command = new Refresh();
                refreshView.CommandParameter = this;
                SetUpPeriodPicker();
                SetUpUchastokPicker();
            }
            catch
            {
                this.DisplayToastAsync("Ошибка загрузки данных, проверьте интернет соединение");
            }
            CardList.SeparatorColor = Color.Transparent;
            if (isWater) TitleLabel.Text = "Вода"; else TitleLabel.Text = "Электроэнергия";
        }

        private async void SoftGetAndSetData(int year, int uchastok)
        {
            try
            {
                string sntId = await SecureStorage.GetAsync("sntId");

                List<ElectricityCardModel> cards = null;
                if (!isWater)
                {
                    cards = await dataRepository.GetElectricityDebtCards(year, uchastok, int.Parse(sntId));
                }
                else
                {
                    cards = await dataRepository.GetWaterDebtCards(year, uchastok, int.Parse(sntId));
                }
                Device.BeginInvokeOnMainThread(() => DisplayData(cards));
            }
            catch
            {
                Device.BeginInvokeOnMainThread(() => this.DisplayToastAsync("Ошибка загрузки данных, проверьте интернет соединение"));
            }
        }

        public async void ForceGetAndSetData()
        {
            if (PeriodPicker.SelectedItem != null && UchastokPicker.SelectedItem != null)
            {
                Barrel.Current.EmptyAll();
                Device.BeginInvokeOnMainThread(() => SoftGetAndSetData((int)PeriodPicker.SelectedItem, (int)UchastokPicker.SelectedItem));   
                Debug.WriteLine("-----ПЕРЕЗАГРУЖАЮ-----");
                
            }
        }

        private void DisplayData(List<ElectricityCardModel> cards)
        {
            try
            {
                CardList.ItemsSource = cards;
                StackList.Children.Remove(LoadingIndicator);
                LoadingIndicator.IsRunning = false;
                refreshView.IsRefreshing = false;
            }
            catch { }
        }

        private async void SetUpUchastokPicker()
        {
            try
            {
                var userId = await SecureStorage.GetAsync("userId");
                List<UchastkiModel> uchastki = await dataRepository.GetUsersUchastki(int.Parse(userId));
                foreach (UchastkiModel u in uchastki)
                {
                    uchastk.Add(int.Parse(u.uchastok));
                }
                UchastokPicker.ItemsSource = uchastk;
                UchastokPicker.SelectedIndex = 0;
            } catch { }
        }

		private void SetUpPeriodPicker()
		{
            try
            {
                periods.Add((DateTime.Now.Year - 3) % 100);
                periods.Add((DateTime.Now.Year - 2) % 100);
                periods.Add((DateTime.Now.Year - 1) % 100);
                periods.Add(DateTime.Now.Year % 100);
                PeriodPicker.ItemsSource = periods;
                PeriodPicker.SelectedIndex = 3;
            }
            catch { }
        }

        private void NewPokazanieButton_Clicked(object sender, EventArgs e)
        {
			Navigation.PushAsync(new CreatePokazanie(isWater: false));
        }

        private void UchastokPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadingIndicator.IsRunning = true;
                if (PeriodPicker.SelectedItem != null && UchastokPicker.SelectedItem != null)
                {
                    SoftGetAndSetData((int)PeriodPicker.SelectedItem, (int)UchastokPicker.SelectedItem);
                }
            }
            catch { }
        }

        private void PeriodPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadingIndicator.IsRunning = true;
                if (PeriodPicker.SelectedItem != null && UchastokPicker.SelectedItem != null)
                {
                    SoftGetAndSetData((int)PeriodPicker.SelectedItem, (int)UchastokPicker.SelectedItem);
                }
            }
            catch { }
        }
    }
}
