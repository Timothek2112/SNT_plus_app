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
    public partial class Penality : ContentPage
    {
        public enum Which
        {
            Null,
            membership,
            penality,
            target
        }

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
                Penality current = (Penality)parameter;
                current.ForceGetAndSetData();

            }

        }

        Which selected = Which.Null;
        List<int> periods = new List<int>();
        List<int> uchastk = new List<int>();
        DataRepository dataRepository = new DataRepository();

        public Penality(Which state)
        {
            InitializeComponent();
            selected = state;
            switch (state)
            {
                case Which.penality:
                    TitleLabel.Text = "Пеня";
                    break;

                case Which.target:
                    TitleLabel.Text = "Целевые";
                    break;
                
                case Which.membership:
                    TitleLabel.Text = "Членские";
                    break;
            }
            refreshView.Command = new Refresh();
            refreshView.CommandParameter = this;
            try
            {
                SetUpPeriodPicker();
                SetUpUchastokPicker();
            }
            catch
            {
                this.DisplayToastAsync("Ошибка загрузки данных, проверьте интернет соединение");
            }
            
            CardList.SeparatorColor = Color.Transparent;
        }

        private async void GetAndSetData(int year, int uchastok, bool force_reload)
        {
            try
            {
                string sntId = await SecureStorage.GetAsync("sntId");

                List<ElectricityCardModel> cards = null;

                switch (selected)
                {
                    case Which.penality:
                        cards = await dataRepository.GetPenalityDebtCards(year, uchastok, int.Parse(sntId));
                        break;
                    case Which.membership:
                        cards = await dataRepository.GetMembershipDebtCards(year, uchastok, int.Parse(sntId));
                        break;
                    case Which.target:
                        cards = await dataRepository.GetTargetDebtCards(year, uchastok, int.Parse(sntId));
                        break;
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
                Device.BeginInvokeOnMainThread(() => GetAndSetData((int)PeriodPicker.SelectedItem, (int)UchastokPicker.SelectedItem, true));

            }
        }

        private void DisplayData(List<ElectricityCardModel> cards)
        {
            CardList.ItemsSource = cards;
            StackList.Children.Remove(LoadingIndicator);
            LoadingIndicator.IsRunning = false;
            refreshView.IsRefreshing = false;
        }

        private async void SetUpUchastokPicker()
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

        private void NewPokazanieButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CreatePokazanie(isWater: false));
        }

        private void UchastokPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadingIndicator.IsRunning = true;
            if (PeriodPicker.SelectedItem != null && UchastokPicker.SelectedItem != null)
            {
                GetAndSetData((int)PeriodPicker.SelectedItem, (int)UchastokPicker.SelectedItem, false);
            }
        }

        private void PeriodPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadingIndicator.IsRunning = true;
            if (PeriodPicker.SelectedItem != null && UchastokPicker.SelectedItem != null)
            {
                GetAndSetData((int)PeriodPicker.SelectedItem, (int)UchastokPicker.SelectedItem, false);
            }
        }
    }
}
