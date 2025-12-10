/*
 * FILE          : SupplierOperations.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : Contains CRUD operations and reports for Supplier entity.
 */
using System;
using MySql.Data.MySqlClient;

namespace QuickBiteRestaurant.Data
{
    public class SupplierOperations
    {
        public static void CreateSupplier(string supplierName, string contactPerson, string email, string phoneNumber, string address)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Supplier (SupplierName, ContactPerson, Email, PhoneNumber, Address) VALUES (@SupplierName, @ContactPerson, @Email, @PhoneNumber, @Address)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SupplierName", supplierName);
                        cmd.Parameters.AddWithValue("@ContactPerson", contactPerson);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Address", address);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"\nSupplier created successfully! ID: {cmd.LastInsertedId}");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"\nDatabase Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Gets and displays supplier details by ID, including ingredient count and total inventory value.
        /// </summary>
        /// <param name="supplierId"></param>
        public static void GetSupplierById(int supplierId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT s.*,
                                    COUNT(i.IngredientID) as IngredientCount,
                                    SUM(i.CurrentStock * i.UnitPrice) as TotalInventoryValue
                                    FROM Supplier s
                                    LEFT JOIN Ingredient i ON s.SupplierID = i.SupplierID
                                    WHERE s.SupplierID = @SupplierID
                                    GROUP BY s.SupplierID";
                    // Execute the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                        // Read the results
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("\n========================================");
                                Console.WriteLine("=           Supplier Details           =");
                                Console.WriteLine("========================================");
                                Console.WriteLine($"ID:             {reader["SupplierID"]}");
                                Console.WriteLine($"Name:           {reader["SupplierName"]}");
                                Console.WriteLine($"Contact:        {reader["ContactPerson"]}");
                                Console.WriteLine($"Email:          {reader["Email"]}");
                                Console.WriteLine($"Phone:          {reader["PhoneNumber"]}");
                                Console.WriteLine($"Address:        {reader["Address"]}");
                                Console.WriteLine($"Ingredients:    {reader["IngredientCount"]}");
                                Console.WriteLine($"Inventory Value: ${reader["TotalInventoryValue"]:F2}");
                            }
                            else
                            {
                                Console.WriteLine("\nSupplier not found!");
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
        /// Gets and displays all suppliers with ingredient counts and low stock item counts.
        /// </summary>
        public static void GetAllSuppliers()
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT s.*,
                                    COUNT(i.IngredientID) as IngredientCount,
                                    SUM(CASE WHEN i.CurrentStock <= i.ReorderLevel THEN 1 ELSE 0 END) as LowStockItems
                                    FROM Supplier s
                                    LEFT JOIN Ingredient i ON s.SupplierID = i.SupplierID
                                    GROUP BY s.SupplierID
                                    ORDER BY s.SupplierName";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n===============================================================================================================");
                        Console.WriteLine("=                                                All Suppliers                                                =");
                        Console.WriteLine("===============================================================================================================");
                        Console.WriteLine($"{"ID",-5} {"Name",-25} {"Contact",-20} {"Phone",-15} {"Ingredients",-12} {"Low Stock",-12} {"Email"}");
                        Console.WriteLine(new string('─', 110));

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["SupplierID"],-5} {reader["SupplierName"],-25} {reader["ContactPerson"],-20} {reader["PhoneNumber"],-15} {reader["IngredientCount"],-12} {reader["LowStockItems"],-12} {reader["Email"]}");
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
        /// Updates supplier details.
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="supplierName"></param>
        /// <param name="contactPerson"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="address"></param>
        public static void UpdateSupplier(int supplierId, string supplierName, string contactPerson, string email, string phoneNumber, string address)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Supplier 
                                    SET SupplierName = @SupplierName,
                                        ContactPerson = @ContactPerson,
                                        Email = @Email,
                                        PhoneNumber = @PhoneNumber,
                                        Address = @Address
                                    WHERE SupplierID = @SupplierID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SupplierName", supplierName);
                        cmd.Parameters.AddWithValue("@ContactPerson", contactPerson);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nSupplier updated successfully!");
                        else
                            Console.WriteLine("\nSupplier not found!");
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
        /// Deletes a supplier. If the supplier has ingredients, prompts to reassign them before deletion.
        /// </summary>
        /// <param name="supplierId"></param>
        public static void DeleteSupplier(int supplierId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // Check for associated ingredients
                    string checkQuery = "SELECT COUNT(*) FROM Ingredient WHERE SupplierID = @SupplierID";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@SupplierID", supplierId);
                        int ingredientCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        // If ingredients exist, prompt for reassignment
                        if (ingredientCount > 0)
                        {
                            Console.WriteLine($"\nCannot delete supplier with {ingredientCount} ingredients!");
                            Console.Write("Reassign ingredients to another supplier? (yes/no): ");
                            // Get user confirmation
                            if (Console.ReadLine().ToLower() == "yes")
                            {
                                Console.Write("Enter new Supplier ID: ");
                                // Get new supplier ID
                                if (int.TryParse(Console.ReadLine(), out int newSupplierId))
                                {
                                    string updateQuery = "UPDATE Ingredient SET SupplierID = @NewSupplierID WHERE SupplierID = @OldSupplierID";
                                    // Reassign ingredients
                                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                                    {
                                        // Set parameters and execute
                                        updateCmd.Parameters.AddWithValue("@NewSupplierID", newSupplierId);
                                        updateCmd.Parameters.AddWithValue("@OldSupplierID", supplierId);
                                        int updated = updateCmd.ExecuteNonQuery();
                                        Console.WriteLine($"Reassigned {updated} ingredients.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Supplier ID!");
                                    return;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Deletion cancelled.");
                                return;
                            }
                        }
                    }
                    // Proceed to delete supplier
                    string query = "DELETE FROM Supplier WHERE SupplierID = @SupplierID";
                    // Execute deletion
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        // Check result
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nSupplier deleted successfully!");
                        else
                            Console.WriteLine("\nSupplier not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Generates a performance report for all suppliers.
        /// </summary>
        public static void GetSupplierPerformanceReport()
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT s.SupplierName,
                                    COUNT(i.IngredientID) as TotalIngredients,
                                    SUM(i.CurrentStock * i.UnitPrice) as InventoryValue,
                                    SUM(CASE WHEN i.CurrentStock <= i.ReorderLevel THEN 1 ELSE 0 END) as LowStockCount,
                                    AVG(i.UnitPrice) as AvgPrice
                                    FROM Supplier s
                                    JOIN Ingredient i ON s.SupplierID = i.SupplierID
                                    GROUP BY s.SupplierID
                                    ORDER BY InventoryValue DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n==============================================================================================================");
                        Console.WriteLine("=                                   Supplier Performance Report                                              =");
                        Console.WriteLine("==============================================================================================================");
                        Console.WriteLine($"{"Supplier",-25} {"Ingredients",-15} {"Inventory Value",-20} {"Low Stock",-15} {"Avg Price",-15}");
                        Console.WriteLine(new string('─', 95));
                        // Read and display each supplier's performance data
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["SupplierName"],-25} {reader["TotalIngredients"],-15} ${reader["InventoryValue"],-20:F2} {reader["LowStockCount"],-15} ${reader["AvgPrice"],-15:F2}");
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