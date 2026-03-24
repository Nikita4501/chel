using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace chel
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadProfile();
        }

        private void LoadProfile()
        {
            TicketsList.Items.Clear();
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Вы не вошли. Войдите, пожалуйста.");
                var main = Application.Current.MainWindow as MainWindow;
                main?.MainFrame?.Navigate(new LoginPage());
                return;
            }

            NameText.Text = Core.CurrentUser.DisplayName ?? Core.CurrentUser.UserName;

            var tickets = Core.Context.Tickets
                          .Where(t => t.UserId == Core.CurrentUser.UserId)
                          .OrderByDescending(t => t.PurchaseDate)
                          .ToList()
                          .Select(t =>
                          {
                              var session = Core.Context.Sessions.Find(t.SessionId);
                              string movieTitle = "N/A";
                              string sessionInfo = "";
                              if (session != null)
                              {
                                  var movie = Core.Context.Movies.Find(session.MovieId);
                                  var hall = Core.Context.Halls.Find(session.HallId);
                                  movieTitle = movie?.Title ?? "N/A";
                                  sessionInfo = $"{session.StartDateTime:dd.MM.yyyy HH:mm}, Зал: {hall?.Name ?? "—"}";
                              }

                              return new
                              {
                                  MovieTitle = movieTitle,
                                  SessionInfo = sessionInfo,
                                  SeatInfo = $"Ряд {t.RowNumber}, место {t.SeatNumber}",
                                  PriceInfo = $"Цена: {t.Price} руб."
                              };
                          });

            TicketsList.ItemsSource = tickets;
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MainFrame?.Navigate(new FilmPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;

            var main = Application.Current.MainWindow as MainWindow;
            main?.MainFrame?.Navigate(new LoginPage());
        }


    }
}
