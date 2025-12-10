/*
 * FILE          : RestaurantOperations.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : This file contains methods to perform CRUD operations and reports
 */
using System;
using MySql.Data.MySqlClient;

namespace QuickBiteRestaurant.Data
{
    public class RestaurantOperations
    {
        /// <summary>
        /// Creates a new restaurant record in the database.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="city"></param>
        /// <param name="province"></param>
        /// <param name="postalCode"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="managerId"></param>
        public static void CreateRestaurant(string name, string address, string city, string province, string postalCode, string phoneNumber, int? managerId = null)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Restaurant (Name, Address, City, Province, 
                                    PostalCode, PhoneNumber, ManagerID) 
                                    VALUES (@Name, @Address, @City, @Province, 
                                    @PostalCode, @PhoneNumber, @ManagerID)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@City", city);
                        cmd.Parameters.AddWithValue("@Province", province);
                        cmd.Parameters.AddWithValue("@PostalCode", postalCode);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@ManagerID", managerId.HasValue ? (object)managerId.Value : DBNull.Value);
                        // Execute the insert command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"\nRestaurant created successfully! ID: {cmd.LastInsertedId}");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"\nDatabase Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Gets and displays details of a restaurant by its ID.
        /// </summary>
        /// <param name="restaurantId"></param>
        public static void GetRestaurantById(int restaurantId)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT r.*, 
                                    CONCAT(e.FirstName, ' ', e.LastName) as ManagerName,
                                    (SELECT COUNT(*) FROM Employee WHERE RestaurantID = r.RestaurantID) as EmployeeCount,
                                    (SELECT COUNT(*) FROM `Order` WHERE RestaurantID = r.RestaurantID) as TotalOrders,
                                    (SELECT SUM(TotalAmount) FROM `Order` WHERE RestaurantID = r.RestaurantID) as TotalRevenue
                                    FROM Restaurant r
                                    LEFT JOIN Employee e ON r.ManagerID = e.EmployeeID
                                    WHERE r.RestaurantID = @RestaurantID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RestaurantID", restaurantId);
                        // Execute the select command
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("\n========================================");
                                Console.WriteLine("=        Restaurant Details            =");
                                Console.WriteLine("========================================");
                                Console.WriteLine($"ID:            {reader["RestaurantID"]}");
                                Console.WriteLine($"Name:          {reader["Name"]}");
                                Console.WriteLine($"Address:       {reader["Address"]}");
                                Console.WriteLine($"City:          {reader["City"]}, {reader["Province"]}");
                                Console.WriteLine($"Postal Code:   {reader["PostalCode"]}");
                                Console.WriteLine($"Phone:         {reader["PhoneNumber"]}");
                                Console.WriteLine($"Manager:       {reader["ManagerName"]}");
                                Console.WriteLine($"Employees:     {reader["EmployeeCount"]}");
                                Console.WriteLine($"Total Orders:  {reader["TotalOrders"]}");
                                Console.WriteLine($"Total Revenue: ${reader["TotalRevenue"]:F2}");
                            }
                            else
                            {
                                Console.WriteLine("\nRestaurant not found!");
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

        public static void GetAllRestaurants()
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT r.*, 
                                    CONCAT(e.FirstName, ' ', e.LastName) as ManagerName,
                                    (SELECT COUNT(*) FROM Employee WHERE RestaurantID = r.RestaurantID) as EmployeeCount,
                                    (SELECT COUNT(*) FROM `Order` WHERE RestaurantID = r.RestaurantID) as TotalOrders
                                    FROM Restaurant r
                                    LEFT JOIN Employee e ON r.ManagerID = e.EmployeeID
                                    ORDER BY r.City, r.Name";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n=======================================================================================");
                        Console.WriteLine("                             All Restaurants                                          =");
                        Console.WriteLine("=======================================================================================");
                        Console.WriteLine($"{"ID",-5} {"Name",-25} {"City",-15} {"Phone",-15} {"Manager",-20} {"Employees",-10} {"Orders",-10}");
                        Console.WriteLine(new string('─', 105));
                        // Read and display each restaurant record
                        while (reader.Read())
                        {
                            string manager = reader["ManagerName"] != DBNull.Value ? reader["ManagerName"].ToString() : "No Manager";
                            Console.WriteLine($"{reader["RestaurantID"],-5} {reader["Name"],-25} {reader["City"],-15} {reader["PhoneNumber"],-15} {manager,-20} {reader["EmployeeCount"],-10} {reader["TotalOrders"],-10}");
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
        /// Updates an existing restaurant record in the database.
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <param name="name"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="address"></param>
        /// <param name="city"></param>
        /// <param name="province"></param>
        /// <param name="postalCode"></param>
        /// <param name="managerId"></param>
        public static void UpdateRestaurant(int restaurantId, string name, string phoneNumber, string address, string city, string province, string postalCode, int? managerId = null)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Restaurant 
                                    SET Name = @Name,
                                        PhoneNumber = @PhoneNumber,
                                        Address = @Address,
                                        City = @City,
                                        Province = @Province,
                                        PostalCode = @PostalCode,
                                        ManagerID = @ManagerID
                                    WHERE RestaurantID = @RestaurantID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@City", city);
                        cmd.Parameters.AddWithValue("@Province", province);
                        cmd.Parameters.AddWithValue("@PostalCode", postalCode);
                        cmd.Parameters.AddWithValue("@ManagerID", managerId.HasValue ? (object)managerId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@RestaurantID", restaurantId);
                        // Execute the update command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nRestaurant updated successfully!");
                        else
                            Console.WriteLine("\nRestaurant not found!");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"\nDatabase Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Deletes a restaurant record from the database.
        /// </summary>
        /// <param name="restaurantId"></param>
        public static void DeleteRestaurant(int restaurantId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Restaurant WHERE RestaurantID = @RestaurantID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RestaurantID", restaurantId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nRestaurant deleted successfully!");
                        else
                            Console.WriteLine("\nRestaurant not found!");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"\nDatabase Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Generates a sales report for a specific restaurant over the last 30 days.
        /// </summary>
        /// <param name="restaurantId"></param>
        public static void GetRestaurantSalesReport(int restaurantId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT 
                                    DATE(o.OrderDate) as SaleDate,
                                    COUNT(*) as TotalOrders,
                                    SUM(o.TotalAmount) as DailyRevenue,
                                    AVG(o.TotalAmount) as AverageOrderValue
                                    FROM `Order` o
                                    WHERE o.RestaurantID = @RestaurantID
                                    AND o.OrderStatus != 'Cancelled'
                                    GROUP BY DATE(o.OrderDate)
                                    ORDER BY SaleDate DESC
                                    LIMIT 30";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RestaurantID", restaurantId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n===================================================================");
                            Console.WriteLine("=               Restaurant Sales Report (Last 30 Days)            =");
                            Console.WriteLine("===================================================================");
                            Console.WriteLine($"{"Date",-15} {"Orders",-10} {"Revenue",-15} {"Avg Order",-15}");
                            Console.WriteLine(new string('─', 60));
                            // Read and display each sales record
                            decimal totalRevenue = 0;
                            int totalOrders = 0;
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["SaleDate"]:yyyy-MM-dd} {reader["TotalOrders"],-10} ${reader["DailyRevenue"],-15:F2} ${reader["AverageOrderValue"],-15:F2}");
                                totalRevenue += Convert.ToDecimal(reader["DailyRevenue"]);
                                totalOrders += Convert.ToInt32(reader["TotalOrders"]);
                            }
                            // Display totals
                            Console.WriteLine(new string('─', 60));
                            Console.WriteLine($"{"TOTAL:",-15} {totalOrders,-10} ${totalRevenue:F2}");
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