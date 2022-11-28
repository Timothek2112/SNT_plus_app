using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SNT.Navigation;
namespace SNT
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            
        }

        public async void OnLoginClick(object sender, EventArgs e)
        {
            //добавить логин
            await Shell.Current.GoToAsync("//Home");
            
        }
    }
}
