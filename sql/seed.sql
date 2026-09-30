-- =========================================================================
-- Seed Data Script (SQLite / Turso)
-- Inserts default admin account, sample staff directory records, and clients.
-- All table IDs use INTEGER values (1, 2, 3...)
-- =========================================================================

-- 1. Default Admin Account & authuser Account (Password: Password123!)
INSERT OR IGNORE INTO AuthUsers (
    id,
    username,
    passwordHash,
    passwordSalt,
    email,
    role,
    createdDate,
    createdBy,
    modifiedDate,
    modifiedBy,
    modifiedOn,
    deletedDate,
    deletedBy,
    deletedOn
) VALUES 
(
    1,
    'admin',
    '3wVbhz7LQmtmXXlnykIcC+pdtVKgr9uVHQ9ceqSpvu0=',
    'c2FsdF9zZWVkXzEyMzQ1Ng==',
    'admin@cfs.com.au',
    'Admin',
    DATETIME('now'),
    'System',
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL
),
(
    2,
    'authuser',
    '3wVbhz7LQmtmXXlnykIcC+pdtVKgr9uVHQ9ceqSpvu0=',
    'c2FsdF9zZWVkXzEyMzQ1Ng==',
    'authuser@cfs.com.au',
    'StaffReadOnly',
    DATETIME('now'),
    'System',
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL
);

-- 2. Sample Active Directory Staff (Integer IDs 1, 2, 3...)
INSERT OR IGNORE INTO Staff (
    id,
    name,
    age,
    city,
    state,
    pincode,
    createdDate,
    createdBy,
    modifiedDate,
    modifiedBy,
    modifiedOn,
    deletedDate,
    deletedBy,
    deletedOn
) VALUES 
(1, 'Oliver Smith', 34, 'Sydney', 'New South Wales', '2000', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(2, 'Sophia Patel', 29, 'Melbourne', 'Victoria', '3000', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(3, 'Liam Chen', 42, 'Brisbane', 'Queensland', '4000', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(4, 'Emma Watson', 31, 'Perth', 'Western Australia', '6000', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(5, 'Noah Sharma', 38, 'Adelaide', 'South Australia', '5000', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(6, 'Olivia Taylor', 27, 'Hobart', 'Tasmania', '7000', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(7, 'Ethan Davis', 45, 'Darwin', 'Northern Territory', '0800', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(8, 'Mia Kumar', 33, 'Canberra', 'Australian Capital Territory', '2601', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(9, 'Lucas Martin', 36, 'Newcastle', 'New South Wales', '2300', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(10, 'Ava Wilson', 28, 'Gold Coast', 'Queensland', '4217', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(11, 'Jack Thompson', 39, 'Wollongong', 'New South Wales', '2500', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(12, 'Amelia Brown', 32, 'Geelong', 'Victoria', '3220', DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL);

-- 3. Sample Clients (Integer IDs 1, 2, 3... with foreign key staffId and 10-digit phone numbers)
INSERT OR IGNORE INTO Clients (
    id,
    name,
    email,
    phone,
    company,
    staffId,
    createdDate,
    createdBy,
    modifiedDate,
    modifiedBy,
    modifiedOn,
    deletedDate,
    deletedBy,
    deletedOn
) VALUES 
(1, 'Acme Global Ventures', 'contact@acmeglobal.com', '0291234567', 'Acme Corp', 1, DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(2, 'BluePeak Wealth Partners', 'advisory@bluepeak.com.au', '0398765432', 'BluePeak Financial', 2, DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(3, 'Horizon Enterprise Tech', 'operations@horizontech.io', '0733445566', 'Horizon Tech Solutions', 3, DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL),
(4, 'Summit Logistics Group', 'contact@summitlogistics.com.au', '0892223344', 'Summit Logistics', 4, DATETIME('now'), 'System', NULL, NULL, NULL, NULL, NULL, NULL);

-- =========================================================================
-- 4. PENDING / UNEXECUTED QUERIES (2 New AuthUsers Records)
-- NOTE: As requested by requirement: "in the seed.sql add the query to insert 2 AuthUsers new records but dont execute."
-- These queries are defined below for manual execution when needed.
-- =========================================================================
-- INSERT INTO AuthUsers (
--     id,
--     username,
--     passwordHash,
--     passwordSalt,
--     email,
--     role,
--     createdDate,
--     createdBy
-- ) VALUES (
--     2,
--     'advisor_user',
--     '3wVbhz7LQmtmXXlnykIcC+pdtVKgr9uVHQ9ceqSpvu0=',
--     'c2FsdF9zZWVkXzEyMzQ1Ng==',
--     'advisor@cfs.com.au',
--     'Staff',
--     DATETIME('now'),
--     'System'
-- );
-- 
-- INSERT INTO AuthUsers (
--     id,
--     username,
--     passwordHash,
--     passwordSalt,
--     email,
--     role,
--     createdDate,
--     createdBy
-- ) VALUES (
--     3,
--     'manager_user',
--     '3wVbhz7LQmtmXXlnykIcC+pdtVKgr9uVHQ9ceqSpvu0=',
--     'c2FsdF9zZWVkXzEyMzQ1Ng==',
--     'manager@cfs.com.au',
--     'Manager',
--     DATETIME('now'),
--     'System'
-- );
