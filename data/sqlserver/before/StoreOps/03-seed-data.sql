USE [$(StoreOpsDatabase)];
GO

INSERT INTO dbo.Store (StoreNumber, StoreName, RegionCode, PhoneNumber, IsActive, AddressLine1, AddressLine2, City, StateProvinceCode, PostalCode, TimeZoneId)
SELECT source.StoreNumber, source.StoreName, source.RegionCode, source.PhoneNumber, source.IsActive, source.AddressLine1, source.AddressLine2, source.City, source.StateProvinceCode, source.PostalCode, source.TimeZoneId
FROM
(
    VALUES
        (N'014', N'Bellevue Corporate Center', N'NW', N'425-555-0140', CAST(1 AS BIT), N'1450 112th Ave NE', N'Suite 120', N'Bellevue', N'WA', N'98004', N'Pacific Standard Time'),
        (N'022', N'Redmond East Campus', N'NW', N'425-555-0220', CAST(1 AS BIT), N'16825 NE 87th St', N'Building C', N'Redmond', N'WA', N'98052', N'Pacific Standard Time'),
        (N'031', N'Southcenter Mall Annex', N'SM', N'206-555-0310', CAST(1 AS BIT), N'502 Andover Park W', N'Bay 4', N'Tukwila', N'WA', N'98188', N'Pacific Standard Time'),
        (N'044', N'Downtown Kirkland', N'NW', N'425-555-0440', CAST(1 AS BIT), N'221 Central Way', NULL, N'Kirkland', N'WA', N'98033', N'Pacific Standard Time'),
        (N'057', N'SeaTac Service Road', N'SM', N'206-555-0570', CAST(1 AS BIT), N'18415 International Blvd', NULL, N'SeaTac', N'WA', N'98188', N'Pacific Standard Time'),
        (N'068', N'Issaquah Highlands', N'EM', N'425-555-0680', CAST(1 AS BIT), N'1560 Highlands Dr NE', NULL, N'Issaquah', N'WA', N'98029', N'Pacific Standard Time'),
        (N'081', N'College Commons', N'CM', N'206-555-0810', CAST(1 AS BIT), N'4725 15th Ave NE', NULL, N'Seattle', N'WA', N'98105', N'Pacific Standard Time'),
        (N'093', N'Ballard Market Street', N'WM', N'206-555-0930', CAST(1 AS BIT), N'2236 NW Market St', NULL, N'Seattle', N'WA', N'98107', N'Pacific Standard Time')
) source (StoreNumber, StoreName, RegionCode, PhoneNumber, IsActive, AddressLine1, AddressLine2, City, StateProvinceCode, PostalCode, TimeZoneId)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Store target
    WHERE target.StoreNumber = source.StoreNumber
);
GO

INSERT INTO dbo.RouteZone (StoreId, ZoneCode, ZoneName, SortOrder)
SELECT s.StoreId, source.ZoneCode, source.ZoneName, source.SortOrder
FROM
(
    VALUES
        (N'014', N'NW-CORP', N'Northwest Corporate Corridor', CAST(1 AS TINYINT)),
        (N'014', N'BEL-DT', N'Bellevue Downtown Towers', CAST(2 AS TINYINT)),
        (N'022', N'RED-CAMPUS', N'Redmond East Campus Loop', CAST(1 AS TINYINT)),
        (N'022', N'SAMM-TECH', N'Sammamish Tech Ridge', CAST(2 AS TINYINT)),
        (N'031', N'SCTR-MALL', N'Southcenter Mall Ring', CAST(1 AS TINYINT)),
        (N'031', N'TUK-IND', N'Tukwila Industry Park', CAST(2 AS TINYINT)),
        (N'044', N'KIRK-DT', N'Kirkland Downtown Core', CAST(1 AS TINYINT)),
        (N'044', N'HBR-PT', N'Harbor Point Waterfront', CAST(2 AS TINYINT)),
        (N'057', N'AIRPORT-HOTEL', N'Airport Hotel Strip', CAST(1 AS TINYINT)),
        (N'057', N'DESM-SHUTTLE', N'Des Moines Shuttle Row', CAST(2 AS TINYINT)),
        (N'068', N'ISS-HGHTS', N'Issaquah Highlands Loop', CAST(1 AS TINYINT)),
        (N'068', N'SQUAK-VAL', N'Squak Valley Estates', CAST(2 AS TINYINT)),
        (N'081', N'CAMPUS-QUAD', N'Campus Quad', CAST(1 AS TINYINT)),
        (N'081', N'GREEN-LATE', N'Green Lake Late-Night Ring', CAST(2 AS TINYINT)),
        (N'093', N'BALLARD-HILL', N'Ballard North Hill', CAST(1 AS TINYINT)),
        (N'093', N'FREMONT-BRIDGE', N'Fremont Bridge Corridor', CAST(2 AS TINYINT))
) source (StoreNumber, ZoneCode, ZoneName, SortOrder)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.RouteZone target
    WHERE target.StoreId = s.StoreId
      AND target.ZoneCode = source.ZoneCode
);
GO

