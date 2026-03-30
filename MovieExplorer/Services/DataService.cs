using MovieExplorer.Models;
using System.Text.Json;

namespace MovieExplorer.Services
{


    public class DataService
    {
        private static readonly string moviesFile = Path.Combine(FileSystem.AppDataDirectory, "movies.json");
        private static readonly string userDataFile = Path.Combine(FileSystem.AppDataDirectory, "userdata.json");
        private static readonly string settingsFile = Path.Combine(FileSystem.AppDataDirectory, "settings.json");

        private static readonly string moviesUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/refs/heads/main/moviesemoji.json";

        //downlading the movie details from the link or loads from cache
        public async Task<List<Movie>> GetMoviesAsync()
        {
            try
            {
                //check if we cached the data before
                if (File.Exists(moviesFile))
                {
                    string json = await File.ReadAllTextAsync(moviesFile);
                    var movies = JsonSerializer.Deserialize<List<Movie>>(json);
                    if (movies != null)
                        return movies;
                }

                //cache the data locally
                using HttpClient client = new HttpClient();
                string data = await client.GetStringAsync(moviesUrl);

                // save to cache
                await File.WriteAllTextAsync(moviesFile, data);

                var downloadedMovies = JsonSerializer.Deserialize<List<Movie>>(data);
                return downloadedMovies ?? new List<Movie>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting movies: {ex.Message}");
                return new List<Movie>();
            }
        }

        //loads user data from file
        public async Task<UserData> LoadUserDataAsync()
        {
            try
            {
                if (File.Exists(userDataFile))
                {
                    string json = await File.ReadAllTextAsync(userDataFile);
                    var data = JsonSerializer.Deserialize<UserData>(json);
                    if (data != null)
                        return data;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user data: {ex.Message}");
            }
            return new UserData();
        }

        //saving user data to file
        public async Task SaveUserDataAsync(UserData userData)
        {
            try
            {
                string json = JsonSerializer.Serialize(userData);
                await File.WriteAllTextAsync(userDataFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving user data: {ex.Message}");
            }
        }

        //loading settings from file
        public async Task<AppSettings> LoadSettingsAsync()
        {
            try
            {
                if (File.Exists(settingsFile))
                {
                    string json = await File.ReadAllTextAsync(settingsFile);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings != null)
                        return settings;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
            return new AppSettings();
        }

        //saving settings to file
        public async Task SaveSettingsAsync(AppSettings settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings);
                await File.WriteAllTextAsync(settingsFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
    }

}
