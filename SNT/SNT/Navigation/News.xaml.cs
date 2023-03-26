using MonkeyCache.FileStore;
using SNT.Models;
using SNT.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SNT.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class News : ContentPage
    {
        public class Refresh : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Execute(object parameter)
            {
                News current = (News)parameter;
                current.count = 10;
                current.GetAndSetNews(true);
                current.isEnd = false;
            }
        }
        DataRepository dataRepository = new DataRepository();
        public int count = 10;
        List<NewsModel> news = new List<NewsModel>();
        List<NewsModel> prevNews = new List<NewsModel>();
        public bool isEnd = false;
        public News()
        {
            InitializeComponent();
            refreshView.Command = new Refresh();
            refreshView.CommandParameter = this;
            GetAndSetNews(false);
            

            CardList.ItemAppearing += (sender, e) =>
            {
                if (news.Count == 0) return;
                if (isEnd) return;

                if (e.Item == news[news.Count - 1])
                {
                    prevNews = news;
                    count += 10;
                    GetAndSetNews(true);
                    if (prevNews == news) isEnd = true;
                    prevNews = news;
                }
            };
        }

        private async void GetAndSetNews(bool force_reload)
        {
            string sntId = await SecureStorage.GetAsync("sntId");
            news = await dataRepository.getNews(count, int.Parse(sntId));
            CardList.ItemsSource = news;
            refreshView.IsRefreshing = false;
            LoadingIndicator.IsRunning = false;
        }

        private async void NewsSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(CardList.SelectedItem != null)
            {
                NewsModel selectedNews = (NewsModel)CardList.SelectedItem;
                await Navigation.PushAsync(new SelectedNews(selectedNews.title, selectedNews.full_text));
            }
        }
    }
}