/*
 * FILE          : OrderOperations.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : This file contains the OrderOperations class which provides methods to create,
 */
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace QuickBiteRestaurant.Data
{
    public class OrderOperations
    {
        /// <summary>
        /// Creates a new order with the specified details and order items.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="employeeId"></param>
        /// <param name="restaurantId"></param>
        /// <param name="orderType"></param>
        /// <param name="orderItems"></param>
        public static void CreateOrder(int customerId, int employeeId, int restaurantId, string orderType, Dictionary<int, OrderItemInfo> orderItems)
        {
            // Initialize connection and transaction
            MySqlConnection conn = null;
            MySqlTransaction transaction = null;
            // Begin try block for transaction
            try
            {
                // Establish a connection to the database
                conn = DatabaseConnection.GetConnection();
                conn.Open();
                // Start a transaction
                transaction = conn.BeginTransaction();
                // Inform user
                Console.WriteLine("\nStarting transaction...");
                // Validate order type
                string[] validTypes = { "Dine-in", "Takeout" };
                if (!Array.Exists(validTypes, t => t.Equals(orderType, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception("Invalid order type! Use: Dine-in or Takeout");
                }
                // Validate customer exists
                if (!ValidateEntityExists(conn, transaction, "Customer", customerId))
                {
                    throw new Exception($"Customer ID {customerId} not found!");
                }
                // Validate employee exists
                if (!ValidateEntityExists(conn, transaction, "Employee", employeeId))
                {
                    throw new Exception($"Employee ID {employeeId} not found!");
                }
                // Validate restaurant exists
                if (!ValidateEntityExists(conn, transaction, "Restaurant", restaurantId))
                {
                    throw new Exception($"Restaurant ID {restaurantId} not found!");
                }
                // Check ingredient availability
                foreach (var item in orderItems)
                {
                    if (!CheckMenuItemAvailability(conn, transaction, item.Key, item.Value.Quantity))
                    {
                        throw new Exception($"Insufficient ingredients for menu item {item.Key}");
                    }
                }
                // Insert Order
                string orderQuery = @"INSERT INTO `Order` (OrderDate, OrderTime, TotalAmount, 
                                    OrderStatus, OrderType, CustomerID, EmployeeID, RestaurantID) 
                                    VALUES (CURDATE(), CURTIME(), 0, 'Pending', @OrderType, 
                                    @CustomerID, @EmployeeID, @RestaurantID);
                                    SELECT LAST_INSERT_ID();";
                // Execute order insert and get OrderID
                int orderId;
                using (MySqlCommand cmd = new MySqlCommand(orderQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@OrderType", orderType);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@RestaurantID", restaurantId);
                    // Execute and retrieve new OrderID
                    orderId = Convert.ToInt32(cmd.ExecuteScalar());
                    Console.WriteLine($"Order created with ID: {orderId}");
                }
                // Insert Order Items and calculate total
                decimal totalAmount = 0;
                foreach (var item in orderItems)
                {
                    // Get menu item price and info
                    string priceQuery = @"SELECT Price, ItemName FROM MenuItem WHERE MenuItemID = @MenuItemID AND IsAvailable = TRUE";
                    decimal price;
                    string itemName;
                    // Execute price query
                    using (MySqlCommand cmd = new MySqlCommand(priceQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemID", item.Key);
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Ensure item exists and is available
                            if (!reader.Read())
                            {
                                throw new Exception($"Menu item ID {item.Key} not found or unavailable!");
                            }
                            price = Convert.ToDecimal(reader["Price"]);
                            itemName = reader["ItemName"].ToString();
                        }
                    }
                    // Calculate subtotal
                    decimal subtotal = price * item.Value.Quantity;
                    totalAmount += subtotal;

                    // Insert order item
                    string itemQuery = @"INSERT INTO OrderItem (OrderID, MenuItemID, Quantity, Subtotal, SpecialInstructions) 
                                       VALUES (@OrderID, @MenuItemID, @Quantity, @Subtotal, @SpecialInstructions)";
                    using (MySqlCommand cmd = new MySqlCommand(itemQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.Parameters.AddWithValue("@MenuItemID", item.Key);
                        cmd.Parameters.AddWithValue("@Quantity", item.Value.Quantity);
                        cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                        cmd.Parameters.AddWithValue("@SpecialInstructions",
                            string.IsNullOrEmpty(item.Value.SpecialInstructions) ?
                            DBNull.Value : (object)item.Value.SpecialInstructions);
                        cmd.ExecuteNonQuery();
                    }
                    // Update ingredient stock
                    UpdateIngredientStock(conn, transaction, item.Key, item.Value.Quantity);
                    // Log added item
                    Console.WriteLine($"Added: {itemName} x {item.Value.Quantity} = ${subtotal:F2}");
                }
                // Update order total
                string updateQuery = "UPDATE `Order` SET TotalAmount = @TotalAmount WHERE OrderID = @OrderID";
                using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.ExecuteNonQuery();
                }
                // Calculate and update loyalty points
                int pointsEarned = (int)(totalAmount / 10) * 10;
                if (pointsEarned > 0)
                {
                    string pointsQuery = @"UPDATE Customer 
                                         SET LoyaltyPoints = LoyaltyPoints + @Points 
                                         WHERE CustomerID = @CustomerID";
                    using (MySqlCommand cmd = new MySqlCommand(pointsQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Points", pointsEarned);
                        cmd.Parameters.AddWithValue("@CustomerID", customerId);
                        cmd.ExecuteNonQuery();
                    }
                }
                // Commit transaction
                transaction.Commit();
                Console.WriteLine($"\nOrder created successfully!");
                Console.WriteLine($"  Order ID: {orderId}");
                Console.WriteLine($"  Total Amount: ${totalAmount:F2}");
                Console.WriteLine($"  Items: {orderItems.Count}");
                Console.WriteLine($"  Loyalty Points Earned: {pointsEarned}");
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                Console.WriteLine($"\nDatabase Error - Transaction rolled back: {ex.Message}");
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Console.WriteLine($"\nError - Transaction rolled back: {ex.Message}");
            }
            finally
            {
                conn?.Close();
            }
        }
        /// <summary>
        /// Validates if an entity exists in the specified table by ID.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static bool ValidateEntityExists(MySqlConnection conn, MySqlTransaction transaction, string table, int id)
        {
            // Check if entity exists
            string query = $"SELECT COUNT(*) FROM {table} WHERE {table}ID = @ID";
            // Execute query
            using (MySqlCommand cmd = new MySqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
        /// <summary>
        /// Checks if the menu item is available and if sufficient ingredients are in stock.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="menuItemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private static bool CheckMenuItemAvailability(MySqlConnection conn, MySqlTransaction transaction, int menuItemId, int quantity)
        {
            // Check if menu item is available
            string checkQuery = @"SELECT IsAvailable FROM MenuItem WHERE MenuItemID = @MenuItemID";
            // Execute query
            using (MySqlCommand cmd = new MySqlCommand(checkQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                object result = cmd.ExecuteScalar();
                if (result == null || !Convert.ToBoolean(result))
                    return false;
            }
            // Check ingredient stock
            string ingredientQuery = @"SELECT i.IngredientID, i.CurrentStock, mii.QuantityRequired
                                     FROM Ingredient i
                                     JOIN MenuItemIngredient mii ON i.IngredientID = mii.IngredientID
                                     WHERE mii.MenuItemID = @MenuItemID";
            // Execute query
            using (MySqlCommand cmd = new MySqlCommand(ingredientQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                using (var reader = cmd.ExecuteReader())
                {
                    // Check each ingredient stock
                    while (reader.Read())
                    {
                        decimal required = Convert.ToDecimal(reader["QuantityRequired"]) * quantity;
                        decimal available = Convert.ToDecimal(reader["CurrentStock"]);
                        if (available < required)
                            return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Updates the ingredient stock based on the menu item and quantity ordered.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="menuItemId"></param>
        /// <param name="quantity"></param>
        private static void UpdateIngredientStock(MySqlConnection conn, MySqlTransaction transaction, int menuItemId, int quantity)
        {
            string query = @"UPDATE Ingredient i
                           JOIN MenuItemIngredient mii ON i.IngredientID = mii.IngredientID
                           SET i.CurrentStock = i.CurrentStock - (mii.QuantityRequired * @Quantity)
                           WHERE mii.MenuItemID = @MenuItemID";
            // Execute update
            using (MySqlCommand cmd = new MySqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@MenuItemID", menuItemId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Gets and displays detailed information about a specific order.
        /// </summary>
        /// <param name="orderId"></param>
        public static void GetOrderDetails(int orderId)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // Get order info
                    string orderQuery = @"SELECT o.*, 
                                        CONCAT(c.FirstName, ' ', c.LastName) as CustomerName,
                                        CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                                        r.Name as RestaurantName
                                        FROM `Order` o 
                                        LEFT JOIN Customer c ON o.CustomerID = c.CustomerID 
                                        JOIN Employee e ON o.EmployeeID = e.EmployeeID 
                                        JOIN Restaurant r ON o.RestaurantID = r.RestaurantID 
                                        WHERE o.OrderID = @OrderID";
                    // Execute order query
                    using (MySqlCommand cmd = new MySqlCommand(orderQuery, conn))
                    {
                        // Add parameter
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        // Read order details
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("\n=========================================================");
                                Console.WriteLine("=                    Order Details                      =");
                                Console.WriteLine("=========================================================");
                                Console.WriteLine($"Order ID:    {reader["OrderID"]}");
                                Console.WriteLine($"Date & Time: {reader["OrderDate"]:yyyy-MM-dd} {reader["OrderTime"]}");
                                Console.WriteLine($"Customer:    {reader["CustomerName"]}");
                                Console.WriteLine($"Employee:    {reader["EmployeeName"]}");
                                Console.WriteLine($"Restaurant:  {reader["RestaurantName"]}");
                                Console.WriteLine($"Status:      {reader["OrderStatus"]}");
                                Console.WriteLine($"Type:        {reader["OrderType"]}");
                                Console.WriteLine($"Total:       ${reader["TotalAmount"]:F2}");
                            }
                            else
                            {
                                Console.WriteLine("\nOrder not found!");
                                return;
                            }
                        }
                    }

                    // Get order items
                    string itemsQuery = @"SELECT oi.*, m.ItemName, m.Price 
                                        FROM OrderItem oi 
                                        JOIN MenuItem m ON oi.MenuItemID = m.MenuItemID 
                                        WHERE oi.OrderID = @OrderID";
                    // Execute items query
                    using (MySqlCommand cmd = new MySqlCommand(itemsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        // Read order items
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n=========================================================");
                            Console.WriteLine("Order Items:");
                            Console.WriteLine($"{"Item Name",-35} {"Price",-10} {"Qty",-5} {"Subtotal"}");
                            Console.WriteLine(new string('─', 60));
                            // Display each item
                            decimal grandTotal = 0;
                            // Read through items
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["ItemName"],-35} ${reader["Price"],-9:F2} {reader["Quantity"],-5} ${reader["Subtotal"],-15:F2}");
                                grandTotal += Convert.ToDecimal(reader["Subtotal"]);
                                if (reader["SpecialInstructions"] != DBNull.Value)
                                    Console.WriteLine($"  ↳ Note: {reader["SpecialInstructions"]}");
                            }
                            Console.WriteLine(new string('─', 60));
                            Console.WriteLine($"{"GRAND TOTAL:",50} ${grandTotal:F2}");
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
        /// Gets and displays all orders with optional date filtering.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public static void GetAllOrders(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT o.OrderID, o.OrderDate, o.OrderTime, 
                                    CONCAT(c.FirstName, ' ', c.LastName) as CustomerName,
                                    CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                                    r.Name as RestaurantName,
                                    o.TotalAmount, o.OrderStatus, o.OrderType 
                                    FROM `Order` o 
                                    LEFT JOIN Customer c ON o.CustomerID = c.CustomerID 
                                    JOIN Employee e ON o.EmployeeID = e.EmployeeID
                                    JOIN Restaurant r ON o.RestaurantID = r.RestaurantID
                                    WHERE (@StartDate IS NULL OR o.OrderDate >= @StartDate)
                                    AND (@EndDate IS NULL OR o.OrderDate <= @EndDate)
                                    ORDER BY o.OrderDate DESC, o.OrderTime DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.HasValue ? (object)startDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@EndDate", endDate.HasValue ? (object)endDate.Value : DBNull.Value);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n=========================================================================================================");
                            Console.WriteLine("=                                   All Orders                                                          =");
                            Console.WriteLine("=========================================================================================================");
                            Console.WriteLine($"{"ID",-5} {"Date",-12} {"Time",-10} {"Customer",-20} {"Employee",-20} {"Restaurant",-20} {"Total",-10} {"Status",-12} {"Type"}");
                            Console.WriteLine(new string('─', 135));

                            decimal totalRevenue = 0;
                            int totalOrders = 0;

                            while (reader.Read())
                            {
                                string status = reader["OrderStatus"].ToString();
                                string statusColor = status == "Completed" ? "✓" :
                                                    status == "Cancelled" ? "✗" : "→";

                                Console.WriteLine($"{statusColor} {reader["OrderID"],-4} {reader["OrderDate"]:yyyy-MM-dd}  {reader["OrderTime"],-10} {reader["CustomerName"],-20} {reader["EmployeeName"],-20} {reader["RestaurantName"],-20} ${reader["TotalAmount"],-10:F2} {status,-12} {reader["OrderType"]}");

                                if (status == "Completed")
                                {
                                    totalRevenue += Convert.ToDecimal(reader["TotalAmount"]);
                                }
                                totalOrders++;
                            }

                            Console.WriteLine(new string('─', 135));
                            Console.WriteLine($"{"TOTALS:",-135} {totalOrders} orders, ${totalRevenue:F2} revenue");
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
        /// Updates the status of an existing order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="newStatus"></param>
        public static void UpdateOrderStatus(int orderId, string newStatus)
        {
            try
            {
                // Validate new status
                string[] validStatuses = { "Pending", "Preparing", "Ready", "Completed", "Cancelled" };
                bool isValid = false;
                // Check if status is valid
                foreach (string status in validStatuses)
                {
                    if (status.Equals(newStatus, StringComparison.OrdinalIgnoreCase))
                    {
                        newStatus = status;
                        isValid = true;
                        break;
                    }
                }
                // If not valid, inform user
                if (!isValid)
                {
                    Console.WriteLine("\nInvalid status! Use: Pending, Preparing, Ready, Completed, or Cancelled");
                    return;
                }

                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // If cancelling, restore ingredient stock
                    if (newStatus == "Cancelled")
                    {
                        RestoreIngredientStock(conn, orderId);
                    }
                    // Update order status
                    string query = "UPDATE `Order` SET OrderStatus = @OrderStatus WHERE OrderID = @OrderID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderStatus", newStatus);
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        // Execute update
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"\nOrder status updated to '{newStatus}' successfully!");
                        else
                            Console.WriteLine("\nOrder not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        /// <summary>
        /// Restores ingredient stock for a cancelled order.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="orderId"></param>
        private static void RestoreIngredientStock(MySqlConnection conn, int orderId)
        {
            string query = @"UPDATE Ingredient i
                           JOIN MenuItemIngredient mii ON i.IngredientID = mii.IngredientID
                           JOIN OrderItem oi ON mii.MenuItemID = oi.MenuItemID
                           SET i.CurrentStock = i.CurrentStock + (mii.QuantityRequired * oi.Quantity)
                           WHERE oi.OrderID = @OrderID";
            // Execute update 
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Generates and displays a sales report for the specified date range.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public static void GetSalesReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Establish a connection to the database
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT 
                                    DATE(o.OrderDate) as SaleDate,
                                    r.Name as Restaurant,
                                    COUNT(*) as TotalOrders,
                                    SUM(o.TotalAmount) as DailyRevenue,
                                    AVG(o.TotalAmount) as AverageOrderValue,
                                    SUM(CASE WHEN o.OrderType = 'Dine-in' THEN 1 ELSE 0 END) as DineInOrders,
                                    SUM(CASE WHEN o.OrderType = 'Takeout' THEN 1 ELSE 0 END) as TakeoutOrders
                                    FROM `Order` o
                                    JOIN Restaurant r ON o.RestaurantID = r.RestaurantID
                                    WHERE o.OrderDate BETWEEN @StartDate AND @EndDate
                                    AND o.OrderStatus != 'Cancelled'
                                    GROUP BY DATE(o.OrderDate), r.RestaurantID
                                    ORDER BY SaleDate DESC, DailyRevenue DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n========================================================================================================================================");
                            Console.WriteLine($"=                                Sales Report ({startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd})                                        =");
                            Console.WriteLine("========================================================================================================================================");
                            Console.WriteLine($"{"Date",-12} {"Restaurant",-25} {"Orders",-8} {"Dine-in",-8} {"Takeout",-8} {"Revenue",-15} {"Avg Order",-12}");
                            Console.WriteLine(new string('─', 95));
                            // Display each record
                            decimal totalRevenue = 0;
                            int totalOrders = 0;
                            // Read through records
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["SaleDate"]:yyyy-MM-dd} {reader["Restaurant"],-25} {reader["TotalOrders"],-8} {reader["DineInOrders"],-8} {reader["TakeoutOrders"],-8} ${reader["DailyRevenue"],-15:F2} ${reader["AverageOrderValue"],-15:F2}");
                                totalRevenue += Convert.ToDecimal(reader["DailyRevenue"]);
                                totalOrders += Convert.ToInt32(reader["TotalOrders"]);
                            }

                            Console.WriteLine(new string('─', 95));
                            Console.WriteLine($"{"TOTALS:",-53} {totalOrders,-8} {"",-16} ${totalRevenue:F2}");
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
    /// <summary>
    /// Holds information about an order item.
    /// </summary>
    public class OrderItemInfo
    {
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; }
        /// <summary>
        /// Constructor for OrderItemInfo
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="specialInstructions"></param>
        public OrderItemInfo(int quantity, string specialInstructions = null)
        {
            Quantity = quantity;
            SpecialInstructions = specialInstructions;
        }
    }
}