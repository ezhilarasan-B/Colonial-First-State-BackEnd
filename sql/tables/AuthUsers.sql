-- =========================================================================
-- Table: AuthUsers (SQLite / Turso)
-- Stores authentication credentials, roles, and full audit tracking.
-- =========================================================================

CREATE TABLE IF NOT EXISTS AuthUsers (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT NOT NULL UNIQUE,
    passwordHash TEXT NOT NULL,
    passwordSalt TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    role TEXT NOT NULL DEFAULT 'User',
    
    -- Audit & Lifecycle Columns
    createdDate TEXT NOT NULL,
    createdBy TEXT NOT NULL,
    modifiedDate TEXT NULL,
    modifiedBy TEXT NULL,
    modifiedOn TEXT NULL,
    deletedDate TEXT NULL,
    deletedBy TEXT NULL,
    deletedOn TEXT NULL
);

-- Indexes
CREATE INDEX IF NOT EXISTS IX_AuthUsers_Username ON AuthUsers(username);
CREATE INDEX IF NOT EXISTS IX_AuthUsers_DeletedDate ON AuthUsers(deletedDate, deletedBy);

-- Seed Data: Default Admin User (Password: Password123!) and Read-Only Staff User authuser (Password: Password123!)
INSERT OR IGNORE INTO AuthUsers (
    id,
    username,
    passwordHash,
    passwordSalt,
    email,
    role,
    createdDate,
    createdBy
) VALUES 
(
    1,
    'admin',
    '3wVbhz7LQmtmXXlnykIcC+pdtVKgr9uVHQ9ceqSpvu0=',
    'c2FsdF9zZWVkXzEyMzQ1Ng==',
    'admin@cfs.com.au',
    'Admin',
    DATETIME('now'),
    'System'
),
(
    2,
    'authuser',
    '3wVbhz7LQmtmXXlnykIcC+pdtVKgr9uVHQ9ceqSpvu0=',
    'c2FsdF9zZWVkXzEyMzQ1Ng==',
    'authuser@cfs.com.au',
    'StaffReadOnly',
    DATETIME('now'),
    'System'
);
