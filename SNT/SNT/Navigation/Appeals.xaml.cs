using SNT.Models;
using SNT.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class Appeals : ContentPage
    {
        const string THEME_IS_EMPTY = "Тема не может быть пустой, пожалуйста, заполните все поля";
        const string MAIN_TEXT_IS_EMPTY = "Основной текст не может быть пустым, пожалуйста, заполните все поля";
        const string ERROR_SENDING_APPEAL = "Невозможно отправить обращение, попробуйте позже";
        const string SUCCESSFULLY_SENDING_APPEAL = "Обращение отправлено, вы можете проследить за его статусом в разделе \"Существующие обращения \"";

        DataRepository dataRepository = new DataRepository();
        public Appeals()
        {
            InitializeComponent();
            MainEditor.BackgroundColor = Color.Transparent;
            ThemeEditor.BackgroundColor = Color.Transparent;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                DateTime now = DateTime.Now;
                string date = $"{now.Month}-{now.Day}-{now.Year}-{now.Hour}:{now.Minute}:{now.Second}.000Z";

                if (!CheckValid(ThemeEditor.Text))
                {
                    DisplayMessageOnMainThread(THEME_IS_EMPTY);
                }

                if (!CheckValid(MainEditor.Text))
                {
                    DisplayMessageOnMainThread(MAIN_TEXT_IS_EMPTY);
                }

                AppealModel appeal = new AppealModel(
                    ThemeEditor.Text,
                    MainEditor.Text,
                    false,
                    "",
                    date
                );

                string sntId = await SecureStorage.GetAsync("sntId");
                bool isSuccess = await dataRepository.PostAppeal(
                    int.Parse(
                        await SecureStorage.GetAsync("userId")),
                    appeal,
                    int.Parse(sntId)
                );
                
                if (isSuccess)
                {
                    DisplayMessageOnMainThread(SUCCESSFULLY_SENDING_APPEAL);
                }
                else
                {
                    DisplayMessageOnMainThread(ERROR_SENDING_APPEAL);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                DisplayMessageOnMainThread(ERROR_SENDING_APPEAL);
            }
        }

        private void DisplayMessageOnMainThread(string message)
        {
            Device.BeginInvokeOnMainThread(() => this.DisplayToastAsync(message));
        }

        private bool CheckValid(string data)
        {
            if(data != "" && data != null)
            {
                return true;
            }
            return false;
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ExistingAppeals());
        }
    }
}