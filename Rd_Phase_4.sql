CREATE DATABASE QuickBiteFastFood;
USE QuickBiteFastFood;

-- ============================================
-- Table: Customer
-- Stores customer information for loyalty and orders
-- ============================================
CREATE TABLE Customer (
    CustomerID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PhoneNumber VARCHAR(15) NOT NULL,
    LoyaltyPoints INT DEFAULT 0,
    DateJoined DATE NOT NULL DEFAULT (CURRENT_DATE),
    CONSTRAINT chk_loyalty_points CHECK (LoyaltyPoints >= 0),
    INDEX idx_customer_email (Email),
    INDEX idx_customer_phone (PhoneNumber)
);

-- ============================================
-- Table: Restaurant
-- Stores information about restaurant locations
-- ============================================
CREATE TABLE Restaurant (
    RestaurantID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Address VARCHAR(200) NOT NULL,
    City VARCHAR(50) NOT NULL,
    Province VARCHAR(50) NOT NULL,
    PostalCode VARCHAR(10) NOT NULL,
    PhoneNumber VARCHAR(15) NOT NULL,
    ManagerID INT,
    INDEX idx_restaurant_city (City)
);

-- ============================================
-- Table: Employee
-- Stores employee information
-- ============================================
CREATE TABLE Employee (
    EmployeeID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PhoneNumber VARCHAR(15) NOT NULL,
    Position VARCHAR(50) NOT NULL,
    HireDate DATE NOT NULL,
    Salary DECIMAL(10, 2) NOT NULL,
    RestaurantID INT NOT NULL,
    CONSTRAINT fk_employee_restaurant FOREIGN KEY (RestaurantID) 
        REFERENCES Restaurant(RestaurantID)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT chk_salary CHECK (Salary > 0),
    INDEX idx_employee_restaurant (RestaurantID),
    INDEX idx_employee_position (Position)
);

-- Add foreign key for Restaurant.ManagerID after Employee table exists
ALTER TABLE Restaurant
ADD CONSTRAINT fk_restaurant_manager FOREIGN KEY (ManagerID)
    REFERENCES Employee(EmployeeID)
    ON DELETE SET NULL
    ON UPDATE CASCADE;

-- ============================================
-- Table: MenuCategory
-- Stores menu item categories
-- ============================================
CREATE TABLE MenuCategory (
    CategoryID INT AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL UNIQUE,
    Description TEXT,
    INDEX idx_category_name (CategoryName)
);

-- ============================================
-- Table: MenuItem
-- Stores menu items offered by the restaurant
-- ============================================
CREATE TABLE MenuItem (
    MenuItemID INT AUTO_INCREMENT PRIMARY KEY,
    ItemName VARCHAR(100) NOT NULL,
    Description TEXT,
    Price DECIMAL(8, 2) NOT NULL,
    CategoryID INT NOT NULL,
    IsAvailable BOOLEAN DEFAULT TRUE,
    CalorieCount INT,
    CONSTRAINT fk_menuitem_category FOREIGN KEY (CategoryID)
        REFERENCES MenuCategory(CategoryID)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT chk_price CHECK (Price > 0),
    CONSTRAINT chk_calories CHECK (CalorieCount >= 0),
    INDEX idx_menuitem_category (CategoryID),
    INDEX idx_menuitem_available (IsAvailable)
);

-- ============================================
-- Table: Order
-- Stores customer orders
-- ============================================
CREATE TABLE `Order` (
    OrderID INT AUTO_INCREMENT PRIMARY KEY,
    OrderDate DATE NOT NULL,
    OrderTime TIME NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL DEFAULT 0,
    OrderStatus ENUM('Pending', 'Preparing', 'Ready', 'Completed', 'Cancelled') 
        NOT NULL DEFAULT 'Pending',
    OrderType ENUM('Dine-in', 'Takeout') NOT NULL,
    CustomerID INT,
    EmployeeID INT NOT NULL,
    RestaurantID INT NOT NULL,
    CONSTRAINT fk_order_customer FOREIGN KEY (CustomerID)
        REFERENCES Customer(CustomerID)
        ON DELETE SET NULL
        ON UPDATE CASCADE,
    CONSTRAINT fk_order_employee FOREIGN KEY (EmployeeID)
        REFERENCES Employee(EmployeeID)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT fk_order_restaurant FOREIGN KEY (RestaurantID)
        REFERENCES Restaurant(RestaurantID)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT chk_total_amount CHECK (TotalAmount >= 0),
    INDEX idx_order_date (OrderDate),
    INDEX idx_order_status (OrderStatus),
    INDEX idx_order_customer (CustomerID),
    INDEX idx_order_restaurant (RestaurantID)
);

