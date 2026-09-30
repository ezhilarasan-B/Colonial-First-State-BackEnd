-- =========================================================================
-- Table: Clients (SQLite / Turso)
-- Stores corporate client accounts, company affiliations, and assigned staff relationships.
-- =========================================================================

CREATE TABLE IF NOT EXISTS Clients (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    email TEXT NOT NULL,
    phone VARCHAR(10) NOT NULL,
    company TEXT NOT NULL,
    staffId INTEGER NOT NULL,
    
    -- Audit & Lifecycle Columns
    createdDate TEXT NOT NULL,
    createdBy TEXT NOT NULL,
    modifiedDate TEXT NULL,
    modifiedBy TEXT NULL,
    modifiedOn TEXT NULL,
    deletedDate TEXT NULL,
    deletedBy TEXT NULL,
    deletedOn TEXT NULL,

    FOREIGN KEY (staffId) REFERENCES Staff(id) ON DELETE RESTRICT
);

-- Indexes for performance and relationship lookups
CREATE INDEX IF NOT EXISTS IX_Clients_DeletedDate ON Clients(deletedDate, deletedBy);
CREATE INDEX IF NOT EXISTS IX_Clients_StaffId ON Clients(staffId);
CREATE INDEX IF NOT EXISTS IX_Clients_Name ON Clients(name);
CREATE INDEX IF NOT EXISTS IX_Clients_CreatedDate ON Clients(createdDate);

-- Seed Data: Sample corporate clients with integer IDs and staff relationships
INSERT OR IGNORE INTO Clients (id, name, email, phone, company, staffId, createdDate, createdBy) VALUES
(1, 'Acme Global Ventures', 'contact@acmeglobal.com', '0291234567', 'Acme Corp', 1, '2026-02-01T09:00:00Z', 'System'),
(2, 'BluePeak Wealth Partners', 'advisory@bluepeak.com.au', '0398765432', 'BluePeak Financial', 2, '2026-02-15T11:30:00Z', 'System'),
(3, 'Horizon Enterprise Tech', 'operations@horizontech.io', '0733445566', 'Horizon Tech Solutions', 3, '2026-03-01T14:15:00Z', 'System'),
(4, 'Summit Logistics Group', 'contact@summitlogistics.com.au', '0892223344', 'Summit Logistics', 4, '2026-03-10T10:00:00Z', 'System');
