using System.Data.SQLite;

namespace DT071G_Projekt.Models
{

    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            // Skapar databasen om den inte finns
            if (!File.Exists("RestaurantReviews.db"))
            {
                SQLiteConnection.CreateFile("RestaurantReviews.db");
            }
        }
    }
    class RestaurantReview
    {
        public int Id { get; set; } // SQLite requires an auto-incrementing integer as the primary key
        public string Review { get; set; }
        public string Author { get; set; }
        public string Restaurant { get; set; }
    }

}
