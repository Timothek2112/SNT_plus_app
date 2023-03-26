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
    public partial class SelectedAppeal : ContentPage
    {
        public SelectedAppeal(string theme, string mainText, string answer = "Ответа пока нет")
        {
            InitializeComponent();
            themeLabel.Text = theme;
            mainTextLabel.Text = mainText;
            answerTextLabel.Text = answer == "" ? "Ответа нет" : answer;
        }
    }
}