-- ============================================
-- Table: OrderItem
-- Stores individual items within an order (junction table)
-- ============================================
CREATE TABLE OrderItem (
    OrderItemID INT AUTO_INCREMENT PRIMARY KEY,
    OrderID INT NOT NULL,
    MenuItemID INT NOT NULL,
    Quantity INT NOT NULL,
    Subtotal DECIMAL(8, 2) NOT NULL,
    SpecialInstructions TEXT,
    CONSTRAINT fk_orderitem_order FOREIGN KEY (OrderID)
        REFERENCES `Order`(OrderID)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT fk_orderitem_menuitem FOREIGN KEY (MenuItemID)
        REFERENCES MenuItem(MenuItemID)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT chk_quantity CHECK (Quantity > 0),
    CONSTRAINT chk_subtotal CHECK (Subtotal >= 0),
    INDEX idx_orderitem_order (OrderID),
    INDEX idx_orderitem_menuitem (MenuItemID)
);

-- ============================================
-- Table: Supplier
-- Stores supplier information for ingredients
-- ============================================
CREATE TABLE Supplier (
    SupplierID INT AUTO_INCREMENT PRIMARY KEY,
    SupplierName VARCHAR(100) NOT NULL,
    ContactPerson VARCHAR(100),
    Email VARCHAR(100) UNIQUE,
    PhoneNumber VARCHAR(15) NOT NULL,
    Address VARCHAR(200),
    INDEX idx_supplier_name (SupplierName)
);

-- ============================================
-- Table: Ingredient
-- Stores ingredients used in menu items
-- ============================================
CREATE TABLE Ingredient (
    IngredientID INT AUTO_INCREMENT PRIMARY KEY,
    IngredientName VARCHAR(100) NOT NULL,
    Unit VARCHAR(20) NOT NULL,
    ReorderLevel DECIMAL(10, 2) NOT NULL,
    CurrentStock DECIMAL(10, 2) NOT NULL DEFAULT 0,
    UnitPrice DECIMAL(8, 2) NOT NULL,
    SupplierID INT NOT NULL,
    CONSTRAINT fk_ingredient_supplier FOREIGN KEY (SupplierID)
        REFERENCES Supplier(SupplierID)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT chk_reorder_level CHECK (ReorderLevel >= 0),
    CONSTRAINT chk_current_stock CHECK (CurrentStock >= 0),
    CONSTRAINT chk_unit_price CHECK (UnitPrice >= 0),
    INDEX idx_ingredient_supplier (SupplierID),
    INDEX idx_ingredient_stock (CurrentStock)
);

-- ============================================
-- Table: MenuItemIngredient
-- Junction table for Many-to-Many relationship between MenuItem and Ingredient
-- ============================================
CREATE TABLE MenuItemIngredient (
    MenuItemIngredientID INT AUTO_INCREMENT PRIMARY KEY,
    MenuItemID INT NOT NULL,
    IngredientID INT NOT NULL,
    QuantityRequired DECIMAL(10, 2) NOT NULL,
    CONSTRAINT fk_mii_menuitem FOREIGN KEY (MenuItemID)
        REFERENCES MenuItem(MenuItemID)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT fk_mii_ingredient FOREIGN KEY (IngredientID)
        REFERENCES Ingredient(IngredientID)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT chk_quantity_required CHECK (QuantityRequired > 0),
    CONSTRAINT uq_menuitem_ingredient UNIQUE (MenuItemID, IngredientID),
    INDEX idx_mii_menuitem (MenuItemID),
    INDEX idx_mii_ingredient (IngredientID)
);

-- ============================================
-- Insert Sample Data for Testing
-- ============================================

-- Insert Menu Categories
INSERT INTO MenuCategory (CategoryName, Description) VALUES
('Burgers', 'Delicious beef and chicken burgers'),
('Sides', 'Fries, onion rings, and more'),
('Drinks', 'Soft drinks and beverages'),
('Desserts', 'Sweet treats and ice cream');

-- Insert Restaurants
INSERT INTO Restaurant (Name, Address, City, Province, PostalCode, PhoneNumber) VALUES
('QuickBite Downtown', '123 Main St', 'Kitchener', 'Ontario', 'M5H 2N2', '416-555-0100'),
('QuickBite North York', '456 King St', 'Waterloo', 'Ontario', 'M2N 5Z1', '416-555-0200');

