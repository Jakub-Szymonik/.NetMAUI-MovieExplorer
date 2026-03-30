namespace MovieExplorer.Models
{
    // stores a favourite movie with when it was added
    public class FavouriteMovie
    {
        public string Title { get; set; } = "";
        public int Year { get; set; }
        public List<string> Genre { get; set; } = new List<string>();
        public string Emoji { get; set; } = "";
        public DateTime AddedDate { get; set; }
        public string GenreString => string.Join(", ", Genre);
    }
}
