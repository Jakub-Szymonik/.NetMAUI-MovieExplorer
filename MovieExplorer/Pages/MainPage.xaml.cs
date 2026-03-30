using MovieExplorer.Models;
using MovieExplorer.Services;

namespace MovieExplorer.Pages
{
    public partial class MainPage : ContentPage
    {
        private DataService dataService;
        private List<Movie> allMovies;
        private UserData userData;

        public MainPage()
        {
            InitializeComponent();
            dataService = new DataService();
            allMovies = new List<Movie>();
        }

        // runs when page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDataAsync();
        }

        // load all the data we need
        private async Task LoadDataAsync()
        {
            userData = await dataService.LoadUserDataAsync();

            // check if user needs to enter name
            if (string.IsNullOrEmpty(userData.UserName))
            {
                string name = await DisplayPromptAsync("Welcome", "Please enter your name:");
                if (!string.IsNullOrEmpty(name))
                {
                    userData.UserName = name;
                    await dataService.SaveUserDataAsync(userData);
                }
            }

            WelcomeLabel.Text = $"Welcome, {userData.UserName}!";

            // load movies
            allMovies = await dataService.GetMoviesAsync();
            MoviesList.ItemsSource = allMovies;

            // setup genre picker
            var genres = allMovies.SelectMany(m => m.Genre).Distinct().OrderBy(g => g).ToList();
            genres.Insert(0, "All Genres");
            GenrePicker.ItemsSource = genres;
            GenrePicker.SelectedIndex = 0;
        }

        // filter movies when search text changes
        private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterMovies();
        }

        // filter movies when genre changes
        private void GenrePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterMovies();
        }

        // filters the movie list based on search and genre
        private void FilterMovies()
        {
            var filtered = allMovies.AsEnumerable();

            // filter by search text
            string searchText = SearchEntry.Text?.ToLower() ?? "";
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(m => m.Title.ToLower().Contains(searchText));
            }

            // filter by genre
            if (GenrePicker.SelectedIndex > 0)
            {
                string selectedGenre = GenrePicker.SelectedItem?.ToString() ?? "";
                filtered = filtered.Where(m => m.Genre.Contains(selectedGenre));
            }

            MoviesList.ItemsSource = filtered.ToList();
        }

        // when a movie is selected show details
        private async void MoviesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0) return;

            var movie = e.CurrentSelection[0] as Movie;
            if (movie == null) return;

            // add to view history
            var viewedMovie = new ViewedMovie
            {
                Title = movie.Title,
                Year = movie.Year,
                Genre = movie.Genre,
                Emoji = movie.Emoji,
                ViewedDate = DateTime.Now
            };
            userData.ViewHistory.Add(viewedMovie);
            await dataService.SaveUserDataAsync(userData);

            // show movie details and ask to favourite
            string action = await DisplayActionSheet(
                $"{movie.Emoji} {movie.Title} ({movie.Year})\nDirector: {movie.Director}\nIMDB: {movie.ImdbRating}\nGenres: {movie.GenreString}",
                "Close",
                null,
                "Add to Favourites");

            if (action == "Add to Favourites")
            {
                // check if already favourite
                bool alreadyFav = userData.Favourites.Any(f => f.Title == movie.Title && f.Year == movie.Year);
                if (alreadyFav)
                {
                    await DisplayAlert("Info", "This movie is already in your favourites!", "OK");
                }
                else
                {
                    var fav = new FavouriteMovie
                    {
                        Title = movie.Title,
                        Year = movie.Year,
                        Genre = movie.Genre,
                        Emoji = movie.Emoji,
                        AddedDate = DateTime.Now
                    };
                    userData.Favourites.Add(fav);
                    await dataService.SaveUserDataAsync(userData);
                    await DisplayAlert("Success", "Added to favourites!", "OK");
                }
            }

            // clear selection
            MoviesList.SelectedItem = null;
        }
    }
}
