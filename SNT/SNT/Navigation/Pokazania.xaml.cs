using SNT.Models;
using SNT.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SNT.Navigation
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Pokazania : ContentPage
	{
		public Pokazania ()
		{
			InitializeComponent ();

			for(int i = 12; i > 0; i--)
			{
				Random rnd = new Random();
				PokazanieCardModel data = new PokazanieCardModel ();
				data.period = $"{i}:22";
				data.pokazanie = rnd.NextDouble() * rnd.Next(0, 3000);
				data.forPeriod = rnd.NextDouble() * rnd.Next(0, 3000);
				PokazanieCardTemplate card = new PokazanieCardTemplate(data);
				CardList.Children.Add(card);
			}
		}
	}
}