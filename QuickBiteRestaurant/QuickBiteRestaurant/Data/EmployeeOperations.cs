/*
 * FILE          : EmployeeOperations.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : This file contains the EmployeeOperations class which manages employee-related functionalities.
 */
using System;
using MySql.Data.MySqlClient;

namespace QuickBiteRestaurant.Data
{
    public class EmployeeOperations
    {
        /// <summary>
        /// Creates a new employee record in the database.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="position"></param>
        /// <param name="hireDate"></param>
        /// <param name="salary"></param>
        /// <param name="restaurantId"></param>
        public static void CreateEmployee(string firstName, string lastName, string email, string phoneNumber, string position, DateTime hireDate, decimal salary, int restaurantId)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to insert a new employee record
                    string query = @"INSERT INTO Employee (FirstName, LastName, Email, PhoneNumber, 
                                    Position, HireDate, Salary, RestaurantID) 
                                    VALUES (@FirstName, @LastName, @Email, @PhoneNumber, 
                                    @Position, @HireDate, @Salary, @RestaurantID)";
                    // Create a MySqlCommand to execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Position", position);
                        cmd.Parameters.AddWithValue("@HireDate", hireDate);
                        cmd.Parameters.AddWithValue("@Salary", salary);
                        cmd.Parameters.AddWithValue("@RestaurantID", restaurantId);
                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"\n✓ Employee created successfully! ID: {cmd.LastInsertedId}");
                    }
                }
            }
            // Handle MySQL-specific exceptions
            catch (MySqlException ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
            // Handle general exceptions
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Retrieves and displays employee details by their ID.
        /// </summary>
        /// <param name="employeeId"></param>
        public static void GetEmployeeById(int employeeId)
        {
            // Establish a connection to the database
            try
            {
                // Open the connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to retrieve employee details
                    string query = @"SELECT e.*, r.Name as RestaurantName,
                                    CONCAT(m.FirstName, ' ', m.LastName) as ManagerName,
                                    (SELECT COUNT(*) FROM `Order` WHERE EmployeeID = e.EmployeeID) as OrdersProcessed,
                                    (SELECT SUM(TotalAmount) FROM `Order` WHERE EmployeeID = e.EmployeeID) as SalesGenerated
                                    FROM Employee e
                                    JOIN Restaurant r ON e.RestaurantID = r.RestaurantID
                                    LEFT JOIN Restaurant rm ON r.ManagerID = rm.ManagerID
                                    LEFT JOIN Employee m ON r.ManagerID = m.EmployeeID
                                    WHERE e.EmployeeID = @EmployeeID";
                    // Create a MySqlCommand to execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameter to the command
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        // Execute the command and read the results
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Display employee details if found
                            if (reader.Read())
                            {
                                Console.WriteLine("\n=======================================");
                                Console.WriteLine("=          Employee Details            =");
                                Console.WriteLine("========================================");
                                Console.WriteLine($"ID:            {reader["EmployeeID"]}");
                                Console.WriteLine($"Name:          {reader["FirstName"]} {reader["LastName"]}");
                                Console.WriteLine($"Email:         {reader["Email"]}");
                                Console.WriteLine($"Phone:         {reader["PhoneNumber"]}");
                                Console.WriteLine($"Position:      {reader["Position"]}");
                                Console.WriteLine($"Restaurant:    {reader["RestaurantName"]}");
                                Console.WriteLine($"Manager:       {reader["ManagerName"]}");
                                Console.WriteLine($"Hire Date:     {reader["HireDate"]:yyyy-MM-dd}");
                                Console.WriteLine($"Salary:        ${reader["Salary"]:F2}");
                                Console.WriteLine($"Orders:        {reader["OrdersProcessed"]}");
                                Console.WriteLine($"Sales:         ${reader["SalesGenerated"]:F2}");
                            }
                            else
                            {
                                Console.WriteLine("\nEmployee not found!");
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
        /// Retrieves and displays all employees with their details.
        /// </summary>
        public static void GetAllEmployees()
        {
            // Establish a connection to the database
            try
            {
                // Open the connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to retrieve all employees
                    string query = @"SELECT e.*, r.Name as RestaurantName,
                                    (SELECT COUNT(*) FROM `Order` WHERE EmployeeID = e.EmployeeID) as OrdersProcessed
                                    FROM Employee e
                                    JOIN Restaurant r ON e.RestaurantID = r.RestaurantID
                                    ORDER BY r.Name, e.Position, e.LastName";
                    // Create a MySqlCommand to execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n==============================================================================");
                        Console.WriteLine("=                         All Employees                                      =");
                        Console.WriteLine("==============================================================================");
                        Console.WriteLine($"{"ID",-5} {"Name",-25} {"Position",-15} {"Restaurant",-20} {"Phone",-15} {"Orders",-10} {"Salary"}");
                        // Print a separator line
                        Console.WriteLine(new string('─', 115));
                        // Read and display each employee's details
                        while (reader.Read())
                        {
                            string fullName = $"{reader["FirstName"]} {reader["LastName"]}";
                            Console.WriteLine($"{reader["EmployeeID"],-5} {fullName,-25} {reader["Position"],-15} {reader["RestaurantName"],-20} {reader["PhoneNumber"],-15} {reader["OrdersProcessed"],-10} ${reader["Salary"]:F2}");
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
        /// Updates an existing employee record in the database.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="position"></param>
        /// <param name="salary"></param>
        /// <param name="restaurantId"></param>
        public static void UpdateEmployee(int employeeId, string firstName, string lastName,
            string email, string phoneNumber, string position, decimal salary, int restaurantId)
        {
            // Input validation
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // SQL query to update the employee record
                    string query = @"UPDATE Employee 
                                    SET FirstName = @FirstName,
                                        LastName = @LastName,
                                        Email = @Email,
                                        PhoneNumber = @PhoneNumber,
                                        Position = @Position,
                                        Salary = @Salary,
                                        RestaurantID = @RestaurantID
                                    WHERE EmployeeID = @EmployeeID";
                    // Create a MySqlCommand to execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Position", position);
                        cmd.Parameters.AddWithValue("@Salary", salary);
                        cmd.Parameters.AddWithValue("@RestaurantID", restaurantId);
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nEmployee updated successfully!");
                        else
                            Console.WriteLine("\nEmployee not found!");
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
        /// Deletes an employee record from the database.
        /// </summary>
        /// <param name="employeeId"></param>
        public static void DeleteEmployee(int employeeId)
        {
            // Input validation
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    // Open the connection
                    conn.Open();
                    // Check if employee is a manager
                    string checkQuery = "SELECT COUNT(*) FROM Restaurant WHERE ManagerID = @EmployeeID";
                    // Create a MySqlCommand to execute the check query
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        // Add parameter to the command
                        checkCmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        int managerCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        // If employee is a manager, prevent deletion
                        if (managerCount > 0)
                        {
                            Console.WriteLine("\nCannot delete employee who is currently a restaurant manager!");
                            return;
                        }
                    }
                    // Delete employee
                    string query = "DELETE FROM Employee WHERE EmployeeID = @EmployeeID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameter to the command
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nEmployee deleted successfully!");
                        else
                            Console.WriteLine("\nEmployee not found!");
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
        /// Generates and displays a performance report for employees.
        /// </summary>
        /// <param name="restaurantId"></param>
        public static void GetEmployeePerformanceReport(int restaurantId = 0)
        {
            // Establish a connection to the database
            try
            {
                //  Open the connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // SQL query to retrieve employee performance data
                    string query = @"SELECT e.EmployeeID, 
                                    CONCAT(e.FirstName, ' ', e.LastName) as Name,
                                    e.Position,
                                    r.Name as Restaurant,
                                    COUNT(o.OrderID) as TotalOrders,
                                    SUM(o.TotalAmount) as TotalSales,
                                    AVG(o.TotalAmount) as AvgOrderValue,
                                    MIN(o.OrderDate) as FirstOrder,
                                    MAX(o.OrderDate) as LastOrder
                                    FROM Employee e
                                    JOIN Restaurant r ON e.RestaurantID = r.RestaurantID
                                    LEFT JOIN `Order` o ON e.EmployeeID = o.EmployeeID
                                    WHERE (@RestaurantID = 0 OR e.RestaurantID = @RestaurantID)
                                    AND o.OrderStatus != 'Cancelled'
                                    GROUP BY e.EmployeeID
                                    ORDER BY TotalSales DESC";
                    // Create a MySqlCommand to execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameter to the command
                        cmd.Parameters.AddWithValue("@RestaurantID", restaurantId);
                        // Execute the command and read the results
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n=========================================================================================================");
                            Console.WriteLine("=                                        Employee Performance Report                                     =");
                            Console.WriteLine("==========================================================================================================");
                            Console.WriteLine($"{"Name",-25} {"Position",-15} {"Restaurant",-20} {"Orders",-10} {"Total Sales",-15} {"Avg Order",-15}");
                            Console.WriteLine(new string('─', 105));
                            // Read and display each employee's performance data
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["Name"],-25} {reader["Position"],-15} {reader["Restaurant"],-20} {reader["TotalOrders"],-10} ${reader["TotalSales"]:F2} ${reader["AvgOrderValue"]:F2}");
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