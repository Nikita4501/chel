using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace chel
{
    public partial class CheckoutPage : Page
    {
        private int _sessionId;
        private List<Tuple<int, int>> _seats;
        private Sessions _session;

        public CheckoutPage(int sessionId, List<Tuple<int, int>> seats)
        {
            InitializeComponent();
            _sessionId = sessionId;
            _seats = seats;
            LoadInfo();
        }

        private void LoadInfo()
        {
            _session = Core.Context.Sessions.FirstOrDefault(s => s.SessionId == _sessionId);
            if (_session == null)
            {
                MessageBox.Show("Сеанс не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var movie = Core.Context.Movies.Find(_session.MovieId);
            var hall = Core.Context.Halls.Find(_session.HallId);

            var movieTitle = movie?.Title ?? "—";
            var hallName = hall?.Name ?? "—";
            var start = _session.StartDateTime;

            InfoText.Text = $"{movieTitle} — Зал: {hallName} — {start:dd.MM.yyyy HH:mm} — Цена за место: {_session.Price}";

            SelectedSeatsList.ItemsSource = _seats.Select(s => $"Ряд {s.Item1}, место {s.Item2}");
            decimal total = _seats.Count * _session.Price;
            TotalText.Text = $"Итого: {total} руб.";
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Требуется вход.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService.Navigate(new LoginPage());
                return;
            }

            var occupied = Core.Context.Tickets
                .Where(t => t.SessionId == _sessionId)
                .Select(t => new { t.RowNumber, t.SeatNumber })
                .AsEnumerable()
                .Select(x => Tuple.Create(x.RowNumber, x.SeatNumber))
                .ToHashSet();


            foreach (var s in _seats)
            {
                if (occupied.Contains(s))
                {
                    MessageBox.Show($"Место ряд {s.Item1} место {s.Item2} уже занято. Обновите страницу сеанса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    NavigationService.Navigate(new SessionPage(_sessionId));
                    return;
                }
            }

            foreach (var s in _seats)
            {
                var t = new Tickets
                {
                    UserId = Core.CurrentUser.UserId,
                    SessionId = _sessionId,
                    RowNumber = s.Item1,
                    SeatNumber = s.Item2,
                    Price = _session.Price,
                    PurchaseDate = DateTime.Now
                };
                Core.Context.Tickets.Add(t);
            }

            Core.Context.SaveChanges();

            MessageBox.Show("Билеты успешно куплены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService.Navigate(new FilmPage());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.GoHome();
        }

    }
}
