/*
 * FILE          : MenuCategoryOperations.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : Handles CRUD operations and reports for menu categories in the QuickBite restaurant system.
 */
using System;
using MySql.Data.MySqlClient;

namespace QuickBiteRestaurant.Data
{
    public class MenuCategoryOperations
    {
        /// <summary>
        /// Creates a new menu category.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="description"></param>
        public static void CreateCategory(string categoryName, string description)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to insert a new menu category
                    string query = @"INSERT INTO MenuCategory (CategoryName, Description) VALUES (@CategoryName, @Description)";
                    // Create a MySqlCommand to execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"\nMenu category created successfully! ID: {cmd.LastInsertedId}");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Gets a menu category by its ID.
        /// </summary>
        /// <param name="categoryId"></param>
        public static void GetCategoryById(int categoryId)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // SQL query to get menu category details along with item count and total value
                    string query = @"SELECT c.*, 
                                    COUNT(m.MenuItemID) as ItemCount,
                                    SUM(m.Price) as TotalValue
                                    FROM MenuCategory c
                                    LEFT JOIN MenuItem m ON c.CategoryID = m.CategoryID
                                    WHERE c.CategoryID = @CategoryID
                                    GROUP BY c.CategoryID";
                    // Create a MySqlCommand to execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        // Execute the command and read the results
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("\n========================================");
                                Console.WriteLine("=         Menu Category Details        =");
                                Console.WriteLine("========================================");
                                Console.WriteLine($"ID:          {reader["CategoryID"]}");
                                Console.WriteLine($"Name:        {reader["CategoryName"]}");
                                Console.WriteLine($"Description: {reader["Description"]}");
                                Console.WriteLine($"Items:       {reader["ItemCount"]}");
                                Console.WriteLine($"Total Value: ${reader["TotalValue"]:F2}");
                            }
                            else
                            {
                                Console.WriteLine("\nCategory not found!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Gets all menu categories with item counts and availability.
        /// </summary>
        public static void GetAllCategories()
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // SQL query to get all menu categories with item counts and available items
                    string query = @"SELECT c.*, 
                                    COUNT(m.MenuItemID) as ItemCount,
                                    SUM(CASE WHEN m.IsAvailable = TRUE THEN 1 ELSE 0 END) as AvailableItems
                                    FROM MenuCategory c
                                    LEFT JOIN MenuItem m ON c.CategoryID = m.CategoryID
                                    GROUP BY c.CategoryID
                                    ORDER BY c.CategoryName";
                    // Create a MySqlCommand to execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n================================================================================================");
                        Console.WriteLine("=                                   All Menu Categories                                         =");
                        Console.WriteLine("=================================================================================================");
                        Console.WriteLine($"{"ID",-5} {"Name",-20} {"Description",-40} {"Total Items",-12} {"Available",-12}");
                        Console.WriteLine(new string('─', 95));
                        // Read and display each category
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["CategoryID"],-5} {reader["CategoryName"],-20} {reader["Description"],-40} {reader["ItemCount"],-12} {reader["AvailableItems"],-12}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Updates an existing menu category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="categoryName"></param>
        /// <param name="description"></param>
        public static void UpdateCategory(int categoryId, string categoryName, string description)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to update the menu category
                    string query = @"UPDATE MenuCategory 
                                    SET CategoryName = @CategoryName,
                                        Description = @Description
                                    WHERE CategoryID = @CategoryID";
                    // Create a MySqlCommand to execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nCategory updated successfully!");
                        else
                            Console.WriteLine("\nCategory not found!");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Deletes a menu category by its ID.
        /// </summary>
        /// <param name="categoryId"></param>
        public static void DeleteCategory(int categoryId)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // Check if category has associated menu items
                    string checkQuery = "SELECT COUNT(*) FROM MenuItem WHERE CategoryID = @CategoryID";
                    // Create a MySqlCommand to execute the check query
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        // Add parameter to the command
                        checkCmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        int itemCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        // If items exist, prompt for confirmation
                        if (itemCount > 0)
                        {
                            Console.WriteLine($"\nCannot delete category with {itemCount} menu items!");
                            Console.Write("Delete all items and category? (yes/no): ");
                            // Get user confirmation
                            if (Console.ReadLine().ToLower() != "yes")
                            {
                                Console.WriteLine("Deletion cancelled.");
                                return;
                            }
                        }
                    }
                    // SQL query to delete the menu category
                    string query = "DELETE FROM MenuCategory WHERE CategoryID = @CategoryID";
                    // Create a MySqlCommand to execute the delete query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine("\nCategory deleted successfully!");
                        else
                            Console.WriteLine("\nCategory not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Generates a sales report for menu categories.
        /// </summary>
        /// <param name="categoryId"></param>
        public static void GetCategorySalesReport(int categoryId = 0)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to generate the sales report
                    string query = @"SELECT c.CategoryID, c.CategoryName,
                                    COUNT(oi.OrderItemID) as TimesOrdered,
                                    SUM(oi.Quantity) as TotalQuantity,
                                    SUM(oi.Subtotal) as TotalRevenue
                                    FROM MenuCategory c
                                    JOIN MenuItem m ON c.CategoryID = m.CategoryID
                                    JOIN OrderItem oi ON m.MenuItemID = oi.MenuItemID
                                    JOIN `Order` o ON oi.OrderID = o.OrderID
                                    WHERE (@CategoryID = 0 OR c.CategoryID = @CategoryID)
                                    AND o.OrderStatus != 'Cancelled'
                                    GROUP BY c.CategoryID
                                    ORDER BY TotalRevenue DESC";
                    // Create a MySqlCommand to execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameter to the command
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        // Execute the command and read the results
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n==========================================================================================");
                            Console.WriteLine("=                               Category Sales Report                                    =");
                            Console.WriteLine("==========================================================================================");
                            Console.WriteLine($"{"Category",-20} {"Orders",-12} {"Quantity",-12} {"Revenue",-15} {"Avg Price",-12}");
                            Console.WriteLine(new string('─', 75));
                            // Read and display each category's sales data
                            while (reader.Read())
                            {
                                decimal avgPrice = Convert.ToDecimal(reader["TotalRevenue"]) /
                                                 (Convert.ToInt32(reader["TimesOrdered"]) > 0 ?
                                                  Convert.ToInt32(reader["TimesOrdered"]) : 1);
                                Console.WriteLine($"{reader["CategoryName"],-20} {reader["TimesOrdered"],-12} {reader["TotalQuantity"],-12} ${reader["TotalRevenue"],-15:F2} ${avgPrice,-15:F2}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
    }
}