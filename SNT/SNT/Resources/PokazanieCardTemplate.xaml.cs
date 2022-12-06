using SNT.Models;
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
	public partial class PokazanieCardTemplate : ContentView
	{
		public string pokazanie { get; set; }
		public string getsetPokazanie 
		{
			get { return pokazanie; }
			set
			{
                pokazanie = value;
				OnPropertyChanged(nameof(pokazanie));
			}
		}
        public string period { get; set; }
        public string getsetPeriod
        {
            get { return period; }
            set
            {
                period = value;
                OnPropertyChanged(nameof(period));
            }
        }
        public string forPeriod { get; set; }
        public string getsetForPeriod
        {
            get { return forPeriod; }
            set
            {
                forPeriod = value;
                OnPropertyChanged(nameof(forPeriod));
            }
        }
        
        public PokazanieCardTemplate(PokazanieCardModel data)
		{
			InitializeComponent();
            getsetPeriod = data.period;
            getsetForPeriod = data.forPeriod.ToString();
            getsetPokazanie = data.pokazanie.ToString();

		}

        
    }
}