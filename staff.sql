-- =========================================================================
-- Table: Staff (SQLite / Turso)
-- Stores corporate staff directory accounts, contact details, and audit metadata.
-- =========================================================================

CREATE TABLE IF NOT EXISTS Staff (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    age INTEGER NOT NULL,
    city TEXT NOT NULL,
    state TEXT NOT NULL,
    pincode TEXT NOT NULL,
    
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

-- Indexes for performance and soft-delete filtering
CREATE INDEX IF NOT EXISTS IX_Staff_DeletedDate ON Staff(deletedDate, deletedBy);
CREATE INDEX IF NOT EXISTS IX_Staff_Name ON Staff(name);
CREATE INDEX IF NOT EXISTS IX_Staff_CreatedDate ON Staff(createdDate);

-- Seed Data: Sample corporate staff records with integer IDs (1, 2, 3...)
INSERT OR IGNORE INTO Staff (id, name, age, city, state, pincode, createdDate, createdBy) VALUES
(1, 'Oliver Smith', 34, 'Sydney', 'New South Wales', '2000', '2026-01-15T08:30:00Z', 'System'),
(2, 'Sophia Patel', 29, 'Melbourne', 'Victoria', '3000', '2026-01-18T10:15:00Z', 'System'),
(3, 'Liam Chen', 42, 'Brisbane', 'Queensland', '4000', '2026-02-01T14:20:00Z', 'System'),
(4, 'Emma Watson', 31, 'Perth', 'Western Australia', '6000', '2026-02-10T11:45:00Z', 'System'),
(5, 'Noah Sharma', 38, 'Adelaide', 'South Australia', '5000', '2026-02-14T09:00:00Z', 'System'),
(6, 'Olivia Taylor', 27, 'Hobart', 'Tasmania', '7000', '2026-02-20T16:10:00Z', 'System'),
(7, 'Ethan Davis', 45, 'Darwin', 'Northern Territory', '0800', '2026-03-01T12:00:00Z', 'System'),
(8, 'Mia Kumar', 33, 'Canberra', 'Australian Capital Territory', '2601', '2026-03-05T15:30:00Z', 'System'),
(9, 'Lucas Martin', 36, 'Newcastle', 'New South Wales', '2300', '2026-03-10T11:00:00Z', 'System'),
(10, 'Ava Wilson', 28, 'Gold Coast', 'Queensland', '4217', '2026-03-12T09:45:00Z', 'System'),
(11, 'Jack Thompson', 39, 'Wollongong', 'New South Wales', '2500', '2026-03-15T14:15:00Z', 'System'),
(12, 'Amelia Brown', 32, 'Geelong', 'Victoria', '3220', '2026-03-18T16:20:00Z', 'System');