-- Insert Employees
INSERT INTO Employee (FirstName, LastName, Email, PhoneNumber, Position, HireDate, Salary, RestaurantID) VALUES
('Brar', 'Singh', 'brar.singh@quickbite.com', '416-555-0001', 'Manager', '2024-01-15', 55000.00, 1),
('Bhullar', 'Singh', 'bhullar.singh@quickbite.com', '416-555-0002', 'Cashier', '2024-03-20', 35000.00, 1),
('Jot', 'Kaur', 'jot.kaur@quickbite.com', '416-555-0003', 'Cook', '2024-02-10', 40000.00, 1);

-- Update Restaurant with Manager
UPDATE Restaurant SET ManagerID = 1 WHERE RestaurantID = 1;

-- Insert Customers
INSERT INTO Customer (FirstName, LastName, Email, PhoneNumber, LoyaltyPoints, DateJoined) VALUES
('Mehakpreet', 'Singh', 'mehak.singh@email.com', '647-987-0401', 150, '2025-01-10'),
('Navjot', 'Singh', 'navjot.singh@email.com', '647-758-0402', 200, '2025-02-15'),
('Manjot', 'Kaur', 'manjot.kaur@email.com', '647-674-0403', 50, '2025-06-20');

-- Insert Menu Items
INSERT INTO MenuItem (ItemName, Description, Price, CategoryID, IsAvailable, CalorieCount) VALUES
('Classic Burger', 'Beef patty with lettuce, tomato, and cheese', 8.99, 1, TRUE, 650),
('Chicken Burger', 'Grilled chicken with mayo and lettuce', 7.99, 1, TRUE, 520),
('French Fries', 'Crispy golden fries', 3.49, 2, TRUE, 380),
('Onion Rings', 'Beer-battered onion rings', 4.49, 2, TRUE, 410),
('Coca Cola', 'Regular Coke 500ml', 2.49, 3, TRUE, 200),
('Chocolate Shake', 'Creamy chocolate milkshake', 5.49, 4, TRUE, 520);

-- Insert Suppliers
INSERT INTO Supplier (SupplierName, ContactPerson, Email, PhoneNumber, Address) VALUES
('Fresh Meat Co', 'Ayushpreet', 'ayushpreet@freshmeat.com', '905-555-0501', '789 Industrial Rd'),
('Veggie Suppliers Ltd', 'Manvinder', 'manvinder@veggiesupply.com', '905-555-0502', '321 Farm Ave');

-- Insert Ingredients
INSERT INTO Ingredient (IngredientName, Unit, ReorderLevel, CurrentStock, UnitPrice, SupplierID) VALUES
('Beef Patty', 'pieces', 50, 200, 1.50, 1),
('Chicken Breast', 'pieces', 40, 150, 1.20, 1),
('Lettuce', 'kg', 10, 25, 2.50, 2),
('Tomato', 'kg', 10, 30, 3.00, 2),
('Cheese Slice', 'pieces', 100, 300, 0.25, 1),
('Bun', 'pieces', 100, 250, 0.40, 2);

-- Insert MenuItemIngredient relationships
INSERT INTO MenuItemIngredient (MenuItemID, IngredientID, QuantityRequired) VALUES
(1, 1, 1), -- Classic Burger: 1 Beef Patty
(1, 3, 0.05), -- Classic Burger: 50g Lettuce
(1, 4, 0.05), -- Classic Burger: 50g Tomato
(1, 5, 2), -- Classic Burger: 2 Cheese Slices
(1, 6, 1), -- Classic Burger: 1 Bun
(2, 2, 1), -- Chicken Burger: 1 Chicken Breast
(2, 3, 0.05), -- Chicken Burger: 50g Lettuce
(2, 6, 1); -- Chicken Burger: 1 Bun

-- Insert Sample Orders
INSERT INTO `Order` (OrderDate, OrderTime, TotalAmount, OrderStatus, OrderType, CustomerID, EmployeeID, RestaurantID) VALUES
('2024-11-29', '12:30:00', 21.47, 'Completed', 'Dine-in', 1, 2, 1),
('2024-11-29', '13:15:00', 15.97, 'Preparing', 'Takeout', 2, 2, 1);

-- Insert Order Items
INSERT INTO OrderItem (OrderID, MenuItemID, Quantity, Subtotal, SpecialInstructions) VALUES
(1, 1, 2, 17.98, 'No pickles'),
(1, 5, 1, 2.49, NULL),
(2, 2, 1, 7.99, 'Extra mayo'),
(2, 3, 1, 3.49, NULL),
(2, 5, 1, 2.49, NULL);