using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;

namespace chel
{
    /// <summary>
    /// Страница входа пользователя в систему.
    /// </summary>
    public partial class LoginPage : Page
    {
        private int _failedAttempts = 0;
        private string _currentCaptchaCode;

        public LoginPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Выполняет проверку учётных данных пользователя.
        /// </summary>
        /// <param name="login">Имя пользователя (логин).</param>
        /// <param name="password">Пароль.</param>
        /// <returns>
        /// <c>true</c>, если пользователь с указанными логином и паролем найден в базе данных;
        /// в этом случае свойство <see cref="Core.CurrentUser"/> устанавливается на найденного пользователя.
        /// <c>false</c>, если пользователь не найден.
        /// </returns>
        /// <remarks>
        /// Метод был выделен из обработчика <see cref="Login_Click"/> для обеспечения тестируемости.
        /// </remarks>
        public bool Auth(string login, string password)
        {
            var user = Core.Context.Users.FirstOrDefault(u => u.UserName == login && u.PasswordHash == password);
            if (user == null)
                return false;

            Core.CurrentUser = user;
            return true;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Войти".
        /// </summary>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Если уже были три неудачные попытки и капча не пройдена – блокируем вход
            if (_failedAttempts >= 3 && !ValidateCaptcha())
            {
                MessageBox.Show("Неверный код капчи. Попробуйте снова.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string login = UserNameBox.Text.Trim();
            string pass = PasswordBox.Password;

            if (Auth(login, pass))
            {
                _failedAttempts = 0;
                HideCaptcha();
                MessageBox.Show($"Добро пожаловать, {Core.CurrentUser.DisplayName ?? Core.CurrentUser.UserName}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new FilmPage());
            }
            else
            {
                _failedAttempts++;
                if (_failedAttempts >= 3)
                {
                    ShowCaptcha();
                }
                MessageBox.Show("Неверный логин или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Переход на главную страницу.
        /// </summary>
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.GoHome();
        }

        /// <summary>
        /// Показывает капчу после трёх неудачных попыток входа.
        /// </summary>
        private void ShowCaptcha()
        {
            _currentCaptchaCode = new Random().Next(1000, 9999).ToString();
            CaptchaImage.Source = GenerateCaptchaImage(_currentCaptchaCode);

            CaptchaText.Visibility = Visibility.Visible;
            CaptchaImage.Visibility = Visibility.Visible;
            CaptchaInput.Visibility = Visibility.Visible;
            VerifyCaptchaBtn.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Скрывает элементы капчи.
        /// </summary>
        private void HideCaptcha()
        {
            CaptchaText.Visibility = Visibility.Collapsed;
            CaptchaImage.Visibility = Visibility.Collapsed;
            CaptchaInput.Visibility = Visibility.Collapsed;
            VerifyCaptchaBtn.Visibility = Visibility.Collapsed;
            CaptchaInput.Text = "";
        }

        /// <summary>
        /// Проверяет введённый пользователем код капчи.
        /// </summary>
        /// <returns>
        /// <c>true</c>, если введённый код совпадает с текущим сгенерированным кодом;
        /// <c>false</c> в противном случае.
        /// </returns>
        private bool ValidateCaptcha()
        {
            return CaptchaInput.Text.Trim() == _currentCaptchaCode;
        }

        /// <summary>
        /// Обработчик кнопки проверки капчи.
        /// </summary>
        private void VerifyCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateCaptcha())
            {
                HideCaptcha();
                MessageBox.Show("Капча пройдена. Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Неверный код. Попробуйте ещё раз.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowCaptcha(); // обновляем капчу
            }
        }

        /// <summary>
        /// Генерирует изображение капчи с шумом.
        /// </summary>
        /// <param name="code">Код, который нужно отобразить.</param>
        /// <returns>Растровое изображение капчи.</returns>
        private BitmapSource GenerateCaptchaImage(string code)
        {
            // Размеры изображения
            int width = 150;
            int height = 50;
            var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var context = visual.RenderOpen())
            {
                // Заливка фона (светло-серый с небольшим шумом)
                var backgroundBrush = new SolidColorBrush(Colors.LightGray);
                context.DrawRectangle(backgroundBrush, null, new Rect(0, 0, width, height));

                // Добавляем случайные линии (шум)
                Random rand = new Random();
                for (int i = 0; i < 10; i++)
                {
                    var pen = new Pen(new SolidColorBrush(Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))), 1);
                    context.DrawLine(pen, new Point(rand.Next(width), rand.Next(height)), new Point(rand.Next(width), rand.Next(height)));
                }

                // Рисуем текст кода
                var text = new FormattedText(
                    code,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    24,
                    Brushes.DarkBlue,
                    VisualTreeHelper.GetDpi(visual).PixelsPerDip);
                context.DrawText(text, new Point(20, 10));

                // Добавляем точки-шумы
                for (int i = 0; i < 200; i++)
                {
                    context.DrawRectangle(Brushes.Black, null, new Rect(rand.Next(width), rand.Next(height), 1, 1));
                }
            }

            renderTarget.Render(visual);
            return renderTarget;
        }
    }
}