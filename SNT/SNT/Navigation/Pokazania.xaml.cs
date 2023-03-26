using MonkeyCache.FileStore;
using SNT.Models;
using SNT.Repositories;
using SNT.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SNT.Navigation
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Pokazania : ContentPage
	{
        class Refresh : ICommand
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
                Pokazania current = (Pokazania)parameter;
                current.getPokazania(true);
            }
        }
        List<string> states = new List<string>();
        List<PokazanieCardModel> cards = new List<PokazanieCardModel>();
		DataRepository dataRepository = new DataRepository();
        List<int> uchastk = new List<int>();
        List<int> periods = new List<int>();
		public Pokazania ()
		{
			InitializeComponent ();
            refreshView.Command = new Refresh();
            refreshView.CommandParameter = this;
            try
            {
                SetUpPage();
            }
            catch
            {
                this.DisplayToastAsync("Ошибка загрузки данных, проверьте интернет соединение");
            }
        }

        private async void SetUpPage()
        {
            
            await SetUpUchastokPicker();
            await SetUpStatePicker();
            SetUpPeriodPicker();
            getPokazania(false);
        }

		private async void getPokazania(bool force_reload)
		{
            try
            {
                string sntId = await SecureStorage.GetAsync("sntId");
                if ((string)StatePicker.SelectedItem == "Вода")
                {
                    cards = await dataRepository.GetWaterPokazania((int)PeriodPicker.SelectedItem, (int)UchastokPicker.SelectedItem, int.Parse(sntId));
                }
                else
                {
                    cards = await dataRepository.GetElectricityPokazania((int)PeriodPicker.SelectedItem, (int)UchastokPicker.SelectedItem, int.Parse(sntId));
                }
                CardList.ItemsSource = cards;
                stack.Children.Remove(LoadingIndicator);
                refreshView.IsRefreshing = false;
            }
            catch
            {
                Device.BeginInvokeOnMainThread(() => this.DisplayToastAsync("Ошибка загрузки данных, проверьте интернет соединение"));
            }
		}

        private async Task SetUpStatePicker()
        {
            states.Add("Электроэнергия");
            states.Add("Вода");
            StatePicker.SelectedIndex = 1;
            StatePicker.ItemsSource = states;
        }

        private async Task SetUpUchastokPicker()
        {
            var userId = await SecureStorage.GetAsync("userId");
            List<UchastkiModel> uchastki = await dataRepository.GetUsersUchastki(int.Parse(userId));
            foreach (UchastkiModel u in uchastki)
            {
                uchastk.Add(int.Parse(u.uchastok));
            }
            UchastokPicker.ItemsSource = uchastk;
            UchastokPicker.SelectedIndex = 0;
        }

        private void SetUpPeriodPicker()
        {
            periods.Add((DateTime.Now.Year - 3) % 100);
            periods.Add((DateTime.Now.Year - 2) % 100);
            periods.Add((DateTime.Now.Year - 1) % 100);
            periods.Add(DateTime.Now.Year % 100);
            PeriodPicker.ItemsSource = periods;
            PeriodPicker.SelectedIndex = 3;
        }

        private void StatePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(PeriodPicker.SelectedItem != null && UchastokPicker.SelectedItem != null && StatePicker.SelectedItem != null) getPokazania(false);
        }

        private void UchastokPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(PeriodPicker.SelectedItem != null && UchastokPicker.SelectedItem != null && StatePicker.SelectedItem != null) getPokazania(false);
        }

        private void PeriodPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(PeriodPicker.SelectedItem != null && UchastokPicker.SelectedItem != null && StatePicker.SelectedItem != null) getPokazania(false);
        }
    }
}