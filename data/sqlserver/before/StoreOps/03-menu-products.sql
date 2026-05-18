USE [$(StoreOpsDatabase)];
GO

INSERT INTO dbo.MenuCategory (CategoryCode, CategoryName, SortOrder, IsActive)
SELECT source.CategoryCode, source.CategoryName, source.SortOrder, source.IsActive
FROM
(
    VALUES
        (N'PIZZA-CLASSIC', N'Classic Pizza', CAST(1 AS TINYINT), CAST(1 AS BIT)),
        (N'PIZZA-SPECIALTY', N'Specialty Pizza', CAST(2 AS TINYINT), CAST(1 AS BIT)),
        (N'SIDES', N'Sides', CAST(3 AS TINYINT), CAST(1 AS BIT)),
        (N'WINGS', N'Wings', CAST(4 AS TINYINT), CAST(1 AS BIT)),
        (N'DRINKS', N'Drinks', CAST(5 AS TINYINT), CAST(1 AS BIT)),
        (N'DESSERT', N'Dessert', CAST(6 AS TINYINT), CAST(1 AS BIT))
) source (CategoryCode, CategoryName, SortOrder, IsActive)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.MenuCategory target
    WHERE target.CategoryCode = source.CategoryCode
);
GO

INSERT INTO dbo.MenuProduct (ProductCode, MenuCategoryId, ProductName, ProductSize, LegacyPosCode, BasePrice, IsActive, IsDeliveryEligible, SortOrder)
SELECT source.ProductCode, category.MenuCategoryId, source.ProductName, source.ProductSize, source.LegacyPosCode, source.BasePrice, source.IsActive, source.IsDeliveryEligible, source.SortOrder
FROM
(
    VALUES
        (N'CHEESE-LG', N'PIZZA-CLASSIC', N'Classic Cheese Pizza', N'Large', N'POS-1101', CAST(16.99 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(1 AS TINYINT)),
        (N'PEPPERONI-LG', N'PIZZA-CLASSIC', N'Pepperoni Pizza', N'Large', N'POS-1102', CAST(18.49 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(2 AS TINYINT)),
        (N'SUPREME-LG', N'PIZZA-SPECIALTY', N'Northwest Supreme', N'Large', N'POS-1201', CAST(22.99 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(1 AS TINYINT)),
        (N'BBQCHICKEN-LG', N'PIZZA-SPECIALTY', N'Smoky BBQ Chicken', N'Large', N'POS-1202', CAST(23.49 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(2 AS TINYINT)),
        (N'VEGGIE-MD', N'PIZZA-SPECIALTY', N'Garden Veggie', N'Medium', N'POS-1203', CAST(18.99 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(3 AS TINYINT)),
        (N'BREADSTICKS', N'SIDES', N'Garlic Breadsticks', NULL, N'POS-2101', CAST(6.49 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(1 AS TINYINT)),
        (N'CHEESYBREAD', N'SIDES', N'Cheesy Bread', NULL, N'POS-2102', CAST(7.79 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(2 AS TINYINT)),
        (N'CAESAR-SALAD', N'SIDES', N'Caesar Side Salad', NULL, N'POS-2103', CAST(5.99 AS MONEY), CAST(1 AS BIT), CAST(0 AS BIT), CAST(3 AS TINYINT)),
        (N'BUFFALO-8', N'WINGS', N'Buffalo Wings', N'8 Piece', N'POS-2201', CAST(10.99 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(1 AS TINYINT)),
        (N'GARLICPARM-8', N'WINGS', N'Garlic Parmesan Wings', N'8 Piece', N'POS-2202', CAST(11.49 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(2 AS TINYINT)),
        (N'SODA-2L', N'DRINKS', N'Fountain Soda', N'2 Liter', N'POS-3101', CAST(3.49 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(1 AS TINYINT)),
        (N'SPARKLING-WATER', N'DRINKS', N'Sparkling Water', NULL, N'POS-3102', CAST(2.29 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(2 AS TINYINT)),
        (N'BROWNIE-TRAY', N'DESSERT', N'Brownie Tray', NULL, N'POS-3201', CAST(8.99 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(1 AS TINYINT)),
        (N'COOKIE-6PK', N'DESSERT', N'Chocolate Chip Cookie Pack', N'6 Pack', N'POS-3202', CAST(5.99 AS MONEY), CAST(1 AS BIT), CAST(1 AS BIT), CAST(2 AS TINYINT))
) source (ProductCode, CategoryCode, ProductName, ProductSize, LegacyPosCode, BasePrice, IsActive, IsDeliveryEligible, SortOrder)
INNER JOIN dbo.MenuCategory category
    ON category.CategoryCode = source.CategoryCode
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.MenuProduct target
    WHERE target.ProductCode = source.ProductCode
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'StoreOps\03-menu-products.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'StoreOps\03-menu-products.sql');
END
GO
