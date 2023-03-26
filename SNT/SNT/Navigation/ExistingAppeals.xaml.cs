using SNT.Models;
using SNT.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SNT.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExistingAppeals : ContentPage
    {
        List<AppealModel> appeals = new List<AppealModel>();
        DataRepository dataRepository = new DataRepository();
        public ExistingAppeals()
        {
            InitializeComponent();
            SetAppeals();
        }

        private async void SetAppeals()
        {
            appeals.Clear();
            int userId = int.Parse(await SecureStorage.GetAsync("userId"));
            appeals = await dataRepository.GetAppeals(userId);
            appeals.Reverse();
            appealsList.ItemsSource = appeals;
        }

        private async void appealsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (appealsList.SelectedItem != null)
            {
                AppealModel selectedAppeal = (AppealModel)appealsList.SelectedItem;
                await Navigation.PushAsync(new SelectedAppeal(selectedAppeal.themeRaw, selectedAppeal.mainTextRaw, selectedAppeal.answer));
            }
            appealsList.SelectedItem = null;
        }
    }
}