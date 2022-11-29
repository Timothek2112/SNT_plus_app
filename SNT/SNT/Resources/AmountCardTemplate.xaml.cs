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
	public partial class AmountCardTemplate : ContentView
	{
		public string debt { get; set; }
		public string getsetDebt 
		{
			get { return debt; }
			set
			{
                debt = value;
				OnPropertyChanged(nameof(debt));
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
        public string payment { get; set; }
        public string getsetPayment
        {
            get { return payment; }
            set
            {
                payment = value;
                OnPropertyChanged(nameof(payment));
            }
        }
        public string amount { get; set; }
        public string getsetAmount
        {
            get { return amount; }
            set
            {
                amount = value;
                OnPropertyChanged(nameof(amount));
            }
        }
        public Color amountColor { get; set; } = Color.Green;
        public Color getsetAmountColor
        {
            get { return amountColor; }
            set { amountColor = value; OnPropertyChanged(nameof(amountColor)); }
        }
        
        public AmountCardTemplate(ElectricityCardModel data)
		{
			InitializeComponent();
            getsetPeriod = data.period;
            getsetAmount = data.amount.ToString();
            getsetDebt = data.debt.ToString();
            getsetPayment = data.payment.ToString();

		}

        
    }
}