using Microsoft.AspNetCore.Mvc;
using DT071G_Projekt.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DT071G_Projekt.Controllers
{
    public class ReviewController : Controller
    {
        private readonly string connectionString = "Data Source=RestaurantReviews.db;Version=3;";
        private static List<RestaurantReview> restaurantReviews = new List<RestaurantReview>();

        public IActionResult Index()
        {
            // Load reviews from the database
            List<RestaurantReview> reviews = LoadReviews();

            // Pass the reviews to the view
            return View(reviews);
        }

        public IActionResult Save()
        {
            // Save reviews logic
            SaveReviews();

            // Redirect to the Index action to display the updated reviews
            return RedirectToAction("Index");
        }

        public IActionResult WriteReview()
        {
            return View(); // Render the view for writing a review
        }
        public IActionResult ReviewsByRestaurant()
        {
            List<string> uniqueRestaurants = GetUniqueRestaurants();

            return View(uniqueRestaurants);
        }

        public IActionResult DisplayReviewsByRestaurant(string restaurant)
        {
            List<RestaurantReview> reviews = GetReviewsByRestaurant(restaurant);

            ViewData["Restaurant"] = restaurant;

            return View(reviews);
        }

        public IActionResult ReviewsByAuthor()
        {
            List<string> uniqueAuthors = GetUniqueAuthors();

            return View(uniqueAuthors);
        }

        public IActionResult DisplayReviewsByAuthor(string author)
        {
            List<RestaurantReview> reviews = GetReviewsByAuthor(author);

            ViewData["Author"] = author;

            return View(reviews);
        }

        private List<RestaurantReview> LoadReviews()
        {
            List<RestaurantReview> reviews = new List<RestaurantReview>();

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
                            RestaurantReview review = new RestaurantReview
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Review = Convert.ToString(reader["Review"]),
                                Author = Convert.ToString(reader["Author"]),
                                Restaurant = Convert.ToString(reader["Restaurant"]),
                            };

                            reviews.Add(review);
                        }
                    }
                }
            }

            return reviews;
        }

        private void SaveReviews()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var review in restaurantReviews)
                    {
                        using (SQLiteCommand insertCommand = new SQLiteCommand(
                            "INSERT OR REPLACE INTO Reviews (Id, Review, Author, Restaurant) VALUES (@Id, @Review, @Author, @Restaurant)",
                            connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@Id", review.Id);
                            insertCommand.Parameters.AddWithValue("@Review", review.Review);
                            insertCommand.Parameters.AddWithValue("@Author", review.Author);
                            insertCommand.Parameters.AddWithValue("@Restaurant", review.Restaurant);

                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        // Other helper methods for different functionalities (e.g., DeleteReview) can go here




        private void ReadReviews()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Reviews";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        Console.Clear();
                        Console.WriteLine("Restaurant Reviews:");

                        if (!reader.HasRows)
                        {
                            Console.WriteLine("\nNo reviews available.");
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["Author"]} was at {reader["Restaurant"]} and says: {reader["Review"]}");
                            }
                        }
                    }
                }
            }

            Console.WriteLine("\nPress any key to return.");
            Console.ReadKey();
        }

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
                                // Add other properties as needed
                            });
                        }
                    }
                }
            }

            return reviews;
        }

        private List<string> GetUniqueAuthors()
        {
            List<string> uniqueAuthors = new List<string>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT Author FROM Reviews";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uniqueAuthors.Add(reader["Author"].ToString());
                        }
                    }
                }
            }

            return uniqueAuthors;
        }

        private List<RestaurantReview> GetReviewsByAuthor(string author)
        {
            List<RestaurantReview> reviews = new List<RestaurantReview>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Reviews WHERE Author = @Author";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Author", author);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviews.Add(new RestaurantReview
                            {
                                Author = reader["Author"].ToString(),
                                Restaurant = reader["Restaurant"].ToString(),
                                Review = reader["Review"].ToString()
                                // Add other properties as needed
                            });
                        }
                    }
                }
            }

            return reviews;
        }

        public IActionResult SubmitReview(string userReview, string userName, string reviewedRestaurant)
        {
            if (!string.IsNullOrWhiteSpace(userReview))
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO Reviews (Review, Author, Restaurant) VALUES (@Review, @Author, @Restaurant)";
                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Review", userReview);
                        insertCommand.Parameters.AddWithValue("@Author", userName);
                        insertCommand.Parameters.AddWithValue("@Restaurant", reviewedRestaurant);

                        insertCommand.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Review added successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Review cannot be empty. Please try again.";
            }

            // Redirect back to the WriteReview page
            return RedirectToAction("WriteReview");
        }

        private void DeleteReview()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT * FROM Reviews";
                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.Clear();
                            Console.WriteLine("No reviews available.\n");
                            Console.WriteLine("Press any key to return.");
                            Console.ReadKey();
                            return;
                        }
                    }
                }

                do
                {
                    Console.Clear();
                    Console.WriteLine("Delete a restaurant review:");

                    List<RestaurantReview> reviewsFromDatabase = new List<RestaurantReview>();
                    using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                    {
                        using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                reviewsFromDatabase.Add(new RestaurantReview
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Review = reader["Review"].ToString(),
                                    Author = reader["Author"].ToString(),
                                    Restaurant = reader["Restaurant"].ToString(),
                                });
                            }
                        }
                    }

                    for (int i = 0; i < reviewsFromDatabase.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {reviewsFromDatabase[i].Restaurant}, {reviewsFromDatabase[i].Review} - By: {reviewsFromDatabase[i].Author}");
                    }

                    Console.Write("Enter the number of the review you want to delete:\n");

                    if (int.TryParse(Console.ReadLine(), out int reviewNumber) && reviewNumber >= 1 && reviewNumber <= reviewsFromDatabase.Count)
                    {
                        int reviewIdToDelete = reviewsFromDatabase[reviewNumber - 1].Id;

                        string deleteQuery = "DELETE FROM Reviews WHERE Id = @ReviewId";
                        using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@ReviewId", reviewIdToDelete);
                            deleteCommand.ExecuteNonQuery();
                        }

                        Console.WriteLine("\nReview deleted successfully.\n");
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter a valid number.\n");
                    }

                    Console.WriteLine("Press any key to enter a number again or press backspace to return to the main menu.\n");
                } while (Console.ReadKey(true).Key != ConsoleKey.Backspace);
            }
        }
    }
}
