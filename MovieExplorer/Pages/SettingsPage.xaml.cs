using MovieExplorer.Models;
using MovieExplorer.Services;

namespace MovieExplorer.Pages
{


    public partial class SettingsPage : ContentPage
    {
        private DataService dataService;
        private AppSettings settings;
        private UserData userData;

        public SettingsPage()
        {
            InitializeComponent();
            dataService = new DataService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadSettings();
        }

        private async Task LoadSettings()
        {
            settings = await dataService.LoadSettingsAsync();
            userData = await dataService.LoadUserDataAsync();

            // set controls to current values
            DarkModeSwitch.IsToggled = settings.DarkMode;
            //FontSizeSlider.Value = settings.FontSize;
            //FontSizeLabel.Text = settings.FontSize.ToString();
            NameEntry.Text = userData.UserName;
        }

        private async void DarkModeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            settings.DarkMode = e.Value;
            await dataService.SaveSettingsAsync(settings);

            // apply dark mode
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
            }
        }

        private async void SaveNameButton_Clicked(object sender, EventArgs e)
        {
            string newName = NameEntry.Text;
            if (!string.IsNullOrEmpty(newName))
            {
                userData.UserName = newName;
                await dataService.SaveUserDataAsync(userData);
                await DisplayAlert("Saved", "Name updated!", "OK");
            }
        }

        private async void ResetButton_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Warning",
                "This will delete all your data including favourites and history. Continue?",
                "Yes, Reset", "Cancel");

            if (confirm)
            {
                // reset everything
                userData = new UserData();
                settings = new AppSettings();
                await dataService.SaveUserDataAsync(userData);
                await dataService.SaveSettingsAsync(settings);
                await LoadSettings();
                await DisplayAlert("Done", "All data has been reset.", "OK");
            }
        }
    }
}