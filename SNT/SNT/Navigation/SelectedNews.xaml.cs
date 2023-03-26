using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SNT.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectedNews : ContentPage
    {
        public SelectedNews(string title, string mainText)
        {
            InitializeComponent();
            titleLabel.Text = title;
            mainTextLabel.Text = mainText;
        }
    }
}