INSERT INTO dbo.StoreOperatingHours (StoreId, DayOfWeekNumber, OpenTime, CloseTime, DeliveryCutoffTime, LobbyCloseTime)
SELECT s.StoreId, source.DayOfWeekNumber, source.OpenTime, source.CloseTime, source.DeliveryCutoffTime, source.LobbyCloseTime
FROM
(
    VALUES
        (N'014', CAST(1 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'014', CAST(2 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'014', CAST(3 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'014', CAST(4 AS TINYINT), '10:30:00', '23:30:00', '23:00:00', '23:15:00'),
        (N'014', CAST(5 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'014', CAST(6 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'014', CAST(7 AS TINYINT), '11:00:00', '22:00:00', '21:30:00', '21:45:00'),
        (N'022', CAST(1 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'022', CAST(2 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'022', CAST(3 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'022', CAST(4 AS TINYINT), '10:30:00', '23:30:00', '23:00:00', '23:15:00'),
        (N'022', CAST(5 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'022', CAST(6 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'022', CAST(7 AS TINYINT), '11:00:00', '22:00:00', '21:30:00', '21:45:00'),
        (N'031', CAST(1 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'031', CAST(2 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'031', CAST(3 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'031', CAST(4 AS TINYINT), '10:30:00', '23:30:00', '23:00:00', '23:15:00'),
        (N'031', CAST(5 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'031', CAST(6 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'031', CAST(7 AS TINYINT), '11:00:00', '22:00:00', '21:30:00', '21:45:00'),
        (N'044', CAST(1 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'044', CAST(2 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'044', CAST(3 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'044', CAST(4 AS TINYINT), '10:30:00', '23:30:00', '23:00:00', '23:15:00'),
        (N'044', CAST(5 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'044', CAST(6 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'044', CAST(7 AS TINYINT), '11:00:00', '22:00:00', '21:30:00', '21:45:00'),
        (N'057', CAST(1 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'057', CAST(2 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'057', CAST(3 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'057', CAST(4 AS TINYINT), '10:30:00', '23:30:00', '23:00:00', '23:15:00'),
        (N'057', CAST(5 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'057', CAST(6 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'057', CAST(7 AS TINYINT), '11:00:00', '22:00:00', '21:30:00', '21:45:00'),
        (N'068', CAST(1 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'068', CAST(2 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'068', CAST(3 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'068', CAST(4 AS TINYINT), '10:30:00', '23:30:00', '23:00:00', '23:15:00'),
        (N'068', CAST(5 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'068', CAST(6 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'068', CAST(7 AS TINYINT), '11:00:00', '22:00:00', '21:30:00', '21:45:00'),
        (N'081', CAST(1 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'081', CAST(2 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'081', CAST(3 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'081', CAST(4 AS TINYINT), '10:30:00', '23:30:00', '23:00:00', '23:15:00'),
        (N'081', CAST(5 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'081', CAST(6 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'081', CAST(7 AS TINYINT), '11:00:00', '22:00:00', '21:30:00', '21:45:00'),
        (N'093', CAST(1 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'093', CAST(2 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'093', CAST(3 AS TINYINT), '10:30:00', '23:00:00', '22:30:00', '22:45:00'),
        (N'093', CAST(4 AS TINYINT), '10:30:00', '23:30:00', '23:00:00', '23:15:00'),
        (N'093', CAST(5 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'093', CAST(6 AS TINYINT), '10:30:00', '00:30:00', '00:00:00', '00:15:00'),
        (N'093', CAST(7 AS TINYINT), '11:00:00', '22:00:00', '21:30:00', '21:45:00')
) source (StoreNumber, DayOfWeekNumber, OpenTime, CloseTime, DeliveryCutoffTime, LobbyCloseTime)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.StoreOperatingHours target
    WHERE target.StoreId = s.StoreId
      AND target.DayOfWeekNumber = source.DayOfWeekNumber
);
GO

INSERT INTO dbo.Driver (StoreId, DriverCode, DisplayName, EmploymentStatus, HomeZoneCode, EligibilityExpirationDate, AvailabilityStatus, MobilePhone, LastStatusChangeUtc)
SELECT s.StoreId, source.DriverCode, source.DisplayName, source.EmploymentStatus, source.HomeZoneCode, source.EligibilityExpirationDate, source.AvailabilityStatus, source.MobilePhone, source.LastStatusChangeUtc
FROM
(
    VALUES
        (N'014', N'DRV-014-A', N'Luis Mendoza', N'Active', N'NW-CORP', '2027-05-18T01:39:47.894', N'Available', N'425-555-6014', '2026-05-18T01:31:47.894'),
        (N'014', N'DRV-014-B', N'Rachel Kim', N'Active', N'BEL-DT', '2027-05-18T01:39:47.894', N'On Road', N'425-555-6015', '2026-05-18T01:21:47.894'),
        (N'022', N'DRV-022-A', N'Darren Scott', N'Active', N'RED-CAMPUS', '2027-05-18T01:39:47.894', N'Available', N'425-555-6022', '2026-05-18T01:33:47.894'),
        (N'022', N'DRV-022-B', N'Priya Shah', N'Active', N'SAMM-TECH', '2027-05-18T01:39:47.894', N'Break', N'425-555-6023', '2026-05-18T01:25:47.894'),
        (N'031', N'DRV-031-A', N'Jasmine Patel', N'Active', N'SCTR-MALL', '2027-05-18T01:39:47.894', N'On Road', N'206-555-6031', '2026-05-18T01:28:47.894'),
        (N'044', N'DRV-044-A', N'Marcus Bell', N'Active', N'KIRK-DT', '2027-05-18T01:39:47.894', N'Available', N'425-555-6044', '2026-05-18T01:34:47.894'),
        (N'057', N'DRV-057-A', N'Trevor Morris', N'Active', N'AIRPORT-HOTEL', '2027-05-18T01:39:47.894', N'On Road', N'206-555-6057', '2026-05-18T01:19:47.894'),
        (N'057', N'DRV-057-B', N'Nina Alvarez', N'Active', N'DESM-SHUTTLE', '2027-05-18T01:39:47.894', N'Available', N'206-555-6058', '2026-05-18T01:30:47.894'),
        (N'068', N'DRV-068-A', N'Connor Fields', N'Active', N'ISS-HGHTS', '2027-05-18T01:39:47.894', N'Available', N'425-555-6068', '2026-05-18T01:32:47.894'),
        (N'081', N'DRV-081-A', N'Riley Nguyen', N'Active', N'CAMPUS-QUAD', '2027-05-18T01:39:47.894', N'On Road', N'206-555-6081', '2026-05-18T01:23:47.894'),
        (N'081', N'DRV-081-B', N'Elena Park', N'Probation', N'GREEN-LATE', '2026-11-14T01:39:47.894', N'Available', N'206-555-6082', '2026-05-18T01:27:47.894'),
        (N'093', N'DRV-093-A', N'Sean O''Malley', N'Active', N'BALLARD-HILL', '2027-05-18T01:39:47.894', N'Available', N'206-555-6093', '2026-05-18T01:35:47.894')
) source (StoreNumber, DriverCode, DisplayName, EmploymentStatus, HomeZoneCode, EligibilityExpirationDate, AvailabilityStatus, MobilePhone, LastStatusChangeUtc)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Driver target
    WHERE target.DriverCode = source.DriverCode
);
GO

INSERT INTO dbo.StoreOperationsStatus
(
    StoreId,
    DistrictName,
    DispatchTerminalId,
    ManagerOnDuty,
    BoardMode,
    LastStatusRefreshUtc,
    StoreStatus,
    EscalationNote
)
SELECT s.StoreId, source.DistrictName, source.DispatchTerminalId, source.ManagerOnDuty, source.BoardMode, source.LastStatusRefreshUtc, source.StoreStatus, source.EscalationNote
FROM
(
    VALUES
        (N'014', N'North Metro', N'TERM-02', N'M. Delgado', N'Balanced', '2026-05-18T01:35:47.894', N'Normal', N'Late-night corporate catering pickups are clearing inside promise.'),
        (N'022', N'North Metro', N'TERM-04', N'A. Rivera', N'Balanced', '2026-05-18T01:33:47.894', N'Normal', N'Campus group orders are flowing through call center routing normally.'),
        (N'031', N'South Metro', N'TERM-05', N'J. Patel', N'Rush Recovery', '2026-05-18T01:31:47.894', N'Needs Follow-Up', N'Security desk escort waves are slowing stacked mall deliveries.'),
        (N'044', N'North Metro', N'TERM-06', N'L. Romero', N'Balanced', '2026-05-18T01:34:47.894', N'Normal', N'Waterfront condo lobby codes were refreshed before the dinner wave.'),
        (N'057', N'South Metro', N'TERM-03', N'T. Morris', N'Delivery Heavy', '2026-05-18T01:32:47.894', N'Normal', N'Airport hotel strip has two stacked shuttle-hold deliveries on deck.'),
        (N'068', N'East Metro', N'TERM-08', N'C. Foster', N'Balanced', '2026-05-18T01:33:47.894', N'Normal', N'Highlands townhouse runs are back under the dispatch target.'),
        (N'081', N'Campus', N'TERM-07', N'R. Nguyen', N'Counter Hold', '2026-05-18T01:30:47.894', N'Needs Follow-Up', N'Dorm lobby runners are batching card readers between rounds.'),
        (N'093', N'West Metro', N'TERM-09', N'S. O''Brien', N'Balanced', '2026-05-18T01:34:47.894', N'Normal', N'Late-night bar rush is steady but still inside labor targets.')
) source (StoreNumber, DistrictName, DispatchTerminalId, ManagerOnDuty, BoardMode, LastStatusRefreshUtc, StoreStatus, EscalationNote)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.StoreOperationsStatus target
    WHERE target.StoreId = s.StoreId
);
GO

INSERT INTO dbo.StoreOrder
(
    OrderNumber,
    StoreId,
    CustomerName,
    ChannelCode,
    ServiceMode,
    PromiseUtc,
    TicketTotal,
    KitchenStatus,
    DispatchStatus,
    PaymentStatus,
    CreatedUtc
)
SELECT source.OrderNumber, s.StoreId, source.CustomerName, source.ChannelCode, source.ServiceMode, source.PromiseUtc, source.TicketTotal, source.KitchenStatus, source.DispatchStatus, source.PaymentStatus, source.CreatedUtc
FROM
(
    VALUES
        (76001, N'014', N'Bellevue Office Tower Concierge', N'Web', N'Delivery', '2026-05-18T02:01:47.894', CAST(29.95 AS MONEY), N'Make Line', N'Queued', N'Card Hold', '2026-05-17T23:41:47.894'),
        (76002, N'014', N'Bellevue Apartment Lobby Desk', N'Call Center', N'Delivery', '2026-05-18T01:53:47.894', CAST(34.60 AS MONEY), N'Ready', N'Staged', N'Settled', '2026-05-18T00:07:47.894'),
        (76003, N'014', N'Bellevue Hotel Front Desk', N'Phone', N'Delivery', '2026-05-18T01:46:47.894', CAST(29.15 AS MONEY), N'Ready', N'Dispatched', N'Settled', '2026-05-18T00:21:47.894'),
        (76004, N'014', N'Bellevue Night Shift Crew Meal', N'POS', N'Delivery', '2026-05-18T01:35:47.894', CAST(24.10 AS MONEY), N'Ready', N'Out For Delivery', N'Settled', '2026-05-18T00:35:47.894'),
        (76005, N'014', N'Bellevue Family Dinner Order', N'Web', N'Delivery', '2026-05-18T01:11:47.894', CAST(43.00 AS MONEY), N'Completed', N'Delivered', N'Settled', '2026-05-17T23:31:47.894'),
        (76006, N'014', N'Bellevue Late Catering Drop', N'Corporate', N'Delivery', '2026-05-18T00:56:47.894', CAST(59.65 AS MONEY), N'Completed', N'Completed Late', N'Settled', '2026-05-17T23:05:47.894'),
        (76007, N'014', N'Bellevue Pickup Shelf Order', N'Phone', N'Carryout', '2026-05-18T01:49:47.894', CAST(19.65 AS MONEY), N'Ready', N'Carryout Hold', N'Cash Pending', '2026-05-18T01:03:47.894'),
        (76008, N'014', N'Bellevue Walk-Up Pickup', N'Web', N'Carryout', '2026-05-18T01:31:47.894', CAST(22.60 AS MONEY), N'Completed', N'Completed', N'Settled', '2026-05-18T00:41:47.894'),
        (76009, N'014', N'Bellevue Voided Counter Order', N'POS', N'Carryout', '2026-05-18T01:57:47.894', CAST(19.65 AS MONEY), N'Exception', N'Cancelled', N'Voided', '2026-05-18T01:15:47.894'),
        (76011, N'022', N'Redmond Office Tower Concierge', N'Web', N'Delivery', '2026-05-18T02:02:47.894', CAST(30.80 AS MONEY), N'Make Line', N'Queued', N'Card Hold', '2026-05-17T23:39:47.894'),
        (76012, N'022', N'Redmond Apartment Lobby Desk', N'Call Center', N'Delivery', '2026-05-18T01:54:47.894', CAST(35.45 AS MONEY), N'Ready', N'Staged', N'Settled', '2026-05-18T00:05:47.894'),
        (76013, N'022', N'Redmond Hotel Front Desk', N'Phone', N'Delivery', '2026-05-18T01:47:47.894', CAST(30.00 AS MONEY), N'Ready', N'Dispatched', N'Settled', '2026-05-18T00:19:47.894'),
        (76014, N'022', N'Redmond Night Shift Crew Meal', N'POS', N'Delivery', '2026-05-18T01:36:47.894', CAST(24.95 AS MONEY), N'Ready', N'Out For Delivery', N'Settled', '2026-05-18T00:33:47.894'),
        (76015, N'022', N'Redmond Family Dinner Order', N'Web', N'Delivery', '2026-05-18T01:12:47.894', CAST(43.85 AS MONEY), N'Completed', N'Delivered', N'Settled', '2026-05-17T23:29:47.894'),
        (76016, N'022', N'Redmond Late Catering Drop', N'Corporate', N'Delivery', '2026-05-18T00:57:47.894', CAST(60.50 AS MONEY), N'Completed', N'Completed Late', N'Settled', '2026-05-17T23:03:47.894'),
        (76017, N'022', N'Redmond Pickup Shelf Order', N'Phone', N'Carryout', '2026-05-18T01:50:47.894', CAST(20.50 AS MONEY), N'Ready', N'Carryout Hold', N'Cash Pending', '2026-05-18T01:01:47.894'),
        (76018, N'022', N'Redmond Walk-Up Pickup', N'Web', N'Carryout', '2026-05-18T01:32:47.894', CAST(23.45 AS MONEY), N'Completed', N'Completed', N'Settled', '2026-05-18T00:39:47.894'),
        (76019, N'022', N'Redmond Voided Counter Order', N'POS', N'Carryout', '2026-05-18T01:58:47.894', CAST(20.50 AS MONEY), N'Exception', N'Cancelled', N'Voided', '2026-05-18T01:13:47.894'),
        (76021, N'031', N'Tukwila Office Tower Concierge', N'Web', N'Delivery', '2026-05-18T02:03:47.894', CAST(31.65 AS MONEY), N'Make Line', N'Queued', N'Card Hold', '2026-05-17T23:37:47.894'),
        (76022, N'031', N'Tukwila Apartment Lobby Desk', N'Call Center', N'Delivery', '2026-05-18T01:55:47.894', CAST(36.30 AS MONEY), N'Ready', N'Staged', N'Settled', '2026-05-18T00:03:47.894'),
        (76023, N'031', N'Tukwila Hotel Front Desk', N'Phone', N'Delivery', '2026-05-18T01:48:47.894', CAST(30.85 AS MONEY), N'Ready', N'Dispatched', N'Settled', '2026-05-18T00:17:47.894'),
        (76024, N'031', N'Tukwila Night Shift Crew Meal', N'POS', N'Delivery', '2026-05-18T01:37:47.894', CAST(25.80 AS MONEY), N'Ready', N'Out For Delivery', N'Settled', '2026-05-18T00:31:47.894'),
        (76025, N'031', N'Tukwila Family Dinner Order', N'Web', N'Delivery', '2026-05-18T01:13:47.894', CAST(44.70 AS MONEY), N'Completed', N'Delivered', N'Settled', '2026-05-17T23:27:47.894'),
        (76026, N'031', N'Tukwila Late Catering Drop', N'Corporate', N'Delivery', '2026-05-18T00:58:47.894', CAST(61.35 AS MONEY), N'Completed', N'Completed Late', N'Settled', '2026-05-17T23:01:47.894'),
        (76027, N'031', N'Tukwila Pickup Shelf Order', N'Phone', N'Carryout', '2026-05-18T01:51:47.894', CAST(21.35 AS MONEY), N'Ready', N'Carryout Hold', N'Cash Pending', '2026-05-18T00:59:47.894'),
        (76028, N'031', N'Tukwila Walk-Up Pickup', N'Web', N'Carryout', '2026-05-18T01:33:47.894', CAST(24.30 AS MONEY), N'Completed', N'Completed', N'Settled', '2026-05-18T00:37:47.894'),
        (76029, N'031', N'Tukwila Voided Counter Order', N'POS', N'Carryout', '2026-05-18T01:59:47.894', CAST(21.35 AS MONEY), N'Exception', N'Cancelled', N'Voided', '2026-05-18T01:11:47.894'),
        (76031, N'044', N'Kirkland Office Tower Concierge', N'Web', N'Delivery', '2026-05-18T02:01:47.894', CAST(32.50 AS MONEY), N'Make Line', N'Queued', N'Card Hold', '2026-05-17T23:35:47.894'),
        (76032, N'044', N'Kirkland Apartment Lobby Desk', N'Call Center', N'Delivery', '2026-05-18T01:53:47.894', CAST(37.15 AS MONEY), N'Ready', N'Staged', N'Settled', '2026-05-18T00:01:47.894'),
        (76033, N'044', N'Kirkland Hotel Front Desk', N'Phone', N'Delivery', '2026-05-18T01:46:47.894', CAST(31.70 AS MONEY), N'Ready', N'Dispatched', N'Settled', '2026-05-18T00:15:47.894'),
        (76034, N'044', N'Kirkland Night Shift Crew Meal', N'POS', N'Delivery', '2026-05-18T01:35:47.894', CAST(26.65 AS MONEY), N'Ready', N'Out For Delivery', N'Settled', '2026-05-18T00:29:47.894'),
        (76035, N'044', N'Kirkland Family Dinner Order', N'Web', N'Delivery', '2026-05-18T01:11:47.894', CAST(45.55 AS MONEY), N'Completed', N'Delivered', N'Settled', '2026-05-17T23:25:47.894'),
        (76036, N'044', N'Kirkland Late Catering Drop', N'Corporate', N'Delivery', '2026-05-18T00:56:47.894', CAST(62.20 AS MONEY), N'Completed', N'Completed Late', N'Settled', '2026-05-17T22:59:47.894'),
        (76037, N'044', N'Kirkland Pickup Shelf Order', N'Phone', N'Carryout', '2026-05-18T01:49:47.894', CAST(22.20 AS MONEY), N'Ready', N'Carryout Hold', N'Cash Pending', '2026-05-18T00:57:47.894'),
        (76038, N'044', N'Kirkland Walk-Up Pickup', N'Web', N'Carryout', '2026-05-18T01:31:47.894', CAST(25.15 AS MONEY), N'Completed', N'Completed', N'Settled', '2026-05-18T00:35:47.894'),
        (76039, N'044', N'Kirkland Voided Counter Order', N'POS', N'Carryout', '2026-05-18T01:57:47.894', CAST(22.20 AS MONEY), N'Exception', N'Cancelled', N'Voided', '2026-05-18T01:09:47.894'),
        (76041, N'057', N'SeaTac Office Tower Concierge', N'Web', N'Delivery', '2026-05-18T02:02:47.894', CAST(33.35 AS MONEY), N'Make Line', N'Queued', N'Card Hold', '2026-05-17T23:33:47.894'),
        (76042, N'057', N'SeaTac Apartment Lobby Desk', N'Call Center', N'Delivery', '2026-05-18T01:54:47.894', CAST(38.00 AS MONEY), N'Ready', N'Staged', N'Settled', '2026-05-17T23:59:47.894'),
        (76043, N'057', N'SeaTac Hotel Front Desk', N'Phone', N'Delivery', '2026-05-18T01:47:47.894', CAST(32.55 AS MONEY), N'Ready', N'Dispatched', N'Settled', '2026-05-18T00:13:47.894'),
        (76044, N'057', N'SeaTac Night Shift Crew Meal', N'POS', N'Delivery', '2026-05-18T01:36:47.894', CAST(27.50 AS MONEY), N'Ready', N'Out For Delivery', N'Settled', '2026-05-18T00:27:47.894'),
        (76045, N'057', N'SeaTac Family Dinner Order', N'Web', N'Delivery', '2026-05-18T01:12:47.894', CAST(46.40 AS MONEY), N'Completed', N'Delivered', N'Settled', '2026-05-17T23:23:47.894'),
        (76046, N'057', N'SeaTac Late Catering Drop', N'Corporate', N'Delivery', '2026-05-18T00:57:47.894', CAST(63.05 AS MONEY), N'Completed', N'Completed Late', N'Settled', '2026-05-17T22:57:47.894'),
        (76047, N'057', N'SeaTac Pickup Shelf Order', N'Phone', N'Carryout', '2026-05-18T01:50:47.894', CAST(23.05 AS MONEY), N'Ready', N'Carryout Hold', N'Cash Pending', '2026-05-18T00:55:47.894'),
        (76048, N'057', N'SeaTac Walk-Up Pickup', N'Web', N'Carryout', '2026-05-18T01:32:47.894', CAST(26.00 AS MONEY), N'Completed', N'Completed', N'Settled', '2026-05-18T00:33:47.894'),
        (76049, N'057', N'SeaTac Voided Counter Order', N'POS', N'Carryout', '2026-05-18T01:58:47.894', CAST(23.05 AS MONEY), N'Exception', N'Cancelled', N'Voided', '2026-05-18T01:07:47.894'),
        (76051, N'068', N'Issaquah Office Tower Concierge', N'Web', N'Delivery', '2026-05-18T02:03:47.894', CAST(34.20 AS MONEY), N'Make Line', N'Queued', N'Card Hold', '2026-05-17T23:31:47.894'),
        (76052, N'068', N'Issaquah Apartment Lobby Desk', N'Call Center', N'Delivery', '2026-05-18T01:55:47.894', CAST(38.85 AS MONEY), N'Ready', N'Staged', N'Settled', '2026-05-17T23:57:47.894'),
        (76053, N'068', N'Issaquah Hotel Front Desk', N'Phone', N'Delivery', '2026-05-18T01:48:47.894', CAST(33.40 AS MONEY), N'Ready', N'Dispatched', N'Settled', '2026-05-18T00:11:47.894'),
        (76054, N'068', N'Issaquah Night Shift Crew Meal', N'POS', N'Delivery', '2026-05-18T01:37:47.894', CAST(28.35 AS MONEY), N'Ready', N'Out For Delivery', N'Settled', '2026-05-18T00:25:47.894'),
        (76055, N'068', N'Issaquah Family Dinner Order', N'Web', N'Delivery', '2026-05-18T01:13:47.894', CAST(47.25 AS MONEY), N'Completed', N'Delivered', N'Settled', '2026-05-17T23:21:47.894'),
        (76056, N'068', N'Issaquah Late Catering Drop', N'Corporate', N'Delivery', '2026-05-18T00:58:47.894', CAST(63.90 AS MONEY), N'Completed', N'Completed Late', N'Settled', '2026-05-17T22:55:47.894'),
        (76057, N'068', N'Issaquah Pickup Shelf Order', N'Phone', N'Carryout', '2026-05-18T01:51:47.894', CAST(23.90 AS MONEY), N'Ready', N'Carryout Hold', N'Cash Pending', '2026-05-18T00:53:47.894'),
        (76058, N'068', N'Issaquah Walk-Up Pickup', N'Web', N'Carryout', '2026-05-18T01:33:47.894', CAST(26.85 AS MONEY), N'Completed', N'Completed', N'Settled', '2026-05-18T00:31:47.894'),
        (76059, N'068', N'Issaquah Voided Counter Order', N'POS', N'Carryout', '2026-05-18T01:59:47.894', CAST(23.90 AS MONEY), N'Exception', N'Cancelled', N'Voided', '2026-05-18T01:05:47.894'),
        (76061, N'081', N'Seattle Office Tower Concierge', N'Web', N'Delivery', '2026-05-18T02:01:47.894', CAST(35.05 AS MONEY), N'Make Line', N'Queued', N'Card Hold', '2026-05-17T23:29:47.894'),
        (76062, N'081', N'Seattle Apartment Lobby Desk', N'Call Center', N'Delivery', '2026-05-18T01:53:47.894', CAST(39.70 AS MONEY), N'Ready', N'Staged', N'Settled', '2026-05-17T23:55:47.894'),
        (76063, N'081', N'Seattle Hotel Front Desk', N'Phone', N'Delivery', '2026-05-18T01:46:47.894', CAST(34.25 AS MONEY), N'Ready', N'Dispatched', N'Settled', '2026-05-18T00:09:47.894'),
        (76064, N'081', N'Seattle Night Shift Crew Meal', N'POS', N'Delivery', '2026-05-18T01:35:47.894', CAST(29.20 AS MONEY), N'Ready', N'Out For Delivery', N'Settled', '2026-05-18T00:23:47.894'),
        (76065, N'081', N'Seattle Family Dinner Order', N'Web', N'Delivery', '2026-05-18T01:11:47.894', CAST(48.10 AS MONEY), N'Completed', N'Delivered', N'Settled', '2026-05-17T23:19:47.894'),
        (76066, N'081', N'Seattle Late Catering Drop', N'Corporate', N'Delivery', '2026-05-18T00:56:47.894', CAST(64.75 AS MONEY), N'Completed', N'Completed Late', N'Settled', '2026-05-17T22:53:47.894'),
        (76067, N'081', N'Seattle Pickup Shelf Order', N'Phone', N'Carryout', '2026-05-18T01:49:47.894', CAST(24.75 AS MONEY), N'Ready', N'Carryout Hold', N'Cash Pending', '2026-05-18T00:51:47.894'),
        (76068, N'081', N'Seattle Walk-Up Pickup', N'Web', N'Carryout', '2026-05-18T01:31:47.894', CAST(27.70 AS MONEY), N'Completed', N'Completed', N'Settled', '2026-05-18T00:29:47.894'),
        (76069, N'081', N'Seattle Voided Counter Order', N'POS', N'Carryout', '2026-05-18T01:57:47.894', CAST(24.75 AS MONEY), N'Exception', N'Cancelled', N'Voided', '2026-05-18T01:03:47.894'),
        (76071, N'093', N'Seattle Office Tower Concierge', N'Web', N'Delivery', '2026-05-18T02:02:47.894', CAST(35.90 AS MONEY), N'Make Line', N'Queued', N'Card Hold', '2026-05-17T23:27:47.894'),
        (76072, N'093', N'Seattle Apartment Lobby Desk', N'Call Center', N'Delivery', '2026-05-18T01:54:47.894', CAST(40.55 AS MONEY), N'Ready', N'Staged', N'Settled', '2026-05-17T23:53:47.894'),
        (76073, N'093', N'Seattle Hotel Front Desk', N'Phone', N'Delivery', '2026-05-18T01:47:47.894', CAST(35.10 AS MONEY), N'Ready', N'Dispatched', N'Settled', '2026-05-18T00:07:47.894'),
        (76074, N'093', N'Seattle Night Shift Crew Meal', N'POS', N'Delivery', '2026-05-18T01:36:47.894', CAST(30.05 AS MONEY), N'Ready', N'Out For Delivery', N'Settled', '2026-05-18T00:21:47.894'),
        (76075, N'093', N'Seattle Family Dinner Order', N'Web', N'Delivery', '2026-05-18T01:12:47.894', CAST(48.95 AS MONEY), N'Completed', N'Delivered', N'Settled', '2026-05-17T23:17:47.894'),
        (76076, N'093', N'Seattle Late Catering Drop', N'Corporate', N'Delivery', '2026-05-18T00:57:47.894', CAST(65.60 AS MONEY), N'Completed', N'Completed Late', N'Settled', '2026-05-17T22:51:47.894'),
        (76077, N'093', N'Seattle Pickup Shelf Order', N'Phone', N'Carryout', '2026-05-18T01:50:47.894', CAST(25.60 AS MONEY), N'Ready', N'Carryout Hold', N'Cash Pending', '2026-05-18T00:49:47.894'),
        (76078, N'093', N'Seattle Walk-Up Pickup', N'Web', N'Carryout', '2026-05-18T01:32:47.894', CAST(28.55 AS MONEY), N'Completed', N'Completed', N'Settled', '2026-05-18T00:27:47.894'),
        (76079, N'093', N'Seattle Voided Counter Order', N'POS', N'Carryout', '2026-05-18T01:58:47.894', CAST(25.60 AS MONEY), N'Exception', N'Cancelled', N'Voided', '2026-05-18T01:01:47.894')
) source (OrderNumber, StoreNumber, CustomerName, ChannelCode, ServiceMode, PromiseUtc, TicketTotal, KitchenStatus, DispatchStatus, PaymentStatus, CreatedUtc)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.StoreOrder target
    WHERE target.OrderNumber = source.OrderNumber
);
GO

INSERT INTO dbo.DispatchTicket (TicketNumber, StoreOrderId, StoreId, DriverId, RouteZoneId, OrderOrigin, PromiseUtc, StatusCode, TotalAmount, LastUpdatedUtc)
SELECT source.TicketNumber, so.StoreOrderId, s.StoreId, d.DriverId, rz.RouteZoneId, source.OrderOrigin, source.PromiseUtc, source.StatusCode, source.TotalAmount, source.LastUpdatedUtc
FROM
(
    VALUES
        (51001, 76001, N'014', N'DRV-014-A', N'NW-CORP', N'Web', '2026-05-18T02:01:47.894', N'Queued', CAST(29.95 AS MONEY), '2026-05-18T01:59:47.894'),
        (51002, 76002, N'014', N'DRV-014-B', N'BEL-DT', N'CallCenter', '2026-05-18T01:53:47.894', N'Queued', CAST(34.60 AS MONEY), '2026-05-18T01:51:47.894'),
        (51003, 76003, N'014', N'DRV-014-A', N'NW-CORP', N'Phone', '2026-05-18T01:46:47.894', N'Dispatched', CAST(29.15 AS MONEY), '2026-05-18T01:44:47.894'),
        (51004, 76004, N'014', N'DRV-014-B', N'BEL-DT', N'POS', '2026-05-18T01:35:47.894', N'Dispatched', CAST(24.10 AS MONEY), '2026-05-18T01:33:47.894'),
        (51005, 76005, N'014', N'DRV-014-A', N'NW-CORP', N'Web', '2026-05-18T01:11:47.894', N'Completed', CAST(43.00 AS MONEY), '2026-05-18T01:11:47.894'),
        (51006, 76006, N'014', N'DRV-014-B', N'BEL-DT', N'Corporate', '2026-05-18T00:56:47.894', N'CompletedLate', CAST(59.65 AS MONEY), '2026-05-18T00:54:47.894'),
        (51011, 76011, N'022', N'DRV-022-A', N'RED-CAMPUS', N'Web', '2026-05-18T02:02:47.894', N'Queued', CAST(30.80 AS MONEY), '2026-05-18T01:59:47.894'),
        (51012, 76012, N'022', N'DRV-022-B', N'SAMM-TECH', N'CallCenter', '2026-05-18T01:54:47.894', N'Queued', CAST(35.45 AS MONEY), '2026-05-18T01:51:47.894'),
        (51013, 76013, N'022', N'DRV-022-A', N'RED-CAMPUS', N'Phone', '2026-05-18T01:47:47.894', N'Dispatched', CAST(30.00 AS MONEY), '2026-05-18T01:44:47.894'),
        (51014, 76014, N'022', N'DRV-022-B', N'SAMM-TECH', N'POS', '2026-05-18T01:36:47.894', N'Dispatched', CAST(24.95 AS MONEY), '2026-05-18T01:33:47.894'),
        (51015, 76015, N'022', N'DRV-022-A', N'RED-CAMPUS', N'Web', '2026-05-18T01:12:47.894', N'Completed', CAST(43.85 AS MONEY), '2026-05-18T01:12:47.894'),
        (51016, 76016, N'022', N'DRV-022-B', N'SAMM-TECH', N'Corporate', '2026-05-18T00:57:47.894', N'CompletedLate', CAST(60.50 AS MONEY), '2026-05-18T00:54:47.894'),
        (51021, 76021, N'031', N'DRV-031-A', N'SCTR-MALL', N'Web', '2026-05-18T02:03:47.894', N'Queued', CAST(31.65 AS MONEY), '2026-05-18T01:59:47.894'),
        (51022, 76022, N'031', N'DRV-031-A', N'TUK-IND', N'CallCenter', '2026-05-18T01:55:47.894', N'Queued', CAST(36.30 AS MONEY), '2026-05-18T01:51:47.894'),
        (51023, 76023, N'031', N'DRV-031-A', N'SCTR-MALL', N'Phone', '2026-05-18T01:48:47.894', N'Dispatched', CAST(30.85 AS MONEY), '2026-05-18T01:44:47.894'),
        (51024, 76024, N'031', N'DRV-031-A', N'TUK-IND', N'POS', '2026-05-18T01:37:47.894', N'Dispatched', CAST(25.80 AS MONEY), '2026-05-18T01:33:47.894'),
        (51025, 76025, N'031', N'DRV-031-A', N'SCTR-MALL', N'Web', '2026-05-18T01:13:47.894', N'Completed', CAST(44.70 AS MONEY), '2026-05-18T01:13:47.894'),
        (51026, 76026, N'031', N'DRV-031-A', N'TUK-IND', N'Corporate', '2026-05-18T00:58:47.894', N'CompletedLate', CAST(61.35 AS MONEY), '2026-05-18T00:54:47.894'),
        (51031, 76031, N'044', N'DRV-044-A', N'KIRK-DT', N'Web', '2026-05-18T02:01:47.894', N'Queued', CAST(32.50 AS MONEY), '2026-05-18T01:59:47.894'),
        (51032, 76032, N'044', N'DRV-044-A', N'HBR-PT', N'CallCenter', '2026-05-18T01:53:47.894', N'Queued', CAST(37.15 AS MONEY), '2026-05-18T01:51:47.894'),
        (51033, 76033, N'044', N'DRV-044-A', N'KIRK-DT', N'Phone', '2026-05-18T01:46:47.894', N'Dispatched', CAST(31.70 AS MONEY), '2026-05-18T01:44:47.894'),
        (51034, 76034, N'044', N'DRV-044-A', N'HBR-PT', N'POS', '2026-05-18T01:35:47.894', N'Dispatched', CAST(26.65 AS MONEY), '2026-05-18T01:33:47.894'),
        (51035, 76035, N'044', N'DRV-044-A', N'KIRK-DT', N'Web', '2026-05-18T01:11:47.894', N'Completed', CAST(45.55 AS MONEY), '2026-05-18T01:11:47.894'),
        (51036, 76036, N'044', N'DRV-044-A', N'HBR-PT', N'Corporate', '2026-05-18T00:56:47.894', N'CompletedLate', CAST(62.20 AS MONEY), '2026-05-18T00:54:47.894'),
        (51041, 76041, N'057', N'DRV-057-A', N'AIRPORT-HOTEL', N'Web', '2026-05-18T02:02:47.894', N'Queued', CAST(33.35 AS MONEY), '2026-05-18T01:59:47.894'),
        (51042, 76042, N'057', N'DRV-057-B', N'DESM-SHUTTLE', N'CallCenter', '2026-05-18T01:54:47.894', N'Queued', CAST(38.00 AS MONEY), '2026-05-18T01:51:47.894'),
        (51043, 76043, N'057', N'DRV-057-A', N'AIRPORT-HOTEL', N'Phone', '2026-05-18T01:47:47.894', N'Dispatched', CAST(32.55 AS MONEY), '2026-05-18T01:44:47.894'),
        (51044, 76044, N'057', N'DRV-057-B', N'DESM-SHUTTLE', N'POS', '2026-05-18T01:36:47.894', N'Dispatched', CAST(27.50 AS MONEY), '2026-05-18T01:33:47.894'),
        (51045, 76045, N'057', N'DRV-057-A', N'AIRPORT-HOTEL', N'Web', '2026-05-18T01:12:47.894', N'Completed', CAST(46.40 AS MONEY), '2026-05-18T01:12:47.894'),
        (51046, 76046, N'057', N'DRV-057-B', N'DESM-SHUTTLE', N'Corporate', '2026-05-18T00:57:47.894', N'CompletedLate', CAST(63.05 AS MONEY), '2026-05-18T00:54:47.894'),
        (51051, 76051, N'068', N'DRV-068-A', N'ISS-HGHTS', N'Web', '2026-05-18T02:03:47.894', N'Queued', CAST(34.20 AS MONEY), '2026-05-18T01:59:47.894'),
        (51052, 76052, N'068', N'DRV-068-A', N'SQUAK-VAL', N'CallCenter', '2026-05-18T01:55:47.894', N'Queued', CAST(38.85 AS MONEY), '2026-05-18T01:51:47.894'),
        (51053, 76053, N'068', N'DRV-068-A', N'ISS-HGHTS', N'Phone', '2026-05-18T01:48:47.894', N'Dispatched', CAST(33.40 AS MONEY), '2026-05-18T01:44:47.894'),
        (51054, 76054, N'068', N'DRV-068-A', N'SQUAK-VAL', N'POS', '2026-05-18T01:37:47.894', N'Dispatched', CAST(28.35 AS MONEY), '2026-05-18T01:33:47.894'),
        (51055, 76055, N'068', N'DRV-068-A', N'ISS-HGHTS', N'Web', '2026-05-18T01:13:47.894', N'Completed', CAST(47.25 AS MONEY), '2026-05-18T01:13:47.894'),
        (51056, 76056, N'068', N'DRV-068-A', N'SQUAK-VAL', N'Corporate', '2026-05-18T00:58:47.894', N'CompletedLate', CAST(63.90 AS MONEY), '2026-05-18T00:54:47.894'),
        (51061, 76061, N'081', N'DRV-081-A', N'CAMPUS-QUAD', N'Web', '2026-05-18T02:01:47.894', N'Queued', CAST(35.05 AS MONEY), '2026-05-18T01:59:47.894'),
        (51062, 76062, N'081', N'DRV-081-B', N'GREEN-LATE', N'CallCenter', '2026-05-18T01:53:47.894', N'Queued', CAST(39.70 AS MONEY), '2026-05-18T01:51:47.894'),
        (51063, 76063, N'081', N'DRV-081-A', N'CAMPUS-QUAD', N'Phone', '2026-05-18T01:46:47.894', N'Dispatched', CAST(34.25 AS MONEY), '2026-05-18T01:44:47.894'),
        (51064, 76064, N'081', N'DRV-081-B', N'GREEN-LATE', N'POS', '2026-05-18T01:35:47.894', N'Dispatched', CAST(29.20 AS MONEY), '2026-05-18T01:33:47.894'),
        (51065, 76065, N'081', N'DRV-081-A', N'CAMPUS-QUAD', N'Web', '2026-05-18T01:11:47.894', N'Completed', CAST(48.10 AS MONEY), '2026-05-18T01:11:47.894'),
        (51066, 76066, N'081', N'DRV-081-B', N'GREEN-LATE', N'Corporate', '2026-05-18T00:56:47.894', N'CompletedLate', CAST(64.75 AS MONEY), '2026-05-18T00:54:47.894'),
        (51071, 76071, N'093', N'DRV-093-A', N'BALLARD-HILL', N'Web', '2026-05-18T02:02:47.894', N'Queued', CAST(35.90 AS MONEY), '2026-05-18T01:59:47.894'),
        (51072, 76072, N'093', N'DRV-093-A', N'FREMONT-BRIDGE', N'CallCenter', '2026-05-18T01:54:47.894', N'Queued', CAST(40.55 AS MONEY), '2026-05-18T01:51:47.894'),
        (51073, 76073, N'093', N'DRV-093-A', N'BALLARD-HILL', N'Phone', '2026-05-18T01:47:47.894', N'Dispatched', CAST(35.10 AS MONEY), '2026-05-18T01:44:47.894'),
        (51074, 76074, N'093', N'DRV-093-A', N'FREMONT-BRIDGE', N'POS', '2026-05-18T01:36:47.894', N'Dispatched', CAST(30.05 AS MONEY), '2026-05-18T01:33:47.894'),
        (51075, 76075, N'093', N'DRV-093-A', N'BALLARD-HILL', N'Web', '2026-05-18T01:12:47.894', N'Completed', CAST(48.95 AS MONEY), '2026-05-18T01:12:47.894'),
        (51076, 76076, N'093', N'DRV-093-A', N'FREMONT-BRIDGE', N'Corporate', '2026-05-18T00:57:47.894', N'CompletedLate', CAST(65.60 AS MONEY), '2026-05-18T00:54:47.894')
) source (TicketNumber, OrderNumber, StoreNumber, DriverCode, ZoneCode, OrderOrigin, PromiseUtc, StatusCode, TotalAmount, LastUpdatedUtc)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
INNER JOIN dbo.StoreOrder so
    ON so.OrderNumber = source.OrderNumber
   AND so.StoreId = s.StoreId
INNER JOIN dbo.Driver d
    ON d.DriverCode = source.DriverCode
INNER JOIN dbo.RouteZone rz
    ON rz.StoreId = s.StoreId
   AND rz.ZoneCode = source.ZoneCode
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.DispatchTicket target
    WHERE target.TicketNumber = source.TicketNumber
);
GO

SET IDENTITY_INSERT dbo.PosOrderImportBatch ON;
GO

INSERT INTO dbo.PosOrderImportBatch (PosOrderImportBatchId, StoreId, SourceSystem, BatchDate, ImportedUtc, BatchStatus, ItemCount)
SELECT source.PosOrderImportBatchId, s.StoreId, source.SourceSystem, source.BatchDate, source.ImportedUtc, source.BatchStatus, source.ItemCount
FROM
(
    VALUES
        (8801, N'014', N'CorporatePOS', '2026-05-18', '2026-05-18T00:44:47.894', N'Complete', 3),
        (8802, N'022', N'CampusPOS', '2026-05-18', '2026-05-18T00:51:47.894', N'Complete', 3),
        (8803, N'057', N'AirportPOS', '2026-05-18', '2026-05-18T00:37:47.894', N'Partial', 3),
        (8804, N'081', N'DormPOS', '2026-05-18', '2026-05-18T01:01:47.894', N'Complete', 3)
) source (PosOrderImportBatchId, StoreNumber, SourceSystem, BatchDate, ImportedUtc, BatchStatus, ItemCount)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.PosOrderImportBatch target
    WHERE target.PosOrderImportBatchId = source.PosOrderImportBatchId
);
GO

SET IDENTITY_INSERT dbo.PosOrderImportBatch OFF;
GO

INSERT INTO dbo.PosOrderImportItem
(
    PosOrderImportBatchId,
    OrderNumber,
    ChannelCode,
    ServiceMode,
    ImportStatus,
    ImportedTicketTotal,
    ExceptionNote
)
SELECT source.PosOrderImportBatchId, source.OrderNumber, source.ChannelCode, source.ServiceMode, source.ImportStatus, source.ImportedTicketTotal, source.ExceptionNote
FROM
(
    VALUES
        (8801, 76001, N'Web', N'Delivery', N'Complete', CAST(29.95 AS MONEY), NULL),
        (8801, 76002, N'Call Center', N'Delivery', N'Complete', CAST(34.60 AS MONEY), NULL),
        (8801, 76005, N'Web', N'Delivery', N'Complete', CAST(43.00 AS MONEY), NULL),
        (8802, 76011, N'Web', N'Delivery', N'Complete', CAST(30.80 AS MONEY), NULL),
        (8802, 76012, N'Call Center', N'Delivery', N'Complete', CAST(35.45 AS MONEY), NULL),
        (8802, 76015, N'Web', N'Delivery', N'Complete', CAST(43.85 AS MONEY), NULL),
        (8803, 76041, N'Web', N'Delivery', N'Complete', CAST(33.35 AS MONEY), NULL),
        (8803, 76042, N'Call Center', N'Delivery', N'Complete', CAST(38.00 AS MONEY), NULL),
        (8803, 76046, N'Corporate', N'Delivery', N'Complete', CAST(63.05 AS MONEY), NULL),
        (8804, 76061, N'Web', N'Delivery', N'Complete', CAST(35.05 AS MONEY), NULL),
        (8804, 76062, N'Call Center', N'Delivery', N'Complete', CAST(39.70 AS MONEY), NULL),
        (8804, 76068, N'Web', N'Carryout', N'Complete', CAST(27.70 AS MONEY), NULL)
) source (PosOrderImportBatchId, OrderNumber, ChannelCode, ServiceMode, ImportStatus, ImportedTicketTotal, ExceptionNote)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.PosOrderImportItem target
    WHERE target.PosOrderImportBatchId = source.PosOrderImportBatchId
      AND target.OrderNumber = source.OrderNumber
);
GO

INSERT INTO dbo.WorkforceAlert
(
    StoreId,
    TeamName,
    ConcernText,
    ActionRequired,
    SeverityCode,
    EffectiveUtc,
    ResolvedUtc
)
SELECT s.StoreId, source.TeamName, source.ConcernText, source.ActionRequired, source.SeverityCode, source.EffectiveUtc, source.ResolvedUtc
FROM
(
    VALUES
        (N'014', N'Drivers', N'Two corporate runs are stacking in the 112th tower loop.', N'Hold the third downtown stack until the lobby freight elevator clears.', N'Warning', '2026-05-18T00:57:47.894', NULL),
        (N'022', N'Make line', N'Campus combo demand is skewing heavy to large pies after midnight.', N'Keep one cross-trained cashier on dough stretch through close.', N'Watch', '2026-05-18T01:04:47.894', NULL),
        (N'031', N'Dispatch', N'Security escort delays are stretching the mall annex route.', N'Call the on-site supervisor before assigning the next escort-dependent run.', N'Warning', '2026-05-18T01:11:47.894', NULL),
        (N'044', N'Front counter', N'Downtown bar close is pushing a short carryout queue.', N'Keep pickup shelf labels staged at the warmer.', N'Info', '2026-05-18T01:20:47.894', NULL),
        (N'057', N'Drivers', N'Two hotel shuttle drops requested insulated bag swap-outs.', N'Stage extra hot bags by terminal three before the next hotel wave.', N'Watch', '2026-05-18T01:15:47.894', NULL),
        (N'068', N'Prep', N'Highlands townhouse orders are skewing to breadside bundles.', N'Rebuild breadstick par sheets before 2:15 AM.', N'Info', '2026-05-18T01:08:47.894', NULL),
        (N'081', N'Carryout', N'Dorm lobby runners are checking out one card reader at a time.', N'Keep a manual receipt log at the counter until the spare reader is charged.', N'Warning', '2026-05-18T01:17:47.894', NULL),
        (N'093', N'Kitchen', N'Ballard late-night slice demand is pulling from large dough balls.', N'Shift one proof rack forward for the final half hour.', N'Watch', '2026-05-18T01:22:47.894', NULL)
) source (StoreNumber, TeamName, ConcernText, ActionRequired, SeverityCode, EffectiveUtc, ResolvedUtc)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.WorkforceAlert target
    WHERE target.StoreId = s.StoreId
      AND target.TeamName = source.TeamName
      AND target.ConcernText = source.ConcernText
);
GO

INSERT INTO dbo.RouteZoneBulletin
(
    StoreId,
    RouteZoneId,
    BulletinText,
    EffectiveUtc,
    ExpiresUtc,
    RequiresAcknowledgement
)
SELECT s.StoreId, rz.RouteZoneId, source.BulletinText, source.EffectiveUtc, source.ExpiresUtc, source.RequiresAcknowledgement
FROM
(
    VALUES
        (N'014', N'BEL-DT', N'Lincoln tower loading dock closes to food couriers after 2:00 AM.', '2026-05-18T00:04:47.894', NULL, CAST(0 AS BIT)),
        (N'022', N'RED-CAMPUS', N'Campus east residence hall requires desk call-in for every overnight delivery.', '2026-05-18T00:19:47.894', NULL, CAST(1 AS BIT)),
        (N'031', N'SCTR-MALL', N'Southcenter security desk is batching escort requests in ten-minute windows.', '2026-05-18T00:25:47.894', NULL, CAST(1 AS BIT)),
        (N'044', N'HBR-PT', N'Waterfront condo desk prefers contactless handoff at the west entrance.', '2026-05-18T00:39:47.894', NULL, CAST(0 AS BIT)),
        (N'057', N'AIRPORT-HOTEL', N'Hotel shuttle lane is coned off; use valet loop for drop-offs.', '2026-05-18T00:11:47.894', NULL, CAST(0 AS BIT)),
        (N'068', N'ISS-HGHTS', N'Townhouse gate on Highlands Drive sticks during rain; allow extra stop time.', '2026-05-18T00:47:47.894', NULL, CAST(0 AS BIT)),
        (N'081', N'CAMPUS-QUAD', N'Dorm quad desk rotates guest access codes every half hour overnight.', '2026-05-18T00:50:47.894', NULL, CAST(1 AS BIT)),
        (N'093', N'FREMONT-BRIDGE', N'Bridge construction detour adds six minutes to Fremont-side runs.', '2026-05-18T00:32:47.894', NULL, CAST(0 AS BIT))
) source (StoreNumber, ZoneCode, BulletinText, EffectiveUtc, ExpiresUtc, RequiresAcknowledgement)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
INNER JOIN dbo.RouteZone rz
    ON rz.StoreId = s.StoreId
   AND rz.ZoneCode = source.ZoneCode
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.RouteZoneBulletin target
    WHERE target.StoreId = s.StoreId
      AND target.RouteZoneId = rz.RouteZoneId
      AND target.BulletinText = source.BulletinText
);
GO

INSERT INTO dbo.PayrollDailyImport
(
    StoreId,
    WorkDate,
    ScheduledHours,
    WorkedHours,
    OvertimeHours,
    RegularLaborCost,
    OvertimeLaborCost,
    AgencyLaborCost,
    ScheduledDriverSlots,
    FilledDriverSlots,
    OpenDriverSlots,
    CrossTrainedTeamMembers,
    CalloutCount,
    NetSales,
    ImportedUtc
)
SELECT s.StoreId, source.WorkDate, source.ScheduledHours, source.WorkedHours, source.OvertimeHours, source.RegularLaborCost, source.OvertimeLaborCost, source.AgencyLaborCost, source.ScheduledDriverSlots, source.FilledDriverSlots, source.OpenDriverSlots, source.CrossTrainedTeamMembers, source.CalloutCount, source.NetSales, source.ImportedUtc
FROM
(
    VALUES
        (N'014', '2026-05-18', 158.00, 161.50, 4.00, CAST(2325.00 AS MONEY), CAST(128.00 AS MONEY), CAST(35.00 AS MONEY), 14, 13, 1, 2, 0, CAST(7450.00 AS MONEY), '2026-05-18T01:14:47.894'),
        (N'022', '2026-05-18', 164.00, 167.00, 5.50, CAST(2465.00 AS MONEY), CAST(150.00 AS MONEY), CAST(53.00 AS MONEY), 15, 14, 2, 3, 1, CAST(7860.00 AS MONEY), '2026-05-18T01:13:47.894'),
        (N'031', '2026-05-18', 170.00, 172.50, 7.00, CAST(2605.00 AS MONEY), CAST(172.00 AS MONEY), CAST(35.00 AS MONEY), 16, 15, 1, 4, 0, CAST(8270.00 AS MONEY), '2026-05-18T01:12:47.894'),
        (N'044', '2026-05-18', 176.00, 178.00, 4.00, CAST(2745.00 AS MONEY), CAST(194.00 AS MONEY), CAST(53.00 AS MONEY), 17, 16, 2, 2, 1, CAST(8680.00 AS MONEY), '2026-05-18T01:11:47.894'),
        (N'057', '2026-05-18', 182.00, 183.50, 5.50, CAST(2885.00 AS MONEY), CAST(216.00 AS MONEY), CAST(35.00 AS MONEY), 14, 13, 1, 3, 0, CAST(9090.00 AS MONEY), '2026-05-18T01:10:47.894'),
        (N'068', '2026-05-18', 188.00, 189.00, 7.00, CAST(3025.00 AS MONEY), CAST(238.00 AS MONEY), CAST(53.00 AS MONEY), 15, 14, 2, 4, 1, CAST(9500.00 AS MONEY), '2026-05-18T01:09:47.894'),
        (N'081', '2026-05-18', 194.00, 194.50, 4.00, CAST(3165.00 AS MONEY), CAST(260.00 AS MONEY), CAST(35.00 AS MONEY), 16, 15, 1, 2, 0, CAST(9910.00 AS MONEY), '2026-05-18T01:08:47.894'),
        (N'093', '2026-05-18', 200.00, 200.00, 5.50, CAST(3305.00 AS MONEY), CAST(282.00 AS MONEY), CAST(53.00 AS MONEY), 17, 16, 2, 3, 1, CAST(10320.00 AS MONEY), '2026-05-18T01:07:47.894')
) source (StoreNumber, WorkDate, ScheduledHours, WorkedHours, OvertimeHours, RegularLaborCost, OvertimeLaborCost, AgencyLaborCost, ScheduledDriverSlots, FilledDriverSlots, OpenDriverSlots, CrossTrainedTeamMembers, CalloutCount, NetSales, ImportedUtc)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.PayrollDailyImport target
    WHERE target.StoreId = s.StoreId
      AND target.WorkDate = source.WorkDate
);
GO

INSERT INTO dbo.PayrollOvertimeWeeklyImport
(
    StoreId,
    WeekEndingDate,
    DriverOvertimeHours,
    KitchenOvertimeHours,
    ShiftLeadOvertimeHours,
    ImportedUtc
)
SELECT s.StoreId, source.WeekEndingDate, source.DriverOvertimeHours, source.KitchenOvertimeHours, source.ShiftLeadOvertimeHours, source.ImportedUtc
FROM
(
    VALUES
        (N'014', '2026-04-27', 4.40, 2.40, 1.20, '2026-05-15T01:29:47.894'),
        (N'014', '2026-05-04', 4.10, 2.20, 1.10, '2026-05-16T01:29:47.894'),
        (N'014', '2026-05-11', 3.80, 2.00, 1.00, '2026-05-17T01:29:47.894'),
        (N'014', '2026-05-18', 3.50, 1.80, 0.90, '2026-05-18T01:29:47.894'),
        (N'022', '2026-04-27', 5.00, 2.80, 1.40, '2026-05-15T01:28:47.894'),
        (N'022', '2026-05-04', 4.70, 2.60, 1.30, '2026-05-16T01:28:47.894'),
        (N'022', '2026-05-11', 4.40, 2.40, 1.20, '2026-05-17T01:28:47.894'),
        (N'022', '2026-05-18', 4.10, 2.20, 1.10, '2026-05-18T01:28:47.894'),
        (N'031', '2026-04-27', 5.60, 3.20, 1.60, '2026-05-15T01:27:47.894'),
        (N'031', '2026-05-04', 5.30, 3.00, 1.50, '2026-05-16T01:27:47.894'),
        (N'031', '2026-05-11', 5.00, 2.80, 1.40, '2026-05-17T01:27:47.894'),
        (N'031', '2026-05-18', 4.70, 2.60, 1.30, '2026-05-18T01:27:47.894'),
        (N'044', '2026-04-27', 6.20, 3.60, 1.80, '2026-05-15T01:26:47.894'),
        (N'044', '2026-05-04', 5.90, 3.40, 1.70, '2026-05-16T01:26:47.894'),
        (N'044', '2026-05-11', 5.60, 3.20, 1.60, '2026-05-17T01:26:47.894'),
        (N'044', '2026-05-18', 5.30, 3.00, 1.50, '2026-05-18T01:26:47.894'),
        (N'057', '2026-04-27', 6.80, 4.00, 2.00, '2026-05-15T01:25:47.894'),
        (N'057', '2026-05-04', 6.50, 3.80, 1.90, '2026-05-16T01:25:47.894'),
        (N'057', '2026-05-11', 6.20, 3.60, 1.80, '2026-05-17T01:25:47.894'),
        (N'057', '2026-05-18', 5.90, 3.40, 1.70, '2026-05-18T01:25:47.894')
) source (StoreNumber, WeekEndingDate, DriverOvertimeHours, KitchenOvertimeHours, ShiftLeadOvertimeHours, ImportedUtc)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.PayrollOvertimeWeeklyImport target
    WHERE target.StoreId = s.StoreId
      AND target.WeekEndingDate = source.WeekEndingDate
);
GO

INSERT INTO dbo.PayrollTurnoverMonthlyImport
(
    StoreId,
    SummaryMonth,
    BeginningHeadcount,
    HireCount,
    SeparationCount,
    EndingHeadcount,
    ImportedUtc
)
SELECT s.StoreId, source.SummaryMonth, source.BeginningHeadcount, source.HireCount, source.SeparationCount, source.EndingHeadcount, source.ImportedUtc
FROM
(
    VALUES
        (N'014', '2026-05-01', 18, 1, 0, 19, '2026-05-18T01:27:47.894'),
        (N'022', '2026-05-01', 19, 2, 1, 20, '2026-05-18T01:26:47.894'),
        (N'031', '2026-05-01', 20, 3, 0, 23, '2026-05-18T01:25:47.894'),
        (N'044', '2026-05-01', 21, 1, 1, 21, '2026-05-18T01:24:47.894'),
        (N'057', '2026-05-01', 22, 2, 0, 24, '2026-05-18T01:23:47.894'),
        (N'068', '2026-05-01', 23, 3, 1, 25, '2026-05-18T01:22:47.894'),
        (N'081', '2026-05-01', 24, 1, 0, 25, '2026-05-18T01:21:47.894'),
        (N'093', '2026-05-01', 25, 2, 1, 26, '2026-05-18T01:20:47.894')
) source (StoreNumber, SummaryMonth, BeginningHeadcount, HireCount, SeparationCount, EndingHeadcount, ImportedUtc)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.PayrollTurnoverMonthlyImport target
    WHERE target.StoreId = s.StoreId
      AND target.SummaryMonth = source.SummaryMonth
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'StoreOps\03-seed-data.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'StoreOps\03-seed-data.sql');
END
GO
