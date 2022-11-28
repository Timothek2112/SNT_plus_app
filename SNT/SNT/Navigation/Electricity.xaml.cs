using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			for(int i = 0; i< 10; i++)
			{
				AmountCardTemplate card = new AmountCardTemplate();
				card.MainText = i.ToString();
				CardList.Children.Add(card);
			}
		}
	}
}