using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace chel
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var login = UserNameBox.Text.Trim();
            var pass = PasswordBox.Password;
            var display = DisplayNameBox.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Введите имя пользователя и пароль.");
                return;
            }

            if (Core.Context.Users.Any(u => u.UserName == login))
            {
                MessageBox.Show("Пользователь с таким именем уже существует.");
                return;
            }

            var newUser = new Users
            {
                UserName = login,
                PasswordHash = pass,
                DisplayName = display
            };
            Core.Context.Users.Add(newUser);
            Core.Context.SaveChanges();

            Core.CurrentUser = newUser;
            MessageBox.Show("Регистрация прошла успешно.");
            NavigationService.Navigate(new FilmPage());
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.GoHome();
        }

    }
}
