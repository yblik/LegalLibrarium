
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
        command.CommandText = @"
        CREATE TABLE IF NOT EXISTS Claims (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            point TEXT NOT NULL,
            type TEXT NOT NULL,
            category TEXT NOT NULL,
            legislation TEXT NOT NULL,
            respondent TEXT NOT NULL,
            evidence_rating INTEGER NOT NULL
        );
        CREATE TABLE IF NOT EXISTS Types (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL UNIQUE
        );
        CREATE TABLE IF NOT EXISTS Categories (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL UNIQUE
        );
        CREATE TABLE IF NOT EXISTS Legislation (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL UNIQUE
        );
        CREATE TABLE IF NOT EXISTS Respondents (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL UNIQUE
        );
        INSERT OR IGNORE INTO Types (name) VALUES
        ('Medical negligence'),
        ('Munchausen by proxy'),
        ('DLA fraud');
        INSERT OR IGNORE INTO Categories (name) VALUES
        ('fraud'),
        ('medical'),
        ('civil'),
        ('family');
        INSERT OR IGNORE INTO Legislation (name) VALUES
        ('Fraud Act 2006'),
        ('Mental Capacity Act 2005'),
        ('Children Act 1989'),
        ('Civil Procedure Rules');
        INSERT OR IGNORE INTO Respondents (name) VALUES
        ('Angie Samuel'),
        ('Lisa Jenkins'),
        ('Judge Hanslip'),
        ('Dr Camel');
        ";
        command.ExecuteNonQuery();
    }
}