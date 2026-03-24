using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace chel
{
    public partial class LoginPage : Page
    {
        public LoginPage() { InitializeComponent(); }

        public bool Auth(string login, string password)
        {
            var user = Core.Context.Users.FirstOrDefault(u => u.UserName == login && u.PasswordHash == password);
            if (user == null)
                return false;

            Core.CurrentUser = user;
            return true;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = UserNameBox.Text.Trim();
            string pass = PasswordBox.Password;

            if (Auth(login, pass))
            {
                MessageBox.Show($"Добро пожаловать, {Core.CurrentUser.DisplayName ?? Core.CurrentUser.UserName}!");
                NavigationService.Navigate(new FilmPage());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
        }

        private void Home_Click(object sender, RoutedEventArgs e) { /* ... */ }
    }
}
