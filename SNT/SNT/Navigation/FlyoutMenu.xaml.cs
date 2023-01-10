using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SNT.Themes;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace SNT.Navigation
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutMenu : Shell
    {
        GlobalConstants gc = new GlobalConstants();
        public string sa { get; set; } = "saa";
        public FlyoutMenu()
        {
            InitializeComponent();
            gc.setContext();
            BindingContext = gc;
        }

        private void Back_Button_Clicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;
        }

        

        private void themePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*LightTheme light = new LightTheme();
            DarkTheme dark = new DarkTheme();
            const int LIGHT_THEME = 0;
            const int DARK_THEME = 1;

            Picker picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;

            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();
                switch (selectedIndex)
                {
                    case LIGHT_THEME:
                        mergedDictionaries.Add(light);
                        break;

                    case DARK_THEME:
                        mergedDictionaries.Add(dark);
                        break;
                }
            }*/
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

        private void Create_Pokazanie_Clicked(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new CreatePokazanie());
            //Shell.Current.FlyoutIsPresented = false;
            //await Shell.Current.GoToAsync("//CreatePokazanie/CreatePokazanie");
        }
    }
}