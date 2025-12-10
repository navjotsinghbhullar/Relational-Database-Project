/*
 * FILE          : CustomerOperations.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : This file contains the CustomerOperations class which manages customer-related functions.
 */
using System;
using MySql.Data.MySqlClient;

namespace QuickBiteRestaurant.Data
{
    public class CustomerOperations
    {
        /// <summary>
        /// Creates a new customer record in the database.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="loyaltyPoints"></param>
        public static void CreateCustomer(string firstName, string lastName, string email, string phoneNumber, int loyaltyPoints)
        {
            // Input validation
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Customer (FirstName, LastName, Email, PhoneNumber, 
                                    LoyaltyPoints, DateJoined) 
                                    VALUES (@FirstName, @LastName, @Email, @PhoneNumber, 
                                    @LoyaltyPoints, CURDATE())";
                    // Prepare the SQL command with parameters
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@LoyaltyPoints", loyaltyPoints);
                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"\nCustomer created successfully! ID: {cmd.LastInsertedId}");
                    }
                }
            }
            // Handle database-specific exceptions
            catch (MySqlException ex)
            {
                Console.WriteLine($"\n Error: {ex.Message}");
            }
            // Handle general exceptions
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Retrieves and displays customer details by ID.
        /// </summary>
        /// <param name="customerId"></param>
        public static void GetCustomerById(int customerId)
        {
            // Input validation
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to retrieve customer details along with order statistics
                    string query = @"SELECT *, 
                                    (SELECT COUNT(*) FROM `Order` WHERE CustomerID = @CustomerID) as TotalOrders,
                                    (SELECT SUM(TotalAmount) FROM `Order` WHERE CustomerID = @CustomerID) as TotalSpent
                                    FROM Customer WHERE CustomerID = @CustomerID";
                    // Prepare the SQL command with parameters
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerId);
                        // Execute the command and read the results
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Display customer details
                            if (reader.Read())
                            {
                                Console.WriteLine("\n========================================");
                                Console.WriteLine("=        Customer Details               =");
                                Console.WriteLine("=========================================");
                                Console.WriteLine($"ID:             {reader["CustomerID"]}");
                                Console.WriteLine($"Name:           {reader["FirstName"]} {reader["LastName"]}");
                                Console.WriteLine($"Email:          {reader["Email"]}");
                                Console.WriteLine($"Phone:          {reader["PhoneNumber"]}");
                                Console.WriteLine($"Loyalty Points: {reader["LoyaltyPoints"]}");
                                Console.WriteLine($"Date Joined:    {reader["DateJoined"]:yyyy-MM-dd}");
                                Console.WriteLine($"Total Orders:   {reader["TotalOrders"]}");
                                Console.WriteLine($"Total Spent:    ${reader["TotalSpent"]:F2}");
                            }
                            // If no customer found, display message
                            else
                            {
                                Console.WriteLine("\n Customer not found!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Retrieves and displays all customers with their order statistics.
        /// </summary>
        public static void GetAllCustomers()
        {
            // Input validation
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to retrieve all customers along with order statistics
                    string query = @"SELECT c.*, 
                                    COUNT(o.OrderID) as OrderCount,
                                    SUM(o.TotalAmount) as TotalSpent
                                    FROM Customer c
                                    LEFT JOIN `Order` o ON c.CustomerID = o.CustomerID
                                    GROUP BY c.CustomerID
                                    ORDER BY c.CustomerID";
                    // Prepare the SQL command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Display header
                        Console.WriteLine("\n=====================================================================================");
                        Console.WriteLine("=                             All Customers                                          =");
                        Console.WriteLine("======================================================================================");
                        Console.WriteLine($"{"ID",-5} {"Name",-25} {"Email",-30} {"Phone",-15} {"Points",-8} {"Orders",-8} {"Spent",-10}");
                        Console.WriteLine(new string('─', 115));
                        // Read and display each customer record
                        while (reader.Read())
                        {
                            string fullName = $"{reader["FirstName"]} {reader["LastName"]}";
                            Console.WriteLine($"{reader["CustomerID"],-5} {fullName,-25} {reader["Email"],-30} {reader["PhoneNumber"],-15} {reader["LoyaltyPoints"],-8} {reader["OrderCount"],-8} ${reader["TotalSpent"],-10:F2}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Updates an existing customer record in the database.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="loyaltyPoints"></param>
        public static void UpdateCustomer(int customerId, string firstName, string lastName, string email, string phoneNumber, int loyaltyPoints)
        {
            // Input validation
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to update customer details
                    string query = @"UPDATE Customer 
                                    SET FirstName = @FirstName, 
                                        LastName = @LastName,
                                        Email = @Email,
                                        PhoneNumber = @PhoneNumber,
                                        LoyaltyPoints = @LoyaltyPoints
                                    WHERE CustomerID = @CustomerID";
                    // Prepare the SQL command with parameters
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@LoyaltyPoints", loyaltyPoints);
                        cmd.Parameters.AddWithValue("@CustomerID", customerId);
                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\n Customer updated successfully!");
                        else
                            Console.WriteLine("\n Customer not found!");
                    }
                }
            }
            // Handle database-specific exceptions
            catch (MySqlException ex)
            {
                Console.WriteLine($"\n Error: {ex.Message}");
            }
            // Handle general exceptions
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Deletes a customer record from the database.
        /// </summary>
        /// <param name="customerId"></param>
        public static void DeleteCustomer(int customerId)
        {
            // Input validation
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to delete customer
                    string query = "DELETE FROM Customer WHERE CustomerID = @CustomerID";
                    // Prepare the SQL command with parameters
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameter
                        cmd.Parameters.AddWithValue("@CustomerID", customerId);
                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\n Customer deleted successfully!");
                        else
                            Console.WriteLine("\n Customer not found!");
                    }
                }
            }
            // Handle database-specific exceptions
            catch (MySqlException ex)
            {
                Console.WriteLine($"\n Error: {ex.Message}");
            }
            // Handle general exceptions
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Retrieves and displays the top customers based on total spending.
        /// </summary>
        /// <param name="limit"></param>
        public static void GetTopCustomers(int limit = 10)
        {
            // Input validation
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to retrieve top customers by total spending
                    string query = @"SELECT c.CustomerID, 
                                    CONCAT(c.FirstName, ' ', c.LastName) as Name,
                                    COUNT(o.OrderID) as TotalOrders,
                                    SUM(o.TotalAmount) as TotalSpent,
                                    c.LoyaltyPoints
                                    FROM Customer c
                                    LEFT JOIN `Order` o ON c.CustomerID = o.CustomerID
                                    GROUP BY c.CustomerID
                                    ORDER BY TotalSpent DESC
                                    LIMIT @Limit";
                    // Prepare the SQL command with parameters
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameter
                        cmd.Parameters.AddWithValue("@Limit", limit);
                        // Execute the command and read the results
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n====================================================================");
                            Console.WriteLine($"=                   Top {limit} Customers by Spending              =");
                            Console.WriteLine("====================================================================");
                            Console.WriteLine($"{"Rank",-5} {"Name",-25} {"Orders",-10} {"Total Spent",-15} {"Points",-10}");
                            Console.WriteLine(new string('─', 70));
                            // Read and display each top customer record
                            int rank = 1;
                            while (reader.Read())
                            {
                                Console.WriteLine($"{rank,-5} {reader["Name"],-25} {reader["TotalOrders"],-10} ${reader["TotalSpent"],-15:F2} {reader["LoyaltyPoints"],-10}");
                                rank++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error: {ex.Message}");
            }
        }
    }
}