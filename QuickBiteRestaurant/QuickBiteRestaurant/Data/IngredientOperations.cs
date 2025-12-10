/*
 * FILE          : IngredientOperations.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : This file contains the IngredientOperations class which manages ingredient-related functionalities.
 */
using System;
using MySql.Data.MySqlClient;

namespace QuickBiteRestaurant.Data
{
    public class IngredientOperations
    {
        /// <summary>
        /// Creates a new ingredient record in the database.
        /// </summary>
        /// <param name="ingredientName"></param>
        /// <param name="unit"></param>
        /// <param name="reorderLevel"></param>
        /// <param name="currentStock"></param>
        /// <param name="unitPrice"></param>
        /// <param name="supplierId"></param>
        public static void CreateIngredient(string ingredientName, string unit, decimal reorderLevel, decimal currentStock, decimal unitPrice, int supplierId)
        {
            // Establish a connection to the database
            try
            {
                // Open database connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // SQL query to insert a new ingredient
                    string query = @"INSERT INTO Ingredient (IngredientName, Unit, ReorderLevel, 
                                    CurrentStock, UnitPrice, SupplierID) 
                                    VALUES (@IngredientName, @Unit, @ReorderLevel, @CurrentStock, 
                                    @UnitPrice, @SupplierID)";
                    // Prepare and execute the SQL command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IngredientName", ingredientName);
                        cmd.Parameters.AddWithValue("@Unit", unit);
                        cmd.Parameters.AddWithValue("@ReorderLevel", reorderLevel);
                        cmd.Parameters.AddWithValue("@CurrentStock", currentStock);
                        cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                        cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                        // Execute the command and get the number of affected rows
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"\nIngredient created successfully! ID: {cmd.LastInsertedId}");
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
        /// Retrieves and displays ingredient details by ID.
        /// </summary>
        /// <param name="ingredientId"></param>
        public static void GetIngredientById(int ingredientId)
        {
            // Establish a connection to the database
            try
            {
                // Open database connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // SQL query to fetch ingredient details
                    string query = @"SELECT i.*, s.SupplierName,
                                    (SELECT COUNT(*) FROM MenuItemIngredient WHERE IngredientID = i.IngredientID) as UsedInMenuItems,
                                    (SELECT GROUP_CONCAT(m.ItemName SEPARATOR ', ') 
                                     FROM MenuItem m
                                     JOIN MenuItemIngredient mi ON m.MenuItemID = mi.MenuItemID
                                     WHERE mi.IngredientID = i.IngredientID) as MenuItems
                                    FROM Ingredient i
                                    JOIN Supplier s ON i.SupplierID = s.SupplierID
                                    WHERE i.IngredientID = @IngredientID";
                    // Prepare and execute the SQL command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameter for ingredient ID
                        cmd.Parameters.AddWithValue("@IngredientID", ingredientId);
                        // Execute the command and read the results
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Display ingredient details if found
                            if (reader.Read())
                            {
                                Console.WriteLine("\n========================================");
                                Console.WriteLine("=          Ingredient Details          =");
                                Console.WriteLine("========================================");
                                Console.WriteLine($"ID:            {reader["IngredientID"]}");
                                Console.WriteLine($"Name:          {reader["IngredientName"]}");
                                Console.WriteLine($"Unit:          {reader["Unit"]}");
                                Console.WriteLine($"Current Stock: {reader["CurrentStock"]} {reader["Unit"]}");
                                Console.WriteLine($"Reorder Level: {reader["ReorderLevel"]} {reader["Unit"]}");
                                Console.WriteLine($"Unit Price:    ${reader["UnitPrice"]:F2}");
                                Console.WriteLine($"Supplier:      {reader["SupplierName"]}");
                                Console.WriteLine($"Used In:       {reader["UsedInMenuItems"]} menu items");
                                // Display menu items if any
                                if (reader["MenuItems"] != DBNull.Value)
                                {
                                    Console.WriteLine($"Menu Items:    {reader["MenuItems"]}");
                                }
                                // Check for low stock and display alert
                                decimal currentStock = Convert.ToDecimal(reader["CurrentStock"]);
                                decimal reorderLevel = Convert.ToDecimal(reader["ReorderLevel"]);
                                // Display low stock alert if applicable
                                if (currentStock <= reorderLevel)
                                {
                                    Console.WriteLine($"\nLOW STOCK ALERT: {currentStock}/{reorderLevel} {reader["Unit"]}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("\nIngredient not found!");
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
        /// Retrieves and displays all ingredients with their details.
        /// </summary>
        public static void GetAllIngredients()
        {
            // Establish a connection to the database
            try
            {
                // Open database connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // SQL query to fetch all ingredients   
                    string query = @"SELECT i.*, s.SupplierName,
                                    (SELECT COUNT(*) FROM MenuItemIngredient WHERE IngredientID = i.IngredientID) as UsedInMenuItems
                                    FROM Ingredient i
                                    JOIN Supplier s ON i.SupplierID = s.SupplierID
                                    ORDER BY i.IngredientName";
                    // Prepare and execute the SQL command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n====================================================================================");
                        Console.WriteLine("=                              All Ingredients                                     =");
                        Console.WriteLine("====================================================================================");
                        Console.WriteLine($"{"ID",-5} {"Name",-25} {"Stock",-12} {"Reorder",-12} {"Unit",-10} {"Price",-10} {"Supplier",-20} {"Menu Items",-12}");
                        Console.WriteLine(new string('─', 120));
                        // Display each ingredient's details
                        while (reader.Read())
                        {
                            decimal currentStock = Convert.ToDecimal(reader["CurrentStock"]);
                            decimal reorderLevel = Convert.ToDecimal(reader["ReorderLevel"]);
                            string stockDisplay = currentStock <= reorderLevel ?
                                $"{currentStock:F2}" : $"{currentStock:F2}";

                            Console.WriteLine($"{reader["IngredientID"],-5} {reader["IngredientName"],-25} {stockDisplay,-12} {reader["ReorderLevel"],-12:F2} {reader["Unit"],-10} ${reader["UnitPrice"],-10:F2} {reader["SupplierName"],-20} {reader["UsedInMenuItems"],-12}");
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
        /// Updates an existing ingredient record in the database.
        /// </summary>
        /// <param name="ingredientId"></param>
        /// <param name="ingredientName"></param>
        /// <param name="unit"></param>
        /// <param name="reorderLevel"></param>
        /// <param name="currentStock"></param>
        /// <param name="unitPrice"></param>
        /// <param name="supplierId"></param>
        public static void UpdateIngredient(int ingredientId, string ingredientName, string unit, decimal reorderLevel, decimal currentStock, decimal unitPrice, int supplierId)
        {
            try
            {
                // Open database connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // SQL query to update an existing ingredient
                    string query = @"UPDATE Ingredient 
                                    SET IngredientName = @IngredientName,
                                        Unit = @Unit,
                                        ReorderLevel = @ReorderLevel,
                                        CurrentStock = @CurrentStock,
                                        UnitPrice = @UnitPrice,
                                        SupplierID = @SupplierID
                                    WHERE IngredientID = @IngredientID";
                    // Prepare and execute the SQL command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IngredientName", ingredientName);
                        cmd.Parameters.AddWithValue("@Unit", unit);
                        cmd.Parameters.AddWithValue("@ReorderLevel", reorderLevel);
                        cmd.Parameters.AddWithValue("@CurrentStock", currentStock);
                        cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                        cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                        cmd.Parameters.AddWithValue("@IngredientID", ingredientId);
                        // Execute the command and get the number of affected rows
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nIngredient updated successfully!");
                        else
                            Console.WriteLine("\nIngredient not found!");
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
        /// Updates the stock level of an ingredient.
        /// </summary>
        /// <param name="ingredientId"></param>
        /// <param name="newStock"></param>
        public static void UpdateIngredientStock(int ingredientId, decimal newStock)
        {
            try
            {
                // Open database connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Ingredient 
                                    SET CurrentStock = @CurrentStock 
                                    WHERE IngredientID = @IngredientID";
                    // Prepare and execute the SQL command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CurrentStock", newStock);
                        cmd.Parameters.AddWithValue("@IngredientID", ingredientId);
                        // Execute the command and get the number of affected rows
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nIngredient stock updated to {newStock} successfully!");
                        else
                            Console.WriteLine("\nIngredient not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Adds a specified amount to the stock level of an ingredient.
        /// </summary>
        /// <param name="ingredientId"></param>
        /// <param name="amountToAdd"></param>
        public static void AddToIngredientStock(int ingredientId, decimal amountToAdd)
        {
            try
            {
                // Open database connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // SQL query to add to the current stock
                    string query = @"UPDATE Ingredient 
                                    SET CurrentStock = CurrentStock + @AmountToAdd 
                                    WHERE IngredientID = @IngredientID";
                    // Prepare and execute the SQL command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AmountToAdd", amountToAdd);
                        cmd.Parameters.AddWithValue("@IngredientID", ingredientId);
                        // Execute the command and get the number of affected rows
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Fetch and display the new stock level
                            string stockQuery = "SELECT CurrentStock FROM Ingredient WHERE IngredientID = @IngredientID";
                            // Prepare and execute the SQL command to get new stock
                            using (MySqlCommand stockCmd = new MySqlCommand(stockQuery, conn))
                            {
                                stockCmd.Parameters.AddWithValue("@IngredientID", ingredientId);
                                decimal newStock = Convert.ToDecimal(stockCmd.ExecuteScalar());
                                Console.WriteLine($"\nAdded {amountToAdd} to stock. New total: {newStock}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nIngredient not found!");
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
        /// Deletes an ingredient record from the database.
        /// </summary>
        /// <param name="ingredientId"></param>
        public static void DeleteIngredient(int ingredientId)
        {
            try
            {
                // Open database connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // Check if ingredient is used in any menu items
                    string checkQuery = "SELECT COUNT(*) FROM MenuItemIngredient WHERE IngredientID = @IngredientID";
                    // Prepare and execute the SQL command
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@IngredientID", ingredientId);
                        int usageCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        // Prevent deletion if ingredient is in use
                        if (usageCount > 0)
                        {
                            Console.WriteLine($"\nCannot delete ingredient used in {usageCount} menu items!");
                            return;
                        }
                    }
                    // Prepare and execute the SQL command
                    string query = "DELETE FROM Ingredient WHERE IngredientID = @IngredientID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IngredientID", ingredientId);
                        // Execute the command and get the number of affected rows
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nIngredient deleted successfully!");
                        else
                            Console.WriteLine("\nIngredient not found!");
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
        /// Retrieves and displays ingredients that are low in stock.
        /// </summary>
        public static void GetLowStockIngredients()
        {
            // Establish a connection to the database
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection()) 
                {
                    conn.Open();
                    // SQL query to fetch low stock ingredients
                    string query = @"SELECT i.*, s.SupplierName, s.PhoneNumber, s.Email,
                                    (SELECT COUNT(*) FROM MenuItemIngredient WHERE IngredientID = i.IngredientID) as UsedInItems
                                    FROM Ingredient i
                                    JOIN Supplier s ON i.SupplierID = s.SupplierID
                                    WHERE i.CurrentStock <= i.ReorderLevel
                                    ORDER BY (i.CurrentStock / i.ReorderLevel)";
                    // Prepare and execute the SQL command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Display header for low stock ingredients
                        Console.WriteLine("\n=======================================================================================");
                        Console.WriteLine("=                       Low Stock Ingredients (Need Reorder!)                         =");
                        Console.WriteLine("=======================================================================================");
                        // Display each low stock ingredient's details
                        bool hasLowStock = false;
                        while (reader.Read())
                        {
                            hasLowStock = true;
                            decimal percentage = (Convert.ToDecimal(reader["CurrentStock"]) / Convert.ToDecimal(reader["ReorderLevel"])) * 100;
                            // Display ingredient details
                            Console.WriteLine($"\n{reader["IngredientName"]}");
                            Console.WriteLine($"  Current Stock: {reader["CurrentStock"]} {reader["Unit"]} ({percentage:F1}% of reorder level)");
                            Console.WriteLine($"  Reorder Level: {reader["ReorderLevel"]} {reader["Unit"]}");
                            Console.WriteLine($"  Unit Price:    ${reader["UnitPrice"]:F2}");
                            Console.WriteLine($"  Supplier:      {reader["SupplierName"]}");
                            Console.WriteLine($"  Phone:         {reader["PhoneNumber"]}");
                            Console.WriteLine($"  Email:         {reader["Email"]}");
                            Console.WriteLine($"  Used In:       {reader["UsedInItems"]} menu items");
                        }
                        // Display message if all ingredients are well-stocked
                        if (!hasLowStock)
                            Console.WriteLine("\nAll ingredients are well-stocked!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Generates and displays an ingredient usage report for the past specified number of days.
        /// </summary>
        /// <param name="days"></param>
        public static void GetIngredientUsageReport(int days = 30)
        {
            try
            {
                // Open database connection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // SQL query to fetch ingredient usage data
                    string query = @"SELECT i.IngredientName, i.Unit,
                                    SUM(mii.QuantityRequired * oi.Quantity) as TotalUsed,
                                    i.CurrentStock,
                                    (i.CurrentStock / NULLIF(SUM(mii.QuantityRequired * oi.Quantity), 0)) * @Days as DaysRemaining
                                    FROM Ingredient i
                                    JOIN MenuItemIngredient mii ON i.IngredientID = mii.IngredientID
                                    JOIN OrderItem oi ON mii.MenuItemID = oi.MenuItemID
                                    JOIN `Order` o ON oi.OrderID = o.OrderID
                                    WHERE o.OrderDate >= DATE_SUB(CURDATE(), INTERVAL @Days DAY)
                                    AND o.OrderStatus != 'Cancelled'
                                    GROUP BY i.IngredientID
                                    HAVING TotalUsed > 0
                                    ORDER BY TotalUsed DESC";
                    // Prepare and execute the SQL command
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameter for number of days
                        cmd.Parameters.AddWithValue("@Days", days);
                        // Execute the command and read the results
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n==================================================================================");
                            Console.WriteLine($"=                 Ingredient Usage Report (Last {days} Days)                    =");
                            Console.WriteLine("==================================================================================");
                            Console.WriteLine($"{"Ingredient",-25} {"Unit",-10} {"Used",-15} {"Current Stock",-15} {"Days Remaining",-15}");
                            Console.WriteLine(new string('─', 85));
                            // Display each ingredient's usage data
                            while (reader.Read())
                            {
                                // Calculate days remaining and format display
                                decimal daysRemaining = reader["DaysRemaining"] != DBNull.Value ? Convert.ToDecimal(reader["DaysRemaining"]) : 999;
                                string daysDisplay = daysRemaining < 7 ? $" {daysRemaining:F1}" : $"{daysRemaining:F1}";
                                // Display ingredient usage details
                                Console.WriteLine($"{reader["IngredientName"],-25} {reader["Unit"],-10} {reader["TotalUsed"],-15:F2} {reader["CurrentStock"],-15:F2} {daysDisplay,-15}");
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