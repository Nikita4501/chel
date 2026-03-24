using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace chel
{
    public partial class FilmDetailsPage : Page
    {
        private int _movieId;
        private Movies _movie;

        public FilmDetailsPage(int movieId)
        {
            InitializeComponent();
            _movieId = movieId;
            LoadMovie();
        }

        private void LoadMovie()
        {
            _movie = Core.Context.Movies.FirstOrDefault(m => m.MovieId == _movieId);
            if (_movie == null)
            {
                MessageBox.Show("Фильм не найден");
                return;
            }

            this.DataContext = _movie;

            var genres = (from mg in Core.Context.MovieGenres
                          join g in Core.Context.Genres on mg.GenreId equals g.GenreId
                          where mg.MovieId == _movieId
                          select g.Name).ToList();
            GenresList.ItemsSource = genres;

            var sessions = Core.Context.Sessions
                           .Where(s => s.MovieId == _movieId)
                           .OrderBy(s => s.StartDateTime)
                           .ToList();

            var sessionViewModels = new List<SessionViewModel>();
            foreach (var s in sessions)
            {
                var hall = Core.Context.Halls.Find(s.HallId);
                var hallName = hall?.Name;
                var hallClass = hall?.Classification;
                sessionViewModels.Add(new SessionViewModel
                {
                    SessionId = s.SessionId,
                    StartDateTime = s.StartDateTime,
                    Price = s.Price,
                    HallName = hall?.Name ?? "—"
                });
            }

            SessionsList.ItemsSource = sessionViewModels;
        }


        private void OpenSession_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag is int sessionId)
            {
                if (Core.CurrentUser == null)
                {
                    MessageBox.Show("Для выбора места необходимо войти или зарегистрироваться.", "Требуется вход", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new LoginPage());
                    return;
                }

                NavigationService.Navigate(new SessionPage(sessionId));
            }
        }

        private class SessionViewModel
        {
            public int SessionId { get; set; }
            public System.DateTime StartDateTime { get; set; }
            public decimal Price { get; set; }
            public string HallName { get; set; }
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.GoHome();
        }

    }
}
