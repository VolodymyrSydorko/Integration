using CNNDesktop.Models;
using DTO;
using PropertyChanged;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace CNNDesktop.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<News> VisibleNews { get; set; } = new ObservableCollection<News>();

        public ObservableCollection<News> News { get; set; } = new ObservableCollection<News>();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public ReactiveCommand<Unit, Unit> ShowAllNewsCommand { get; }

        public ReactiveCommand<Unit, Unit> ShowLikedNewsCommand { get; }

        public ReactiveCommand<object, Unit> LikeCommand { get; }
        public ReactiveCommand<object, Unit> DislikeCommand { get; }

        public News SelectedNewsItem
        {
            get;
            set;
        }

        public MainWindowViewModel()
        {
            LoadCommand = ReactiveCommand.CreateFromTask(LoadNews);
            ShowAllNewsCommand = ReactiveCommand.Create(ShowAllNews);
            ShowLikedNewsCommand = ReactiveCommand.Create(ShowLikedNews);
            LikeCommand = ReactiveCommand.CreateFromTask<object>(LikeNews);
            DislikeCommand = ReactiveCommand.CreateFromTask<object>(DislikeNews);
        }

        private async Task LoadNews()
        {
            try
            {
                var response = await GraphqlClient.GetAllNews();
                News = new ObservableCollection<News>();

                foreach (var news in response ?? new List<News>())
                {
                    News.Add(news);
                }
                VisibleNews = new ObservableCollection<News>(News);
            }
            catch { }
        }

        private async void ShowAllNews()
        {
            try
            {
                if (News.Count == 0)
                {
                    await LoadNews();
                }
                VisibleNews = new ObservableCollection<News>(News);
            }
            catch { }
        }

        private async void ShowLikedNews()
        {
            try
            {
                if (News.Count == 0)
                {
                    await LoadNews();
                }
                VisibleNews = new ObservableCollection<News>(News.Where(x => x.IsLiked));
            }
            catch { }
        }

        private async Task DislikeNews(object arg)
        {
            try
            {
                if (arg is News news)
                {
                    var response = await GraphqlClient.UpdateNews(news.Id, false);
                    news.IsLiked = response.IsLiked;
                }
            }
            catch { }
        }

        private async Task LikeNews(object arg)
        {
            try
            {
                if (arg is News news)
                {
                    var response = await GraphqlClient.UpdateNews(news.Id, true);
                    news.IsLiked = response.IsLiked;
                }
            }
            catch { }
        }
    }
}
