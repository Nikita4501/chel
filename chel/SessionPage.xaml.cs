using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace chel
{
    public partial class SessionPage : Page
    {
        private int _sessionId;
        private Sessions _session;
        private List<Tuple<int, int>> _selectedSeats = new List<Tuple<int, int>>(); 

        public SessionPage(int sessionId)
        {
            InitializeComponent();
            _sessionId = sessionId;
            LoadSession();
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.GoHome();
        }

        private void LoadSession()
        {
            _session = Core.Context.Sessions.FirstOrDefault(s => s.SessionId == _sessionId);
            if (_session == null)
            {
                MessageBox.Show("Сеанс не найден");
                return;
            }

            var hall = Core.Context.Halls.Find(_session.HallId);

            var movie = Core.Context.Movies.Find(_session.MovieId);
            FilmTitle.Text = movie?.Title ?? "Фильм";
            SessionInfo.Text = $"{_session.StartDateTime:dd.MM.yyyy HH:mm} — Зал: {hall?.Name} ({hall?.Classification}) — Цена: {_session.Price}";

            var hallSeats = Core.Context.Seats.Where(s => s.HallId == hall.HallId).ToList();
            int rows = hallSeats.Any() ? hallSeats.Max(s => s.RowNumber) : 0;
            int cols = hallSeats.Any() ? hallSeats.Max(s => s.SeatNumber) : 0;

            var occupied = Core.Context.Tickets
                .Where(t => t.SessionId == _sessionId)
                .Select(t => new { t.RowNumber, t.SeatNumber }) 
                .AsEnumerable()
                .Select(x => Tuple.Create(x.RowNumber, x.SeatNumber))
                .ToHashSet();

            var rowsList = new List<List<SeatViewModel>>();
            for (int r = 1; r <= rows; r++)
            {
                var rowList = new List<SeatViewModel>();
                for (int c = 1; c <= cols; c++)
                {
                    bool isOccupied = occupied.Contains(Tuple.Create(r, c));
                    var vm = new SeatViewModel { Row = r, Col = c, IsOccupied = isOccupied, SeatLabel = $"{r}-{c}" };
                    rowList.Add(vm);
                }
                rowsList.Add(rowList);
            }

            SeatRows.ItemsSource = rowsList;
        }


        private void SeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Для выбора места необходимо войти.", "Требуется вход", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new LoginPage());
                return;
            }

            if (sender is Button btn && btn.Tag is SeatViewModel vm)
            {
                if (vm.IsOccupied)
                {
                    MessageBox.Show("Место уже занято.", "Недоступно", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var key = Tuple.Create(vm.Row, vm.Col);
                if (_selectedSeats.Contains(key))
                {
                    _selectedSeats.Remove(key);
                    btn.Background = SystemColors.ControlBrush;
                }
                else
                {
                    _selectedSeats.Add(key);
                    btn.Background = Brushes.LightGreen;
                }
            }
        }

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            _selectedSeats.Clear();
            LoadSession();
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSeats.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одно место.", "Выбор места", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            NavigationService.Navigate(new CheckoutPage(_sessionId, _selectedSeats));
        }

        public class SeatViewModel
        {
            public int Row { get; set; }
            public int Col { get; set; }
            public bool IsOccupied { get; set; }
            public string SeatLabel { get; set; }
        }

        private class AnonymousComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y)
            {
                var xa = x.GetType().GetProperty("RowNumber")?.GetValue(x);
                var xb = x.GetType().GetProperty("SeatNumber")?.GetValue(x);
                var ya = y.GetType().GetProperty("RowNumber")?.GetValue(y);
                var yb = y.GetType().GetProperty("SeatNumber")?.GetValue(y);
                return xa.Equals(ya) && xb.Equals(yb);
            }

            public int GetHashCode(object obj)
            {
                var a = obj.GetType().GetProperty("RowNumber")?.GetValue(obj);
                var b = obj.GetType().GetProperty("SeatNumber")?.GetValue(obj);
                return (a.GetHashCode() * 397) ^ b.GetHashCode();
            }
        }
    }
}
