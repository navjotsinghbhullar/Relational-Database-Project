/*
 * FILE          : MenuItemOperations.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : This file contains the MenuItemOperations class which provides methods to perform CRUD operations
 */
using System;
using MySql.Data.MySqlClient;

namespace QuickBiteRestaurant.Data
{
    public class MenuItemOperations
    {
        /// <summary>
        /// Creates a new menu item in the database.
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="description"></param>
        /// <param name="price"></param>
        /// <param name="categoryId"></param>
        /// <param name="isAvailable"></param>
        /// <param name="calorieCount"></param>
        public static void CreateMenuItem(string itemName, string description, decimal price, int categoryId, bool isAvailable, int calorieCount)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // Insert new menu item
                    string query = @"INSERT INTO MenuItem (ItemName, Description, Price, CategoryID, 
                                    IsAvailable, CalorieCount) 
                                    VALUES (@ItemName, @Description, @Price, @CategoryID, 
                                    @IsAvailable, @CalorieCount)";
                    // Execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ItemName", itemName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);
                        cmd.Parameters.AddWithValue("@CalorieCount", calorieCount);
                        // Execute the insert command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"\nMenu item created successfully! ID: {cmd.LastInsertedId}");
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
        /// Retrieves a menu item by its ID.
        /// </summary>
        /// <param name="menuItemId"></param>
        public static void GetMenuItemById(int menuItemId)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT m.*, c.CategoryName,
                                    (SELECT COUNT(*) FROM OrderItem WHERE MenuItemID = m.MenuItemID) as TimesOrdered,
                                    (SELECT SUM(Quantity) FROM OrderItem WHERE MenuItemID = m.MenuItemID) as TotalQuantity
                                    FROM MenuItem m 
                                    JOIN MenuCategory c ON m.CategoryID = c.CategoryID 
                                    WHERE m.MenuItemID = @MenuItemID";
                    // Execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameter
                        cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                        // Execute the query
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Read and display the menu item details
                            if (reader.Read())
                            {
                                Console.WriteLine("\n=========================================");
                                Console.WriteLine("=           Menu Item Details           =");
                                Console.WriteLine("=========================================");
                                Console.WriteLine($"ID:          {reader["MenuItemID"]}");
                                Console.WriteLine($"Name:        {reader["ItemName"]}");
                                Console.WriteLine($"Description: {reader["Description"]}");
                                Console.WriteLine($"Price:       ${reader["Price"]:F2}");
                                Console.WriteLine($"Category:    {reader["CategoryName"]}");
                                Console.WriteLine($"Available:   {(Convert.ToBoolean(reader["IsAvailable"]) ? " Yes" : " No")}");
                                Console.WriteLine($"Calories:    {reader["CalorieCount"]}");
                                Console.WriteLine($"Times Ordered: {reader["TimesOrdered"]}");
                                Console.WriteLine($"Total Sold:    {reader["TotalQuantity"]}");
                            }
                            else
                            {
                                Console.WriteLine("\nMenu item not found!");
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
        /// Retrieves and displays all menu items.
        /// </summary>
        public static void GetAllMenuItems()
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT m.MenuItemID, m.ItemName, m.Price, c.CategoryName, 
                                    m.IsAvailable, m.CalorieCount,
                                    (SELECT COUNT(*) FROM OrderItem WHERE MenuItemID = m.MenuItemID) as TimesOrdered
                                    FROM MenuItem m 
                                    JOIN MenuCategory c ON m.CategoryID = c.CategoryID 
                                    ORDER BY c.CategoryName, m.ItemName";
                    // Execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n============================================================================");
                        Console.WriteLine("=                          Menu Items                                      =");
                        Console.WriteLine("============================================================================");
                        Console.WriteLine($"{"ID",-5} {"Item Name",-25} {"Price",-10} {"Category",-15} {"Calories",-10} {"Available",-10} {"Orders",-10}");
                        Console.WriteLine(new string('─', 95));
                        // Read and display each menu item
                        while (reader.Read())
                        {
                            string available = Convert.ToBoolean(reader["IsAvailable"]) ? " Yes" : " No";
                            Console.WriteLine($"{reader["MenuItemID"],-5} {reader["ItemName"],-25} ${reader["Price"],-9:F2} {reader["CategoryName"],-15} {reader["CalorieCount"],-10} {available,-10} {reader["TimesOrdered"],-10}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }

        public static void UpdateMenuItem(int menuItemId, string itemName, string description, decimal price, int categoryId, bool isAvailable, int calorieCount)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE MenuItem 
                                    SET ItemName = @ItemName,
                                        Description = @Description,
                                        Price = @Price,
                                        CategoryID = @CategoryID,
                                        IsAvailable = @IsAvailable,
                                        CalorieCount = @CalorieCount
                                    WHERE MenuItemID = @MenuItemID";
                    // Execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ItemName", itemName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);
                        cmd.Parameters.AddWithValue("@CalorieCount", calorieCount);
                        cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nMenu item updated successfully!");
                        else
                            Console.WriteLine("\nMenu item not found!");
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
        /// Toggles the availability status of a menu item.
        /// </summary>
        /// <param name="menuItemId"></param>
        public static void ToggleMenuItemAvailability(int menuItemId)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE MenuItem 
                                    SET IsAvailable = NOT IsAvailable 
                                    WHERE MenuItemID = @MenuItemID";
                    // Execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Get new status
                            string statusQuery = "SELECT IsAvailable FROM MenuItem WHERE MenuItemID = @MenuItemID";
                            using (MySqlCommand statusCmd = new MySqlCommand(statusQuery, conn))
                            {
                                statusCmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                                bool newStatus = Convert.ToBoolean(statusCmd.ExecuteScalar());
                                Console.WriteLine($"\nMenu item is now {(newStatus ? "AVAILABLE" : "UNAVAILABLE")}!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nMenu item not found!");
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
        /// Deletes a menu item from the database.
        /// </summary>
        /// <param name="menuItemId"></param>
        public static void DeleteMenuItem(int menuItemId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // Check for existing orders
                    string checkQuery = "SELECT COUNT(*) FROM OrderItem WHERE MenuItemID = @MenuItemID";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                        int orderCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (orderCount > 0)
                        {
                            Console.WriteLine($"\nCannot delete menu item with {orderCount} existing orders!");
                            return;
                        }
                    }
                    // Delete menu item
                    string query = "DELETE FROM MenuItem WHERE MenuItemID = @MenuItemID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nMenu item deleted successfully!");
                        else
                            Console.WriteLine("\nMenu item not found!");
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
        /// Generates a sales report for menu items over the past specified number of days.
        /// </summary>
        /// <param name="days"></param>
        public static void GetMenuItemSalesReport(int days = 30)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    string query = @"SELECT m.ItemName, c.CategoryName,
                                    COUNT(oi.OrderItemID) as TimesOrdered,
                                    SUM(oi.Quantity) as TotalQuantity,
                                    SUM(oi.Subtotal) as TotalRevenue,
                                    AVG(oi.Subtotal / oi.Quantity) as AvgPrice
                                    FROM MenuItem m
                                    JOIN MenuCategory c ON m.CategoryID = c.CategoryID
                                    JOIN OrderItem oi ON m.MenuItemID = oi.MenuItemID
                                    JOIN `Order` o ON oi.OrderID = o.OrderID
                                    WHERE o.OrderDate >= DATE_SUB(CURDATE(), INTERVAL @Days DAY)
                                    AND o.OrderStatus != 'Cancelled'
                                    GROUP BY m.MenuItemID
                                    ORDER BY TotalRevenue DESC
                                    LIMIT 20";
                    // Execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Days", days);
                        // Execute the query
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n======================================================================================");
                            Console.WriteLine($"=                      Top 20 Menu Items (Last {days} Days)                         =");
                            Console.WriteLine("======================================================================================");
                            Console.WriteLine($"{"Item",-25} {"Category",-15} {"Orders",-10} {"Quantity",-10} {"Revenue",-15} {"Avg Price",-12}");
                            Console.WriteLine(new string('─', 95));
                            // Read and display each menu item's sales data
                            int rank = 1;
                            while (reader.Read())
                            {
                                Console.WriteLine($"{rank,-3} {reader["ItemName"],-22} {reader["CategoryName"],-15} {reader["TimesOrdered"],-10} {reader["TotalQuantity"],-10} ${reader["TotalRevenue"],-15:F2} ${reader["AvgPrice"],-15:F2}");
                                rank++;
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