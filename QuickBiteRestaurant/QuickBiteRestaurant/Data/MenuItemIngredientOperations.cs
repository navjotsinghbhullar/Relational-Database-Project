/*
 * FILE          : MenuItemIngredientOperations.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : This file contains the MenuItemIngredientOperations class which provides methods to manage
 */
using System;
using MySql.Data.MySqlClient;

namespace QuickBiteRestaurant.Data
{
    public class MenuItemIngredientOperations
    {
        /// <summary>
        /// Adds an ingredient to a menu item with the specified quantity required.
        /// </summary>
        /// <param name="menuItemId"></param>
        /// <param name="ingredientId"></param>
        /// <param name="quantityRequired"></param>
        public static void AddIngredientToMenuItem(int menuItemId, int ingredientId, decimal quantityRequired)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO MenuItemIngredient (MenuItemID, IngredientID, QuantityRequired) 
                                    VALUES (@MenuItemID, @IngredientID, @QuantityRequired)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                        cmd.Parameters.AddWithValue("@IngredientID", ingredientId);
                        cmd.Parameters.AddWithValue("@QuantityRequired", quantityRequired);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"\nIngredient added to menu item successfully!");
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
        /// Gets and displays all ingredients for a specific menu item along with their costs and profit margin.
        /// </summary>
        /// <param name="menuItemId"></param>
        public static void GetMenuItemIngredients(int menuItemId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT mii.*, i.IngredientName, i.Unit, i.UnitPrice,
                                    (i.UnitPrice * mii.QuantityRequired) as IngredientCost
                                    FROM MenuItemIngredient mii
                                    JOIN Ingredient i ON mii.IngredientID = i.IngredientID
                                    WHERE mii.MenuItemID = @MenuItemID
                                    ORDER BY i.IngredientName";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n==================================================================================");
                            Console.WriteLine("=                               Menu Item Ingredients                            =");
                            Console.WriteLine("==================================================================================");
                            Console.WriteLine($"{"Ingredient",-25} {"Quantity",-15} {"Unit",-10} {"Unit Price",-12} {"Cost",-12}");
                            Console.WriteLine(new string('─', 80));
                            // Initialize total cost
                            decimal totalCost = 0;
                            bool hasIngredients = false;
                            // Read and display each ingredient
                            while (reader.Read())
                            {
                                // Mark that we have at least one ingredient
                                hasIngredients = true;
                                decimal cost = Convert.ToDecimal(reader["IngredientCost"]);
                                totalCost += cost;
                                // Display ingredient details
                                Console.WriteLine($"{reader["IngredientName"],-25} {reader["QuantityRequired"],-15:F2} {reader["Unit"],-10} ${reader["UnitPrice"],-11:F2} ${cost,-12:F2}");
                            }
                            // If no ingredients were found
                            if (!hasIngredients)
                            {
                                Console.WriteLine("No ingredients assigned to this menu item.");
                            }
                            // Otherwise, display total cost and profit margin
                            else
                            {
                                // Display total cost
                                Console.WriteLine(new string('─', 80));
                                Console.WriteLine($"{"TOTAL COST:",65} ${totalCost:F2}");
                                // Get item price to calculate profit margin
                                string priceQuery = "SELECT Price FROM MenuItem WHERE MenuItemID = @MenuItemID";
                                using (MySqlCommand priceCmd = new MySqlCommand(priceQuery, conn))
                                {
                                    // Reuse the same menuItemId parameter
                                    priceCmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                                    decimal price = Convert.ToDecimal(priceCmd.ExecuteScalar());
                                    decimal profitMargin = price - totalCost;
                                    decimal marginPercentage = (profitMargin / price) * 100;
                                    // Display profit margin
                                    Console.WriteLine($"{"ITEM PRICE:",65} ${price:F2}");
                                    Console.WriteLine($"{"PROFIT MARGIN:",65} ${profitMargin:F2} ({marginPercentage:F1}%)");
                                }
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
        /// Updates the quantity required of an ingredient for a specific menu item.
        /// </summary>
        /// <param name="menuItemId"></param>
        /// <param name="ingredientId"></param>
        /// <param name="newQuantity"></param>
        public static void UpdateIngredientQuantity(int menuItemId, int ingredientId, decimal newQuantity)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE MenuItemIngredient 
                                    SET QuantityRequired = @QuantityRequired
                                    WHERE MenuItemID = @MenuItemID AND IngredientID = @IngredientID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuantityRequired", newQuantity);
                        cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                        cmd.Parameters.AddWithValue("@IngredientID", ingredientId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nIngredient quantity updated to {newQuantity} successfully!");
                        else
                            Console.WriteLine("\nMenu item ingredient relationship not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Removes an ingredient from a specific menu item.
        /// </summary>
        /// <param name="menuItemId"></param>
        /// <param name="ingredientId"></param>
        public static void RemoveIngredientFromMenuItem(int menuItemId, int ingredientId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"DELETE FROM MenuItemIngredient 
                                    WHERE MenuItemID = @MenuItemID AND IngredientID = @IngredientID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                        cmd.Parameters.AddWithValue("@IngredientID", ingredientId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nIngredient removed from menu item successfully!");
                        else
                            Console.WriteLine("\nMenu item ingredient relationship not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Gets and displays all menu items that use a specific ingredient along with their value contribution.
        /// </summary>
        /// <param name="ingredientId"></param>
        public static void GetIngredientUsageAcrossMenuItems(int ingredientId)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT m.ItemName, c.CategoryName, m.Price, mii.QuantityRequired,
                                    (mii.QuantityRequired * m.Price) as ValueContribution
                                    FROM MenuItemIngredient mii
                                    JOIN MenuItem m ON mii.MenuItemID = m.MenuItemID
                                    JOIN MenuCategory c ON m.CategoryID = c.CategoryID
                                    WHERE mii.IngredientID = @IngredientID
                                    ORDER BY ValueContribution DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IngredientID", ingredientId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n====================================================================================");
                            Console.WriteLine("=                        Ingredient Usage in Menu Items                            =");
                            Console.WriteLine("====================================================================================");
                            Console.WriteLine($"{"Menu Item",-30} {"Category",-15} {"Price",-10} {"Qty Required",-15} {"Value",-15}");
                            Console.WriteLine(new string('─', 90));
                            // Initialize total value
                            bool found = false;
                            decimal totalValue = 0;
                            // Read and display each menu item
                            while (reader.Read())
                            {
                                // Mark that we have at least one menu item
                                found = true;
                                decimal value = Convert.ToDecimal(reader["ValueContribution"]);
                                totalValue += value;
                                // Display menu item details
                                Console.WriteLine($"{reader["ItemName"],-30} {reader["CategoryName"],-15} ${reader["Price"],-9:F2} {reader["QuantityRequired"],-15:F2} ${value,-15:F2}");
                            }
                            // If no menu items were found
                            if (!found)
                            {
                                Console.WriteLine("This ingredient is not used in any menu items.");
                            }
                            // Otherwise, display total value
                            else
                            {
                                Console.WriteLine(new string('─', 90));
                                Console.WriteLine($"{"TOTAL VALUE:",75} ${totalValue:F2}");
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
        /// Gets a detailed cost analysis for a specific menu item, including ingredient costs, total cost, profit margin, and recommendations.
        /// </summary>
        /// <param name="menuItemId"></param>
        public static void GetMenuItemCostAnalysis(int menuItemId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // First, get the menu item details
                    string itemQuery = @"SELECT m.*, c.CategoryName 
                               FROM MenuItem m
                               JOIN MenuCategory c ON m.CategoryID = c.CategoryID
                               WHERE m.MenuItemID = @MenuItemID";
                    // Variables to hold menu item details
                    string itemName = "";
                    string categoryName = "";
                    decimal price = 0;
                    int calories = 0;
                    // Execute the first query
                    using (MySqlCommand itemCmd = new MySqlCommand(itemQuery, conn))
                    {
                        itemCmd.Parameters.AddWithValue("@MenuItemID", menuItemId);

                        using (MySqlDataReader reader = itemCmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                Console.WriteLine("\nMenu item not found!");
                                return;
                            }

                            // Store values in variables
                            itemName = reader["ItemName"].ToString();
                            categoryName = reader["CategoryName"].ToString();
                            price = Convert.ToDecimal(reader["Price"]);
                            calories = Convert.ToInt32(reader["CalorieCount"]);
                        } 
                    }
                    // Display menu item header
                    Console.WriteLine("\n===========================================================================================");
                    Console.WriteLine("=                                    Menu Item Cost Analysis                              =");
                    Console.WriteLine("===========================================================================================");
                    Console.WriteLine($"Item:       {itemName}");
                    Console.WriteLine($"Category:   {categoryName}");
                    Console.WriteLine($"Price:      ${price:F2}");
                    Console.WriteLine($"Calories:   {calories}");
                    Console.WriteLine(new string('─', 80));

                    // Now execute the second query for ingredient costs
                    string costQuery = @"SELECT i.IngredientName, mii.QuantityRequired, i.Unit, i.UnitPrice,
                                (mii.QuantityRequired * i.UnitPrice) as IngredientCost
                                FROM MenuItemIngredient mii
                                JOIN Ingredient i ON mii.IngredientID = i.IngredientID
                                WHERE mii.MenuItemID = @MenuItemID
                                ORDER BY IngredientCost DESC";
                    // Display ingredient cost breakdown
                    using (MySqlCommand costCmd = new MySqlCommand(costQuery, conn))
                    {
                        // Reuse the same menuItemId parameter
                        costCmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                        // Execute and read results
                        using (MySqlDataReader reader = costCmd.ExecuteReader())
                        {
                            // Display ingredient cost header
                            Console.WriteLine($"{"Ingredient",-25} {"Qty",-10} {"Unit",-10} {"Unit Price",-12} {"Cost",-12}");
                            Console.WriteLine(new string('─', 80));
                            // Initialize total cost
                            decimal totalCost = 0;
                            // Track if there are any ingredients
                            bool hasIngredients = false;
                            // Read and display each ingredient
                            while (reader.Read())
                            {
                                // Mark that we have at least one ingredient
                                hasIngredients = true;
                                decimal cost = Convert.ToDecimal(reader["IngredientCost"]);
                                totalCost += cost;
                                // Display ingredient details
                                Console.WriteLine($"{reader["IngredientName"],-25} {reader["QuantityRequired"],-10:F2} {reader["Unit"],-10} ${reader["UnitPrice"],-11:F2} ${cost,-12:F2}");
                            }
                            // If no ingredients were found
                            if (!hasIngredients)
                            {
                                Console.WriteLine("No ingredients assigned.");
                                return;
                            }
                            // Display total cost
                            Console.WriteLine(new string('─', 80));
                            Console.WriteLine($"{"TOTAL COST:",57} ${totalCost:F2}");

                            // Calculate profit margins
                            decimal profit = price - totalCost;
                            decimal margin = (profit / price) * 100;
                            decimal costPercentage = (totalCost / price) * 100;
                            // Display profit analysis
                            Console.WriteLine($"{"PRICE:",57} ${price:F2}");
                            Console.WriteLine($"{"PROFIT:",57} ${profit:F2}");
                            Console.WriteLine($"{"MARGIN:",57} {margin:F1}%");
                            Console.WriteLine($"{"COST %:",57} {costPercentage:F1}%");
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