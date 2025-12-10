/*
 * FILE          : Program.cs
 * PROJECT       : Final Project - QuickBite Fast Food Restaurant Management System
 * PROGRAMMER    : Mehakpreet Singh, Navjot Singh Bhullar
 * FIRST VERSION : 2025
 * DESCRIPTION   : Main program file for QuickBite Restaurant Management System.
 */
using System;
using System.Collections.Generic;
using QuickBiteRestaurant.Data;

namespace QuickBiteRestaurant
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            Console.WriteLine("=============================================================");
            Console.WriteLine("=       QuickBite Restaurant Management System              =");
            Console.WriteLine("=============================================================");
            // Test database connection
            if (!DatabaseConnection.TestConnection())
            {
                Console.WriteLine("\nError: Cannot connect to database!");
                Console.WriteLine("Please check your connection string in DatabaseConnection.cs");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            // Connection successful
            Console.WriteLine("Database connection successful!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            // Main menu loop
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n=============================================================");
                Console.WriteLine("=   QuickBite Restaurant Management System                  =");
                Console.WriteLine("=============================================================");
                Console.WriteLine("\nMAIN MENU:");
                Console.WriteLine("1.  Customer Operations");
                Console.WriteLine("2.  Restaurant Operations");
                Console.WriteLine("3.  Employee Operations");
                Console.WriteLine("4.  Menu Category Operations");
                Console.WriteLine("5.  Menu Item Operations");
                Console.WriteLine("6.  Order Operations");
                Console.WriteLine("7.  Supplier Operations");
                Console.WriteLine("8.  Ingredient Operations");
                Console.WriteLine("9.  Menu Item - Ingredient Operations");
                Console.WriteLine("10. Reports & Analytics");
                Console.WriteLine("0.  Exit");
                Console.Write("\nSelect an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CustomerMenu();
                        break;
                    case "2":
                        RestaurantMenu();
                        break;
                    case "3":
                        EmployeeMenu();
                        break;
                    case "4":
                        MenuCategoryMenu();
                        break;
                    case "5":
                        MenuItemMenu();
                        break;
                    case "6":
                        OrderMenu();
                        break;
                    case "7":
                        SupplierMenu();
                        break;
                    case "8":
                        IngredientMenu();
                        break;
                    case "9":
                        MenuItemIngredientMenu();
                        break;
                    case "10":
                        ReportsMenu();
                        break;
                    case "0":
                        Console.WriteLine("\nThank you for using QuickBite Management System!");
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("\nInvalid option! Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
        /// <summary>
        /// Customer Operations Menu
        /// </summary>
        static void CustomerMenu()
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("=        Customer Operations             =");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Create Customer");
            Console.WriteLine("2. View Customer by ID");
            Console.WriteLine("3. View All Customers");
            Console.WriteLine("4. Update Customer");
            Console.WriteLine("5. Delete Customer");
            Console.WriteLine("6. Top Customers by Spending");
            Console.Write("\nSelect: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CreateCustomer();
                    break;
                case "2":
                    Console.Write("\nCustomer ID: ");
                    if (int.TryParse(Console.ReadLine(), out int custId))
                        CustomerOperations.GetCustomerById(custId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "3":
                    CustomerOperations.GetAllCustomers();
                    break;
                case "4":
                    UpdateCustomer();
                    break;
                case "5":
                    // Delete Customer
                    Console.Write("\nCustomer ID to delete: ");
                    // Validate input
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                    {
                        Console.Write("Are you sure? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                            CustomerOperations.DeleteCustomer(deleteId);
                        else
                            Console.WriteLine("\nDeletion cancelled.");
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "6":
                    Console.Write("Number of top customers (default 10): ");
                    // Validate input
                    if (int.TryParse(Console.ReadLine(), out int limit) && limit > 0)
                        CustomerOperations.GetTopCustomers(limit);
                    else
                        CustomerOperations.GetTopCustomers();
                    break;
                default:
                    Console.WriteLine("\nInvalid option!");
                    break;
            }
        }
        /// <summary>
        /// Creates a new customer by gathering input from the user.
        /// </summary>
        static void CreateCustomer()
        {
            Console.Write("\nFirst Name: ");
            string firstName = Console.ReadLine();
            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Phone: ");
            string phone = Console.ReadLine();
            Console.Write("Initial Loyalty Points (default 0): ");
            if (int.TryParse(Console.ReadLine(), out int points))
                CustomerOperations.CreateCustomer(firstName, lastName, email, phone, points);
            else
                CustomerOperations.CreateCustomer(firstName, lastName, email, phone, 0);
        }
        /// <summary>
        /// Updates an existing customer's details.
        /// </summary>
        static void UpdateCustomer()
        {
            // Get customer ID
            Console.Write("\nCustomer ID: ");
            if (int.TryParse(Console.ReadLine(), out int custId))
            {
                Console.Write("First Name: ");
                string firstName = Console.ReadLine();
                Console.Write("Last Name: ");
                string lastName = Console.ReadLine();
                Console.Write("Email: ");
                string email = Console.ReadLine();
                Console.Write("Phone: ");
                string phone = Console.ReadLine();
                Console.Write("Loyalty Points: ");
                if (int.TryParse(Console.ReadLine(), out int points))
                    CustomerOperations.UpdateCustomer(custId, firstName, lastName, email, phone, points);
                else
                    Console.WriteLine("\nInvalid points!");
            }
            else
                Console.WriteLine("\nInvalid ID!");
        }
        /// <summary>
        /// Restaurant Operations Menu
        /// </summary>
        static void RestaurantMenu()
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("=       Restaurant Operations            =");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Create Restaurant");
            Console.WriteLine("2. View Restaurant by ID");
            Console.WriteLine("3. View All Restaurants");
            Console.WriteLine("4. Update Restaurant");
            Console.WriteLine("5. Delete Restaurant");
            Console.WriteLine("6. Sales Report");
            Console.Write("\nSelect: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CreateRestaurant();
                    break;
                case "2":
                    Console.Write("\nRestaurant ID: ");
                    if (int.TryParse(Console.ReadLine(), out int restId))
                        RestaurantOperations.GetRestaurantById(restId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "3":
                    RestaurantOperations.GetAllRestaurants();
                    break;
                case "4":
                    UpdateRestaurant();
                    break;
                case "5":
                    Console.Write("\nRestaurant ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                    {
                        Console.Write("Are you sure? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                            RestaurantOperations.DeleteRestaurant(deleteId);
                        else
                            Console.WriteLine("\nDeletion cancelled.");
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "6":
                    Console.Write("\nRestaurant ID: ");
                    if (int.TryParse(Console.ReadLine(), out int reportId))
                        RestaurantOperations.GetRestaurantSalesReport(reportId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                default:
                    Console.WriteLine("\nInvalid option!");
                    break;
            }
        }
        /// <summary>
        /// Creates a new restaurant by gathering input from the user.
        /// </summary>
        static void CreateRestaurant()
        {
            Console.Write("\nRestaurant Name: ");
            string name = Console.ReadLine();
            Console.Write("Address: ");
            string address = Console.ReadLine();
            Console.Write("City: ");
            string city = Console.ReadLine();
            Console.Write("Province: ");
            string province = Console.ReadLine();
            Console.Write("Postal Code: ");
            string postalCode = Console.ReadLine();
            Console.Write("Phone: ");
            string phone = Console.ReadLine();
            Console.Write("Manager ID (optional, press Enter to skip): ");
            string managerInput = Console.ReadLine();

            int? managerId = null;
            if (!string.IsNullOrEmpty(managerInput) && int.TryParse(managerInput, out int mId))
                managerId = mId;
            RestaurantOperations.CreateRestaurant(name, address, city, province, postalCode, phone, managerId);
        }
        /// <summary>
        /// Updates an existing restaurant's details.
        /// </summary>
        static void UpdateRestaurant()
        {
            Console.Write("\nRestaurant ID: ");
            if (int.TryParse(Console.ReadLine(), out int restId))
            {
                Console.Write("Name: ");
                string name = Console.ReadLine();
                Console.Write("Phone: ");
                string phone = Console.ReadLine();
                Console.Write("Address: ");
                string address = Console.ReadLine();
                Console.Write("City: ");
                string city = Console.ReadLine();
                Console.Write("Province: ");
                string province = Console.ReadLine();
                Console.Write("Postal Code: ");
                string postalCode = Console.ReadLine();
                Console.Write("Manager ID (optional, press Enter to skip): ");
                string managerInput = Console.ReadLine();

                int? managerId = null;
                if (!string.IsNullOrEmpty(managerInput) && int.TryParse(managerInput, out int mId))
                    managerId = mId;

                RestaurantOperations.UpdateRestaurant(restId, name, phone, address, city, province, postalCode, managerId);
            }
            else
                Console.WriteLine("\nInvalid ID!");
        }
        /// <summary>
        /// Employee Operations Menu
        /// </summary>
        static void EmployeeMenu()
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("=        Employee Operations             =");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Create Employee");
            Console.WriteLine("2. View Employee by ID");
            Console.WriteLine("3. View All Employees");
            Console.WriteLine("4. Update Employee");
            Console.WriteLine("5. Delete Employee");
            Console.WriteLine("6. Performance Report");
            Console.Write("\nSelect: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CreateEmployee();
                    break;
                case "2":
                    Console.Write("\nEmployee ID: ");
                    if (int.TryParse(Console.ReadLine(), out int empId))
                        EmployeeOperations.GetEmployeeById(empId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "3":
                    EmployeeOperations.GetAllEmployees();
                    break;
                case "4":
                    UpdateEmployee();
                    break;
                case "5":
                    Console.Write("\nEmployee ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                    {
                        Console.Write("Are you sure? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                            EmployeeOperations.DeleteEmployee(deleteId);
                        else
                            Console.WriteLine("\nDeletion cancelled.");
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "6":
                    Console.Write("Restaurant ID (0 for all): ");
                    if (int.TryParse(Console.ReadLine(), out int reportId))
                        EmployeeOperations.GetEmployeePerformanceReport(reportId);
                    else
                        EmployeeOperations.GetEmployeePerformanceReport();
                    break;
                default:
                    Console.WriteLine("\nInvalid option!");
                    break;
            }
        }

        static void CreateEmployee()
        {
            Console.Write("\nFirst Name: ");
            string firstName = Console.ReadLine();
            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Phone: ");
            string phone = Console.ReadLine();
            Console.Write("Position: ");
            string position = Console.ReadLine();
            Console.Write("Hire Date (YYYY-MM-DD): ");
            // Validate date input
            if (DateTime.TryParse(Console.ReadLine(), out DateTime hireDate))
            {
                Console.Write("Salary: ");
                // Validate salary input
                if (decimal.TryParse(Console.ReadLine(), out decimal salary))
                {
                    Console.Write("Restaurant ID: ");
                    // Validate restaurant ID input
                    if (int.TryParse(Console.ReadLine(), out int restId))
                        EmployeeOperations.CreateEmployee(firstName, lastName, email, phone, position, hireDate, salary, restId);
                    else
                        Console.WriteLine("\nInvalid restaurant ID!");
                }
                else
                    Console.WriteLine("\nInvalid salary!");
            }
            else
                Console.WriteLine("\nInvalid date format!");
        }
        /// <summary>
        /// Updates an existing employee's details.
        /// </summary>
        static void UpdateEmployee()
        {
            Console.Write("\nEmployee ID: ");
            if (int.TryParse(Console.ReadLine(), out int empId))
            {
                Console.Write("First Name: ");
                string firstName = Console.ReadLine();
                Console.Write("Last Name: ");
                string lastName = Console.ReadLine();
                Console.Write("Email: ");
                string email = Console.ReadLine();
                Console.Write("Phone: ");
                string phone = Console.ReadLine();
                Console.Write("Position: ");
                string position = Console.ReadLine();
                Console.Write("Salary: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal salary))
                {
                    Console.Write("Restaurant ID: ");
                    if (int.TryParse(Console.ReadLine(), out int restId))
                        EmployeeOperations.UpdateEmployee(empId, firstName, lastName, email, phone, position, salary, restId);
                    else
                        Console.WriteLine("\nInvalid restaurant ID!");
                }
                else
                    Console.WriteLine("\nInvalid salary!");
            }
            else
                Console.WriteLine("\nInvalid ID!");
        }

        static void MenuCategoryMenu()
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("=     Menu Category Operations           =");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Create Category");
            Console.WriteLine("2. View Category by ID");
            Console.WriteLine("3. View All Categories");
            Console.WriteLine("4. Update Category");
            Console.WriteLine("5. Delete Category");
            Console.WriteLine("6. Sales Report");
            Console.Write("\nSelect: ");
            // Get user choice
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("\nCategory Name: ");
                    string name = Console.ReadLine();
                    Console.Write("Description: ");
                    string desc = Console.ReadLine();
                    MenuCategoryOperations.CreateCategory(name, desc);
                    break;
                case "2":
                    Console.Write("\nCategory ID: ");
                    if (int.TryParse(Console.ReadLine(), out int catId))
                        MenuCategoryOperations.GetCategoryById(catId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "3":
                    MenuCategoryOperations.GetAllCategories();
                    break;
                case "4":
                    Console.Write("\nCategory ID: ");
                    if (int.TryParse(Console.ReadLine(), out int updateId))
                    {
                        Console.Write("New Name: ");
                        string newName = Console.ReadLine();
                        Console.Write("New Description: ");
                        string newDesc = Console.ReadLine();
                        MenuCategoryOperations.UpdateCategory(updateId, newName, newDesc);
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "5":
                    Console.Write("\nCategory ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                    {
                        Console.Write("Are you sure? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                            MenuCategoryOperations.DeleteCategory(deleteId);
                        else
                            Console.WriteLine("\nDeletion cancelled.");
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "6":
                    Console.Write("Category ID (0 for all): ");
                    if (int.TryParse(Console.ReadLine(), out int reportId))
                        MenuCategoryOperations.GetCategorySalesReport(reportId);
                    else
                        MenuCategoryOperations.GetCategorySalesReport();
                    break;
                default:
                    Console.WriteLine("\nInvalid option!");
                    break;
            }
        }

        static void MenuItemMenu()
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("=       Menu Item Operations             =");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Create Menu Item");
            Console.WriteLine("2. View Menu Item by ID");
            Console.WriteLine("3. View All Menu Items");
            Console.WriteLine("4. Update Menu Item");
            Console.WriteLine("5. Toggle Availability");
            Console.WriteLine("6. Delete Menu Item");
            Console.WriteLine("7. Sales Report");
            Console.Write("\nSelect: ");
            // Get user choice
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CreateMenuItem();
                    break;
                case "2":
                    Console.Write("\nMenu Item ID: ");
                    if (int.TryParse(Console.ReadLine(), out int itemId))
                        MenuItemOperations.GetMenuItemById(itemId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "3":
                    MenuItemOperations.GetAllMenuItems();
                    break;
                case "4":
                    UpdateMenuItem();
                    break;
                case "5":
                    // Toggle Availability
                    Console.Write("\nMenu Item ID: ");
                    if (int.TryParse(Console.ReadLine(), out int toggleId))
                        MenuItemOperations.ToggleMenuItemAvailability(toggleId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "6":
                    // Delete Menu Item
                    Console.Write("\nMenu Item ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                    {
                        Console.Write("Are you sure? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                            MenuItemOperations.DeleteMenuItem(deleteId);
                        else
                            Console.WriteLine("\nDeletion cancelled.");
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "7":
                    Console.Write("Days to analyze (default 30): ");
                    if (int.TryParse(Console.ReadLine(), out int days) && days > 0)
                        MenuItemOperations.GetMenuItemSalesReport(days);
                    else
                        MenuItemOperations.GetMenuItemSalesReport();
                    break;
                default:
                    Console.WriteLine("\nInvalid option!");
                    break;
            }
        }
        /// <summary>
        /// Creates a new menu item by gathering input from the user.
        /// </summary>
        static void CreateMenuItem()
        {
            Console.Write("\nItem Name: ");
            string name = Console.ReadLine();
            Console.Write("Description: ");
            string desc = Console.ReadLine();
            Console.Write("Price: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.Write("Category ID (1-Burgers, 2-Sides, 3-Drinks, 4-Desserts): ");
                if (int.TryParse(Console.ReadLine(), out int catId))
                {
                    Console.Write("Available (true/false, default true): ");
                    string availInput = Console.ReadLine();
                    bool avail = string.IsNullOrEmpty(availInput) || bool.Parse(availInput);

                    Console.Write("Calorie Count (optional, press Enter to skip): ");
                    string calInput = Console.ReadLine();
                    int? calories = null;
                    if (!string.IsNullOrEmpty(calInput) && int.TryParse(calInput, out int cal))
                        calories = cal;

                    MenuItemOperations.CreateMenuItem(name, desc, price, catId, avail, calories ?? 0);
                }
                else
                    Console.WriteLine("\nInvalid category ID!");
            }
            else
                Console.WriteLine("\nInvalid price!");
        }
        /// <summary>
        /// Updates an existing menu item's details.
        /// </summary>
        static void UpdateMenuItem()
        {
            Console.Write("\nMenu Item ID: ");
            if (int.TryParse(Console.ReadLine(), out int itemId))
            {
                Console.Write("Item Name: ");
                string name = Console.ReadLine();
                Console.Write("Description: ");
                string desc = Console.ReadLine();
                Console.Write("Price: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    Console.Write("Category ID: ");
                    if (int.TryParse(Console.ReadLine(), out int catId))
                    {
                        Console.Write("Available (true/false): ");
                        if (bool.TryParse(Console.ReadLine(), out bool avail))
                        {
                            Console.Write("Calorie Count: ");
                            if (int.TryParse(Console.ReadLine(), out int calories))
                                MenuItemOperations.UpdateMenuItem(itemId, name, desc, price, catId, avail, calories);
                            else
                                Console.WriteLine("\nInvalid calorie count!");
                        }
                        else
                            Console.WriteLine("\nInvalid availability!");
                    }
                    else
                        Console.WriteLine("\nInvalid category ID!");
                }
                else
                    Console.WriteLine("\nInvalid price!");
            }
            else
                Console.WriteLine("\nInvalid ID!");
        }
        /// <summary>
        /// Order Operations Menu
        /// </summary>
        static void OrderMenu()
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("=          Order Operations              =");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Create New Order");
            Console.WriteLine("2. View Order Details");
            Console.WriteLine("3. View All Orders");
            Console.WriteLine("4. Update Order Status");
            Console.WriteLine("5. Sales Report");
            Console.Write("\nSelect: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CreateOrder();
                    break;
                case "2":
                    Console.Write("\nOrder ID: ");
                    if (int.TryParse(Console.ReadLine(), out int orderId))
                        OrderOperations.GetOrderDetails(orderId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "3":
                    Console.Write("Start Date (YYYY-MM-DD, optional): ");
                    string startInput = Console.ReadLine();
                    Console.Write("End Date (YYYY-MM-DD, optional): ");
                    string endInput = Console.ReadLine();

                    DateTime? startDate = null;
                    DateTime? endDate = null;
                    // Parse dates if provided
                    if (!string.IsNullOrEmpty(startInput) && DateTime.TryParse(startInput, out DateTime sDate))
                        startDate = sDate;
                    if (!string.IsNullOrEmpty(endInput) && DateTime.TryParse(endInput, out DateTime eDate))
                        endDate = eDate;
                    // Fetch orders
                    OrderOperations.GetAllOrders(startDate, endDate);
                    break;
                case "4":
                    Console.Write("\nOrder ID: ");
                    if (int.TryParse(Console.ReadLine(), out int updateId))
                    {
                        Console.Write("New Status (Pending/Preparing/Ready/Completed/Cancelled): ");
                        string status = Console.ReadLine();
                        OrderOperations.UpdateOrderStatus(updateId, status);
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "5":
                    Console.Write("Start Date (YYYY-MM-DD): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime reportStart))
                    {
                        Console.Write("End Date (YYYY-MM-DD): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime reportEnd))
                            OrderOperations.GetSalesReport(reportStart, reportEnd);
                        else
                            Console.WriteLine("\nInvalid end date!");
                    }
                    else
                        Console.WriteLine("\nInvalid start date!");
                    break;
                default:
                    Console.WriteLine("\nInvalid option!");
                    break;
            }
        }
        /// <summary>
        /// Creates a new order by gathering input from the user.
        /// </summary>
        static void CreateOrder()
        {
            Console.Write("\nCustomer ID: ");
            if (int.TryParse(Console.ReadLine(), out int custId))
            {
                Console.Write("Employee ID: ");
                if (int.TryParse(Console.ReadLine(), out int empId))
                {
                    Console.Write("Restaurant ID: ");
                    if (int.TryParse(Console.ReadLine(), out int restId))
                    {
                        Console.Write("Order Type (Dine-in/Takeout): ");
                        string type = Console.ReadLine();
                        Console.Write("Number of different items: ");
                        if (int.TryParse(Console.ReadLine(), out int itemCount) && itemCount > 0)
                        {
                            var orderItems = new Dictionary<int, OrderItemInfo>();

                            for (int i = 0; i < itemCount; i++)
                            {
                                Console.Write($"Menu Item ID #{i + 1}: ");
                                if (int.TryParse(Console.ReadLine(), out int menuId))
                                {
                                    Console.Write($"Quantity #{i + 1}: ");
                                    if (int.TryParse(Console.ReadLine(), out int qty))
                                    {
                                        Console.Write($"Special Instructions (optional): ");
                                        string instructions = Console.ReadLine();

                                        orderItems.Add(menuId, new OrderItemInfo(qty, instructions));
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nInvalid quantity!");
                                        return;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("\nInvalid menu item ID!");
                                    return;
                                }
                            }

                            OrderOperations.CreateOrder(custId, empId, restId, type, orderItems);
                        }
                        else
                            Console.WriteLine("\nInvalid item count!");
                    }
                    else
                        Console.WriteLine("\nInvalid restaurant ID!");
                }
                else
                    Console.WriteLine("\nInvalid employee ID!");
            }
            else
                Console.WriteLine("\nInvalid customer ID!");
        }
        /// <summary>
        /// Supplier Operations Menu
        /// </summary>
        static void SupplierMenu()
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("=         Supplier Operations            =");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Create Supplier");
            Console.WriteLine("2. View Supplier by ID");
            Console.WriteLine("3. View All Suppliers");
            Console.WriteLine("4. Update Supplier");
            Console.WriteLine("5. Delete Supplier");
            Console.WriteLine("6. Performance Report");
            Console.Write("\nSelect: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CreateSupplier();
                    break;
                case "2":
                    Console.Write("\nSupplier ID: ");
                    if (int.TryParse(Console.ReadLine(), out int suppId))
                        SupplierOperations.GetSupplierById(suppId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "3":
                    SupplierOperations.GetAllSuppliers();
                    break;
                case "4":
                    UpdateSupplier();
                    break;
                case "5":
                    Console.Write("\nSupplier ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                    {
                        Console.Write("Are you sure? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                            SupplierOperations.DeleteSupplier(deleteId);
                        else
                            Console.WriteLine("\nDeletion cancelled.");
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "6":
                    SupplierOperations.GetSupplierPerformanceReport();
                    break;
                default:
                    Console.WriteLine("\nInvalid option!");
                    break;
            }
        }
        /// <summary>
        /// Creates a new supplier by gathering input from the user.
        /// </summary>
        static void CreateSupplier()
        {
            // Gather supplier details
            Console.Write("\nSupplier Name: ");
            string name = Console.ReadLine();
            Console.Write("Contact Person: ");
            string contact = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Phone: ");
            string phone = Console.ReadLine();
            Console.Write("Address: ");
            string address = Console.ReadLine();

            SupplierOperations.CreateSupplier(name, contact, email, phone, address);
        }
        /// <summary>
        /// Updates an existing supplier's details.
        /// </summary>
        static void UpdateSupplier()
        {
            // Get supplier ID
            Console.Write("\nSupplier ID: ");
            // Validate ID input
            if (int.TryParse(Console.ReadLine(), out int suppId))
            {
                Console.Write("Supplier Name: ");
                string name = Console.ReadLine();
                Console.Write("Contact Person: ");
                string contact = Console.ReadLine();
                Console.Write("Email: ");
                string email = Console.ReadLine();
                Console.Write("Phone: ");
                string phone = Console.ReadLine();
                Console.Write("Address: ");
                string address = Console.ReadLine();

                SupplierOperations.UpdateSupplier(suppId, name, contact, email, phone, address);
            }
            else
                Console.WriteLine("\n✗ Invalid ID!");
        }
        /// <summary>
        /// Ingredient Operations Menu
        /// </summary>
        static void IngredientMenu()
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("=       Ingredient Operations            =");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Create Ingredient");
            Console.WriteLine("2. View Ingredient by ID");
            Console.WriteLine("3. View All Ingredients");
            Console.WriteLine("4. Update Ingredient");
            Console.WriteLine("5. Update Stock Level");
            Console.WriteLine("6. Add to Stock");
            Console.WriteLine("7. Delete Ingredient");
            Console.WriteLine("8. Low Stock Alert");
            Console.WriteLine("9. Usage Report");
            Console.Write("\nSelect: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CreateIngredient();
                    break;
                case "2":
                    // View Ingredient by ID
                    Console.Write("\nIngredient ID: ");
                    // Validate ID input
                    if (int.TryParse(Console.ReadLine(), out int ingId))
                        IngredientOperations.GetIngredientById(ingId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "3":
                    IngredientOperations.GetAllIngredients();
                    break;
                case "4":
                    UpdateIngredient();
                    break;
                case "5":
                    // Update Stock Level
                    Console.Write("\nIngredient ID: ");
                    // Validate ID input
                    if (int.TryParse(Console.ReadLine(), out int updateId))
                    {
                        Console.Write("New Stock Level: ");
                        // Validate stock level input
                        if (decimal.TryParse(Console.ReadLine(), out decimal newStock))
                            IngredientOperations.UpdateIngredientStock(updateId, newStock);
                        else
                            Console.WriteLine("\nInvalid stock level!");
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "6":
                    // Add to Stock
                    Console.Write("\nIngredient ID: ");
                    // Validate ID input
                    if (int.TryParse(Console.ReadLine(), out int addId))
                    {
                        // Validate amount input
                        Console.Write("Amount to Add: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal addAmount))
                            IngredientOperations.AddToIngredientStock(addId, addAmount);
                        else
                            Console.WriteLine("\nInvalid amount!");
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "7":
                    // Delete Ingredient
                    Console.Write("\nIngredient ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                    {
                        Console.Write("Are you sure? (yes/no): ");
                        // Validate confirmation input
                        if (Console.ReadLine().ToLower() == "yes")
                            IngredientOperations.DeleteIngredient(deleteId);
                        else
                            Console.WriteLine("\nDeletion cancelled.");
                    }
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "8":
                    IngredientOperations.GetLowStockIngredients();
                    break;
                case "9":
                    Console.Write("Days to analyze (default 30): ");
                    if (int.TryParse(Console.ReadLine(), out int days) && days > 0)
                        IngredientOperations.GetIngredientUsageReport(days);
                    else
                        IngredientOperations.GetIngredientUsageReport();
                    break;
                default:
                    Console.WriteLine("\nInvalid option!");
                    break;
            }
        }
        /// <summary>
        /// Creates a new ingredient by gathering input from the user.
        /// </summary>
        static void CreateIngredient()
        {
            Console.Write("\nIngredient Name: ");
            string name = Console.ReadLine();
            Console.Write("Unit (kg/L/pieces/etc): ");
            string unit = Console.ReadLine();
            Console.Write("Reorder Level: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal reorder))
            {
                Console.Write("Current Stock: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal stock))
                {
                    Console.Write("Unit Price: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal price))
                    {
                        Console.Write("Supplier ID: ");
                        if (int.TryParse(Console.ReadLine(), out int suppId))
                            IngredientOperations.CreateIngredient(name, unit, reorder, stock, price, suppId);
                        else
                            Console.WriteLine("\nInvalid supplier ID!");
                    }
                    else
                        Console.WriteLine("\nInvalid price!");
                }
                else
                    Console.WriteLine("\nInvalid stock!");
            }
            else
                Console.WriteLine("\nInvalid reorder level!");
        }
        /// <summary>
        /// Updates an existing ingredient's details.
        /// </summary>
        static void UpdateIngredient()
        {
            Console.Write("\nIngredient ID: ");
            if (int.TryParse(Console.ReadLine(), out int ingId))
            {
                Console.Write("Ingredient Name: ");
                string name = Console.ReadLine();
                Console.Write("Unit: ");
                string unit = Console.ReadLine();
                Console.Write("Reorder Level: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal reorder))
                {
                    Console.Write("Current Stock: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal stock))
                    {
                        Console.Write("Unit Price: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal price))
                        {
                            Console.Write("Supplier ID: ");
                            if (int.TryParse(Console.ReadLine(), out int suppId))
                                IngredientOperations.UpdateIngredient(ingId, name, unit, reorder, stock, price, suppId);
                            else
                                Console.WriteLine("\nInvalid supplier ID!");
                        }
                        else
                            Console.WriteLine("\nInvalid price!");
                    }
                    else
                        Console.WriteLine("\nInvalid stock!");
                }
                else
                    Console.WriteLine("\nInvalid reorder level!");
            }
            else
                Console.WriteLine("\nInvalid ID!");
        }
        /// <summary>
        /// Menu Item - Ingredient Operations Menu
        /// </summary>
        static void MenuItemIngredientMenu()
        {
            Console.WriteLine("\n=========================================");
            Console.WriteLine("=   Menu Item - Ingredient Operations   =");
            Console.WriteLine("=========================================");
            Console.WriteLine("1. Add Ingredient to Menu Item");
            Console.WriteLine("2. View Menu Item Ingredients");
            Console.WriteLine("3. Update Ingredient Quantity");
            Console.WriteLine("4. Remove Ingredient from Menu Item");
            Console.WriteLine("5. View Ingredient Usage");
            Console.WriteLine("6. Cost Analysis");
            Console.Write("\nSelect: ");
            // Get user choice
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    // Add Ingredient to Menu Item
                    Console.Write("\nMenu Item ID: ");
                    // Validate menu item ID input
                    if (int.TryParse(Console.ReadLine(), out int menuId))
                    {
                        Console.Write("Ingredient ID: ");
                        // Validate ingredient ID input
                        if (int.TryParse(Console.ReadLine(), out int ingId))
                        {
                            Console.Write("Quantity Required: ");
                            // Validate quantity input
                            if (decimal.TryParse(Console.ReadLine(), out decimal qty))
                                MenuItemIngredientOperations.AddIngredientToMenuItem(menuId, ingId, qty);
                            else
                                Console.WriteLine("\nInvalid quantity!");
                        }
                        else
                            Console.WriteLine("\nInvalid ingredient ID!");
                    }
                    else
                        Console.WriteLine("\nInvalid menu item ID!");
                    break;
                case "2":
                    // View Menu Item Ingredients
                    Console.Write("\nMenu Item ID: ");
                    if (int.TryParse(Console.ReadLine(), out int viewId))
                        MenuItemIngredientOperations.GetMenuItemIngredients(viewId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "3":
                    // Update Ingredient Quantity
                    Console.Write("\nMenu Item ID: ");
                    // Validate menu item ID input
                    if (int.TryParse(Console.ReadLine(), out int updateMenuId))
                    {
                        Console.Write("Ingredient ID: ");
                        // Validate ingredient ID input
                        if (int.TryParse(Console.ReadLine(), out int updateIngId))
                        {
                            Console.Write("New Quantity: ");
                            // Validate quantity input
                            if (decimal.TryParse(Console.ReadLine(), out decimal newQty))
                                MenuItemIngredientOperations.UpdateIngredientQuantity(updateMenuId, updateIngId, newQty);
                            else
                                Console.WriteLine("\nInvalid quantity!");
                        }
                        else
                            Console.WriteLine("\nInvalid ingredient ID!");
                    }
                    else
                        Console.WriteLine("\nInvalid menu item ID!");
                    break;
                case "4":
                    // Remove Ingredient from Menu Item
                    Console.Write("\nMenu Item ID: ");
                    // Validate menu item ID input
                    if (int.TryParse(Console.ReadLine(), out int removeMenuId))
                    {
                        Console.Write("Ingredient ID: ");
                        // Validate ingredient ID input
                        if (int.TryParse(Console.ReadLine(), out int removeIngId))
                            MenuItemIngredientOperations.RemoveIngredientFromMenuItem(removeMenuId, removeIngId);
                        else
                            Console.WriteLine("\nInvalid ingredient ID!");
                    }
                    else
                        Console.WriteLine("\nInvalid menu item ID!");
                    break;
                case "5":
                    // View Ingredient Usage
                    Console.Write("\nIngredient ID: ");
                    // Validate ingredient ID input
                    if (int.TryParse(Console.ReadLine(), out int usageId))
                        MenuItemIngredientOperations.GetIngredientUsageAcrossMenuItems(usageId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "6":
                    // Cost Analysis
                    Console.Write("\nMenu Item ID: ");
                    // Validate menu item ID input
                    if (int.TryParse(Console.ReadLine(), out int costId))
                        MenuItemIngredientOperations.GetMenuItemCostAnalysis(costId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                default:
                    Console.WriteLine("\nInvalid option!");
                    break;
            }
        }
        /// <summary>
        /// Reports & Analytics Menu
        /// </summary>
        static void ReportsMenu()
        {
            Console.WriteLine("\n=========================================");
            Console.WriteLine("=         Reports & Analytics           =");
            Console.WriteLine("=========================================");
            Console.WriteLine("1. Top Customers Report");
            Console.WriteLine("2. Restaurant Sales Report");
            Console.WriteLine("3. Employee Performance Report");
            Console.WriteLine("4. Menu Category Sales Report");
            Console.WriteLine("5. Menu Item Sales Report");
            Console.WriteLine("6. Order Sales Report");
            Console.WriteLine("7. Supplier Performance Report");
            Console.WriteLine("8. Ingredient Usage Report");
            Console.WriteLine("9. Low Stock Alert");
            Console.Write("\nSelect: ");
            // Get user choice
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    // Top Customers Report
                    Console.Write("Number of top customers: ");
                    // Validate input
                    if (int.TryParse(Console.ReadLine(), out int topCount) && topCount > 0)
                        CustomerOperations.GetTopCustomers(topCount);
                    else
                        CustomerOperations.GetTopCustomers();
                    break;
                case "2":
                    // Restaurant Sales Report
                    Console.Write("Restaurant ID: ");
                    // Validate input
                    if (int.TryParse(Console.ReadLine(), out int restId))
                        RestaurantOperations.GetRestaurantSalesReport(restId);
                    else
                        Console.WriteLine("\nInvalid ID!");
                    break;
                case "3":
                    // Employee Performance Report
                    Console.Write("Restaurant ID (0 for all): ");
                    // Validate input
                    if (int.TryParse(Console.ReadLine(), out int empRestId))
                        EmployeeOperations.GetEmployeePerformanceReport(empRestId);
                    else
                        EmployeeOperations.GetEmployeePerformanceReport();
                    break;
                case "4":
                    // Menu Category Sales Report
                    Console.Write("Category ID (0 for all): ");
                    // Validate input
                    if (int.TryParse(Console.ReadLine(), out int catId))
                        MenuCategoryOperations.GetCategorySalesReport(catId);
                    else
                        MenuCategoryOperations.GetCategorySalesReport();
                    break;
                case "5":
                    // Menu Item Sales Report
                    Console.Write("Days to analyze: ");
                    // Validate input
                    if (int.TryParse(Console.ReadLine(), out int itemDays) && itemDays > 0)
                        MenuItemOperations.GetMenuItemSalesReport(itemDays);
                    else
                        MenuItemOperations.GetMenuItemSalesReport();
                    break;
                case "6":
                    // Order Sales Report
                    Console.Write("Start Date (YYYY-MM-DD): ");
                    // Validate input
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                    {
                        // Validate input
                        Console.Write("End Date (YYYY-MM-DD): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                            OrderOperations.GetSalesReport(startDate, endDate);
                        else
                            Console.WriteLine("\nInvalid end date!");
                    }
                    else
                        Console.WriteLine("\nInvalid start date!");
                    break;
                case "7":
                    SupplierOperations.GetSupplierPerformanceReport();
                    break;
                case "8":
                    // Ingredient Usage Report
                    Console.Write("Days to analyze: ");
                    // Validate input
                    if (int.TryParse(Console.ReadLine(), out int ingDays) && ingDays > 0)
                        IngredientOperations.GetIngredientUsageReport(ingDays);
                    else
                        IngredientOperations.GetIngredientUsageReport();
                    break;
                case "9":
                    IngredientOperations.GetLowStockIngredients();
                    break;
                default:
                    Console.WriteLine("\nInvalid option!");
                    break;
            }
        }
    }
}