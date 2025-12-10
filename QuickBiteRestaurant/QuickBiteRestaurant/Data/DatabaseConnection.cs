/*
 * FILE          : DatabaseConnection.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : This file contains the DatabaseConnection class which manages the connection to the MySQL database.
 */
using System;
using MySql.Data.MySqlClient;

namespace QuickBiteRestaurant.Data
{
    /// <summary>
    /// Manages the database connection for the QuickBite Fast Food Restaurant Management System.
    /// </summary>
    public class DatabaseConnection
    {
        // Connection string to connect to the MySQL database
        private static string connectionString ="Server=localhost;Port=3306;Database=QuickBiteFastFood;User=root;Password=Tellmyname@2005";
        /// <summary>
        /// Gets a new MySqlConnection instance.
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection GetConnection()
        {
            // Create and return a new MySqlConnection
            try
            {
                var conn = new MySqlConnection(connectionString);
                return conn;
            }
            // Catch any exceptions that occur during connection creation
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating connection: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Tests the database connection.
        /// </summary>
        /// <returns></returns>
        public static bool TestConnection()
        {
            try
            {
                // Attempt to open a connection to the database
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                // If an exception occurs, return false indicating the connection failed
                return false;
            }
        }
    }
}