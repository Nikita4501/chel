using System;
using System.Data.Entity.Validation;
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

        /// <summary>
        /// Регистрирует нового пользователя в системе.
        /// </summary>
        /// <param name="userName">Имя пользователя (логин).</param>
        /// <param name="password">Пароль.</param>
        /// <param name="displayName">Отображаемое имя.</param>
        /// <returns>true, если регистрация успешна; false в противном случае.</returns>
        public bool Register(string userName, string password, string displayName)
        {
            // Валидация на уровне приложения
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return false;

            if (Core.Context.Users.Any(u => u.UserName == userName))
                return false;

            var newUser = new Users
            {
                UserName = userName,
                PasswordHash = password,
                DisplayName = displayName,
                CreatedAt = DateTime.Now
            };

            Core.Context.Users.Add(newUser);
            try
            {
                Core.Context.SaveChanges();
                Core.CurrentUser = newUser;
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                // Логируем ошибки валидации (можно добавить в отладочных целях)
                // System.Diagnostics.Debug.WriteLine(ex.EntityValidationErrors);
                // Откатываем изменения, чтобы не оставлять объект в контексте
                Core.Context.Entry(newUser).State = System.Data.Entity.EntityState.Detached;
                return false;
            }
            catch (Exception)
            {
                // Любая другая ошибка (например, нарушение уникальности, проблемы с БД)
                Core.Context.Entry(newUser).State = System.Data.Entity.EntityState.Detached;
                return false;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (Register(UserNameBox.Text.Trim(), PasswordBox.Password, DisplayNameBox.Text.Trim()))
            {
                MessageBox.Show("Регистрация прошла успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new FilmPage());
            }
            else
            {
                MessageBox.Show("Ошибка регистрации. Логин уже существует, поля пусты или данные невалидны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.GoHome();
        }
    }
}