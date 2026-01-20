
using Microsoft.Data.Sqlite;
using System;
using System.Windows.Forms;


// Database initialization
public static class DatabaseInitializer
{
    static string ConnectionString = "Data Source=case_timeline.db";
    public static void Initialize()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        // the auto index values are to be converted to alphabetic IDs in the application logic then from that
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Respondents (
                id CHAR(1) PRIMARY KEY,        -- e.g., 'A1-9', 'B1-9', 'C1-9'
                name TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS Legislation (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE
            );

            -- ============================
            --  Evidence Table (REQUIRED BEFORE POINTS AND TIMELINE)
            -- ============================

            CREATE TABLE IF NOT EXISTS Evidence (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                title TEXT NOT NULL,
                file_path TEXT NOT NULL,
                type TEXT NOT NULL,            -- e.g., 'bundle', 'document', 'pdf'
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP
            );

            -- ============================
            --  Claims Table (Updated)
            -- ============================

            CREATE TABLE IF NOT EXISTS Claims (
                id INTEGER PRIMARY KEY AUTOINCREMENT,

                point TEXT NOT NULL,

                -- Multi-select fields stored as strings like 'ABC'
                category TEXT NOT NULL,        -- references Categories.id values
                legislation TEXT NOT NULL,     -- unchanged
                respondent TEXT NOT NULL,      -- references Respondents.id values

                evidence_rating INTEGER NOT NULL,

                -- Evidence linking
                evidence_id INTEGER,                 -- FK to Evidence
                evidence_page INTEGER,               -- page number
                evidence_location_text TEXT,         -- short description

                FOREIGN KEY (evidence_id) REFERENCES Evidence(id)
            );
    
            -- ============================
            --  Testing
            -- ============================

            -- NOTE. code will need to handle alphabetic IDs for multi-select fields

            INSERT OR IGNORE INTO Categories (id, name) VALUES
            ('1', 'civil'),
            ('2', 'family'),
            ('3', 'fraud'),
            ('4', 'medical');

            INSERT OR IGNORE INTO Legislation (name) VALUES
            ('Fraud Act 2006'),
            ('Mental Capacity Act 2005'),
            ('Children Act 1989'),
            ('Civil Procedure Rules');

            INSERT OR IGNORE INTO Respondents (id, name) VALUES
            ('A', 'Angie Samuel'),
            ('B', 'Dr Camel'),
            ('C', 'Judge Hanslip'),
            ('D', 'Lisa Jenkins');
            ";
    command.ExecuteNonQuery();
    }
}