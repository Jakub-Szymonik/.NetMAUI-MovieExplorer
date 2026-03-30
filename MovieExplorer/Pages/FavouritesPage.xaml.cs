using MovieExplorer.Models;
using MovieExplorer.Services;

namespace MovieExplorer.Pages
{

    public partial class FavouritesPage : ContentPage
    {
        private DataService dataService;
        private UserData userData;

        public FavouritesPage()
        {
            InitializeComponent();
            dataService = new DataService();
        }

        //reloading favourites in case new ones were added
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadFavourites();
        }

        //loading the favourite movies, newest first
        private async Task LoadFavourites()
        {
            userData = await dataService.LoadUserDataAsync();
            // show newest first
            FavouritesList.ItemsSource = userData.Favourites.OrderByDescending(f => f.AddedDate).ToList();
        }

        //removing a favourite movie
        private async void FavouritesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0) return;

            var fav = e.CurrentSelection[0] as FavouriteMovie;
            if (fav == null) return;
            //disploy alert to confirm removal
            bool remove = await DisplayAlert("Remove?", $"Do you want to remove {fav.Title} from favourites?", "Yes", "No");

            //removes the movie and refreshes the list
            if (remove)
            {
                userData.Favourites.RemoveAll(f => f.Title == fav.Title && f.Year == fav.Year);
                await dataService.SaveUserDataAsync(userData);
                await LoadFavourites();
            }

            FavouritesList.SelectedItem = null;
        }
    }
}