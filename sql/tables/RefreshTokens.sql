-- =========================================================================
-- Table: RefreshTokens (SQLite / Turso)
-- Stores cryptographically hashed refresh tokens with rotation and audit trails.
-- =========================================================================

CREATE TABLE IF NOT EXISTS RefreshTokens (
    id TEXT PRIMARY KEY,
    userId TEXT NOT NULL,
    tokenHash TEXT NOT NULL UNIQUE,
    expiresAt TEXT NOT NULL,
    isRevoked INTEGER NOT NULL DEFAULT 0,
    revokedAt TEXT NULL,
    replacedByToken TEXT NULL,
    
    -- Audit & Lifecycle Columns
    createdDate TEXT NOT NULL,
    createdBy TEXT NOT NULL,
    modifiedDate TEXT NULL,
    modifiedBy TEXT NULL,
    modifiedOn TEXT NULL,
    deletedDate TEXT NULL,
    deletedBy TEXT NULL,
    deletedOn TEXT NULL,

    FOREIGN KEY (userId) REFERENCES AuthUsers(id) ON DELETE CASCADE
);

-- Indexes
CREATE INDEX IF NOT EXISTS IX_RefreshTokens_TokenHash ON RefreshTokens(tokenHash);
CREATE INDEX IF NOT EXISTS IX_RefreshTokens_UserId ON RefreshTokens(userId);
CREATE INDEX IF NOT EXISTS IX_RefreshTokens_DeletedDate ON RefreshTokens(deletedDate, deletedBy);
