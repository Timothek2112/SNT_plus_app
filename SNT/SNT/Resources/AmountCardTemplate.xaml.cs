using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SNT.Resources
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AmountCardTemplate : ContentView
	{
		private string MainText1 { get; set; } = "";
		public string MainText
		{
			get { return MainText1; }
			set { MainText1 = value; OnPropertyChanged("MainText1"); }
		}

        public AmountCardTemplate()
		{
			InitializeComponent();
		}

        
    }
}