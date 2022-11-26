using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SNT.Navigation;
namespace SNT
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new FlyoutMenu();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
