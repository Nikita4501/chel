using System.Windows;

namespace chel
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new FilmPage());
        }


        private void GoProfile_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Сначала войдите или зарегистрируйтесь.");
                MainFrame.Navigate(new LoginPage());
                return;
            }

            MainFrame.Navigate(new ProfilePage());
        }


        private void GoLogin_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoginPage());
        }

        private void GoRegister_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RegisterPage());
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (MainFrame == null) return;
            var content = MainFrame.Content;
            if (content is IFilterablePage page)
            {
                page.OnSearchChanged(SearchBox.Text);
            }
        }

        private void SortCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MainFrame == null) return;
            var content = MainFrame.Content;
            if (content is IFilterablePage page)
            {
                if (SortCombo.SelectedItem is System.Windows.Controls.ComboBoxItem item && item.Tag is string tag)
                    page.OnSortChanged(tag);
            }
        }
        public void GoHome()
        {
            MainFrame.Navigate(new FilmPage());
        }

    }

    public interface IFilterablePage
    {
        void OnSearchChanged(string searchText);
        void OnSortChanged(string sortTag);
    }
}
