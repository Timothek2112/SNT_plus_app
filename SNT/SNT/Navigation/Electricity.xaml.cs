using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNT.Models;
using SNT.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SNT.Navigation
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Electricity : ContentPage
	{
		public Electricity ()
		{
			InitializeComponent ();
			//Симуляция заполнения данных
			for(int i = 12; i > 0; i--)
			{
				Random rand = new Random ();	
				
				ElectricityCardModel data = new ElectricityCardModel();
				data.period = $"{i}.22";
				data.debt = Math.Round(rand.NextDouble() * rand.Next(0, 3000),2);
				data.payment = Math.Round((rand.NextDouble() * rand.Next(0, 3000)), 2);
				data.SetAmount();
                AmountCardTemplate card = new AmountCardTemplate(data);
                if (data.amount < 0)
				{
					card.getsetAmountColor = Color.Red;
					card.getsetAmount = Math.Abs(data.amount).ToString();
				}

				CardList.Children.Add(card);
			}
		}
	}
}
