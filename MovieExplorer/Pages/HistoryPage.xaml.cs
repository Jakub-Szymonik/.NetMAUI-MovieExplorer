using MovieExplorer.Models;
using MovieExplorer.Services;

namespace MovieExplorer.Pages
{
    public partial class HistoryPage : ContentPage
    {
        private DataService dataService;
        private UserData userData;

        public HistoryPage()
        {
            InitializeComponent();
            dataService = new DataService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadHistory();
        }

        private async Task LoadHistory()
        {
            userData = await dataService.LoadUserDataAsync();
            // show most recent first
            HistoryList.ItemsSource = userData.ViewHistory.OrderByDescending(v => v.ViewedDate).ToList();
        }

        //clears the watched history after confirmation
        private async void ClearButton_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Clear History",
                "Are you sure you want to clear all view history?",
                "Yes", "No");

            if (confirm)
            {
                //remove all from the list and refresh the page
                userData.ViewHistory.Clear();
                await dataService.SaveUserDataAsync(userData);
                await LoadHistory();
            }
        }
    }
}