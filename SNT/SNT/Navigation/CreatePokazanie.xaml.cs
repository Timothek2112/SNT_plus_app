using SNT.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SNT
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreatePokazanie : ContentPage
	{
        enum Selected
        {
            water,
            electricity
        }
        Selected selected;
        DataRepository dataRepository = new DataRepository();
        CalculateRepository calculateRepository = new CalculateRepository();
        List<int> uchastk = new List<int>();
		public Color electricityColor { get; set; }
		public Color getsetElectricityColor {
			get { return electricityColor; }
			set { electricityColor = value; OnPropertyChanged(nameof(electricityColor)); }
		}

        public Color waterColor { get; set; }
        public Color getsetWaterColor
        {
            get { return waterColor; }
            set { waterColor = value; OnPropertyChanged(nameof(waterColor)); }
        }

        public Color waterFrame { get; set; }
        public Color getsetWaterFrame
        {
            get { return waterFrame; }
            set { waterFrame = value; OnPropertyChanged(nameof(waterFrame)); }
        }

        public Color elecFrame { get; set; }
        public Color getsetElecFrame
        {
            get { return elecFrame; }
            set { elecFrame = value; OnPropertyChanged(nameof(elecFrame)); }
        }

        public CreatePokazanie(bool isWater)
		{
			InitializeComponent();
            try
            {
                SetUpPeriodPicker();
                SetUpUchastokPicker();
                SetUpMonthPicker();
            }
            catch
            {
                this.DisplayToastAsync("Ошибка загрузки данных, проверьте интернет соединение");
            }

            if (isWater) { SelectWater(); }
			else { SelectElectricity(); }
		}

        private void SelectWater()
        {
            selected = Selected.water;
            getsetWaterColor = (Color)Application.Current.Resources["SelectedColor"];
            getsetElectricityColor = (Color)Application.Current.Resources["DisselectedColor"];
            getsetWaterFrame = (Color)Application.Current.Resources["DisselectedColor"];
            getsetElecFrame = (Color)Application.Current.Resources["SelectedColor"];
            SelectedLabel.Text = "Вода";
        }

        private void SelectElectricity()
        {
            selected = Selected.electricity;
            getsetWaterColor = (Color)Application.Current.Resources["DisselectedColor"];
            getsetElectricityColor = (Color)Application.Current.Resources["SelectedColor"];
            getsetWaterFrame = (Color)Application.Current.Resources["SelectedColor"];
            getsetElecFrame = (Color)Application.Current.Resources["DisselectedColor"];
            SelectedLabel.Text = "Электроэнергия";
        }

        private void waterButton_Clicked(object sender, EventArgs e)
        {
            SelectWater();
        }

        private void electricityButton_Clicked(object sender, EventArgs e)
        {
           SelectElectricity();
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

        private async void SetUpPeriodPicker()
        {
            List<int> years = new List<int>();
            years.Add((DateTime.Now.Year - 3) % 100);
            years.Add((DateTime.Now.Year - 2) % 100);
            years.Add((DateTime.Now.Year - 1) % 100);
            years.Add(DateTime.Now.Year % 100);
            YearPicker.ItemsSource = years;
            YearPicker.SelectedIndex = 3;

        }

        private async void SetUpMonthPicker()
        {
            if(DateTime.Now.Month - 1 == 0)
            {
                monthPicker.SelectedIndex = 11;
                YearPicker.SelectedIndex -= 1;
            }
            else
            {
                monthPicker.SelectedIndex = DateTime.Now.Month - 1;

            }
        }

        internal async Task<bool> CreateElectricityPokazanie(int uchastok, int year, int month, float electricity)
        {
            string sntId = await SecureStorage.GetAsync("sntId");
            return await dataRepository.CreateElectricityPokazanie(uchastok, year, month, electricity, int.Parse(sntId));
        }

        internal async Task<bool> CreateWaterPokazanie(int uchastok, int year, int month, float water)
        {
            string sntId = await SecureStorage.GetAsync("sntId");
            return await dataRepository.CreateWaterPokazanie(uchastok, year, month, water, int.Parse(sntId));
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            bool isParsable = float.TryParse(PokazanieEntry.Text, out float a);
            if (isParsable && a < 0)
            {
                this.DisplayToastAsync("Показание не может быть меньше нуля");
            }

            try
            {
                if (isParsable)
                {
                    if (selected == Selected.electricity)
                    {
                        CreateElectricityPokazanie((int)UchastokPicker.SelectedItem, (int)YearPicker.SelectedItem, monthPicker.SelectedIndex + 1, float.Parse(PokazanieEntry.Text));
                    }
                    else
                    {
                        CreateWaterPokazanie((int)UchastokPicker.SelectedItem, (int)YearPicker.SelectedItem, monthPicker.SelectedIndex + 1, float.Parse(PokazanieEntry.Text));
                    }
                }
                else
                {
                    this.DisplayToastAsync("Строка показания не может быть пустой");
                }
            }
            catch
            {
                this.DisplayToastAsync("Ошибка загрузки данных, проверьте интернет соединение");
            }
        }
    }
}
