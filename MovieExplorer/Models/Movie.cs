using System.Text.Json.Serialization;

namespace MovieExplorer.Models
{
    //this is the movie class that holds movie data
    public class Movie
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("genre")]
        public List<string> Genre { get; set; } = new List<string>();

        [JsonPropertyName("director")]
        public string Director { get; set; } = "";

        [JsonPropertyName("rating")]
        public double ImdbRating { get; set; }

        [JsonPropertyName("emoji")]
        public string Emoji { get; set; } = "";

        // helper to get genres as a string
        public string GenreString => string.Join(", ", Genre);
    }
}
