using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace chel
{
    public partial class FilmPage : Page, IFilterablePage

    {
        public FilmPage()
        {
            InitializeComponent();
            LoadMovies();

        }
        public void OnSearchChanged(string search)
        {
            ApplyFilter(search, GetCurrentSort());
        }

        public void OnSortChanged(string sort)
        {
            ApplyFilter(GetCurrentSearch(), sort);
        }
        private string GetCurrentSearch()
        {
            return (Application.Current.MainWindow as MainWindow)?.SearchBox.Text ?? "";
        }

        private string GetCurrentSort()
        {
            return ((Application.Current.MainWindow as MainWindow)?
                   .SortCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "TitleAsc";
        }

        private void LoadMovies()
        {
            var list = Core.Context.Movies.OrderBy(m => m.Title).ToList();
            MoviesItemsControl.ItemsSource = list;
        }

        private void OpenFilm_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                this.NavigationService.Navigate(new FilmDetailsPage(id));
            }
        }

        public void ApplyFilter(string search, string sortTag)
        {
            var movies = Core.Context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                movies = movies.Where(m => m.Title.Contains(search));

            switch (sortTag)
            {
                case "TitleAsc":
                    movies = movies.OrderBy(m => m.Title);
                    break;

                case "TitleDesc":
                    movies = movies.OrderByDescending(m => m.Title);
                    break;

                case "RatingAsc":
                    movies = movies.OrderBy(m => m.Rating);
                    break;

                case "RatingDesc":
                    movies = movies.OrderByDescending(m => m.Rating);
                    break;
            }

            MoviesItemsControl.ItemsSource = movies.ToList();
        }

    }
}
