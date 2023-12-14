using Microsoft.AspNetCore.Mvc;
using DT071G_Projekt.Models;
using System.Data.SQLite;

namespace DT071G_Projekt.Controllers
{
    public class ReviewController : Controller
    {
        // Variabel för databasuppkoppling
        private readonly string connectionString = "Data Source=RestaurantReviews.db;Version=3;";
        // Lista som recenserionerna lagras i
        private static List<RestaurantReview> restaurantReviews = new List<RestaurantReview>();

        /*------------------------------------------------------------------------Returer till vyer---------------------------------------------------------------------------------*/
        public IActionResult AllReviews()
        {
            restaurantReviews = LoadReviews(); // Hämtar recensionerna från databasen och lägger de i en lista

            return View(restaurantReviews); // Lägger listan recensionerna till Vyn som returneras

        }
        public IActionResult WriteReview()
        {
            return View(); // Renderar vyn för att skriva en ny recension
        }
        public IActionResult ReviewsByRestaurant()
        {
            List<string> uniqueRestaurants = GetUniqueRestaurants(); //Använder funktionen som hämtar ut alla restauranger och lägger de i en lista

            return View(uniqueRestaurants); // Renderar vyn med alla restauranger
        }

        public IActionResult DisplayReviewsByRestaurant(string restaurant) 
        {
            restaurantReviews = GetReviewsByRestaurant(restaurant);  // Lägger alla recensionerna från vald restaurang i listan

            ViewData["Restaurant"] = restaurant; // Anger att det är just den valda restaurangen som är argument till funktionen

            return View(restaurantReviews); // Renderar vyn med recensioner för vald restaurang
        }

        public IActionResult ReviewsByAuthor()
        {
            List<string> uniqueAuthors = GetUniqueAuthors(); // Lägger alla recensenterna i en lista

            return View(uniqueAuthors); // Renderar vyn med alla skribenter
        }

        public IActionResult DisplayReviewsByAuthor(string author)
        {
            restaurantReviews = GetReviewsByAuthor(author);   // Lägger alla recensionerna från vald skribententen i listan

            ViewData["Author"] = author; // Anger att det är just den valda skribententen som är argument till funktionen

            return View(restaurantReviews); // Renderar vyn med recensioner för vald skribent
        }

        public IActionResult SubmitReview(string userReview, string userName, string reviewedRestaurant) // Funktionen som lägger till ny recension i databasen
        {
            if (!string.IsNullOrWhiteSpace(userReview))
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO Reviews (Review, Author, Restaurant) VALUES (@Review, @Author, @Restaurant)"; // SQL frågan som skickar in det till databasen
                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Review", userReview);
                        insertCommand.Parameters.AddWithValue("@Author", userName);
                        insertCommand.Parameters.AddWithValue("@Restaurant", reviewedRestaurant);

                        insertCommand.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Review added successfully."; // Bekräftelse-meddelande
            }
            else
            {
                TempData["ErrorMessage"] = "Review cannot be empty. Please try again."; // Om skribenten försöker skicka in en tom recension skrivs denna feltext ut
            }
            return RedirectToAction("WriteReview");
        }

        /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

        /*------------------------------------------ Funktioner/listor vars returer skickas av deras respektive IAction ovan till respektive vy -------------------------------------*/


        // Funktion/Lista som laddar/hämtar alla recensioner
        private List<RestaurantReview> LoadReviews()
        {
            List<RestaurantReview> reviews = new List<RestaurantReview>(); // Lista som recensionerna senare läggs i

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Reviews";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RestaurantReview review = new RestaurantReview // Skapar en ny instans av listan
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Review = Convert.ToString(reader["Review"]),
                                Author = Convert.ToString(reader["Author"]),
                                Restaurant = Convert.ToString(reader["Restaurant"]),
                            };

                            reviews.Add(review); // Lägger de hämtade recensionerna till listan
                        }
                    }
                }
            }

            return reviews;
        }

      

        // Funktion/Lista för att hämta ut lista med alla restaurangerna som finns i tabellen för alla recensioner
        private List<string> GetUniqueRestaurants()
        {
            List<string> uniqueRestaurants = new List<string>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT Restaurant FROM Reviews";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uniqueRestaurants.Add(reader["Restaurant"].ToString());
                        }
                    }
                }
            }

            return uniqueRestaurants;
        }

        // Funktion/Lista för att hämta ut alla recensioner för en vald restaurang
        private List<RestaurantReview> GetReviewsByRestaurant(string restaurant)
        {
            List<RestaurantReview> reviews = new List<RestaurantReview>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Reviews WHERE Restaurant = @Restaurant";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Restaurant", restaurant);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviews.Add(new RestaurantReview
                            {
                                Author = reader["Author"].ToString(),
                                Review = reader["Review"].ToString()
                            });
                        }
                    }
                }
            }

            return reviews;
        }
        // Funktion/Lista för att hämta ut lista med alla skribenterna som finns i tabellen för alla recensioner
        private List<string> GetUniqueAuthors()
        {
            List<string> uniqueAuthors = new List<string>(); // Skapar listan som skribenterna läggs i

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT Author FROM Reviews"; // SQL frågan som hämtar ut alla skribenterna

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uniqueAuthors.Add(reader["Author"].ToString()); // Lägger till alla skribenter som finns i listan
                        }
                    }
                }
            }

            return uniqueAuthors; // Returnerar listan med skribenterna
        }

        // Funktion/Lista för att hämta ut alla recensioner för en vald skribent
        private List<RestaurantReview> GetReviewsByAuthor(string author)
        {
            List<RestaurantReview> reviews = new List<RestaurantReview>(); // Skapar listan som den valda skribentens recensioner läggs i

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Reviews WHERE Author = @Author"; // SQL som hämtar ut endast alla recensioner där skribenten är densamma som den valda skribenten

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Author", author);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviews.Add(new RestaurantReview // Lägger till recenionerna från denna skribent till listan
                            {
                                Author = reader["Author"].ToString(),
                                Restaurant = reader["Restaurant"].ToString(),
                                Review = reader["Review"].ToString()
                            });
                        }
                    }
                }
            }
            return reviews;
        }
        /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    }
}
