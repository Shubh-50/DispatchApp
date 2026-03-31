using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;

namespace BarcodeBartenderApp
{
    public static class DatabaseHelper
    {
        private static string dbPath = "users.db";
        private static string connectionString =
            $"Data Source={dbPath};Version=3;BusyTimeout=5000;";

        public static void Initialize()
        {
            if (!File.Exists(dbPath))
                SQLiteConnection.CreateFile(dbPath);

            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();

                new SQLiteCommand("PRAGMA journal_mode=WAL;", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Users (
                    Username TEXT PRIMARY KEY,
                    Password TEXT,
                    IsFirstLogin INTEGER)", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS EmailSettings(
                    Sender TEXT,
                    Password TEXT,
                    Receiver TEXT)", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS ShiftSettings(
                    ShiftName TEXT,
                    StartTime TEXT,
                    EndTime TEXT)", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS PdfSettings(
                    FilePath TEXT)", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Parts(
                    PartName TEXT PRIMARY KEY)", con).ExecuteNonQuery();

                // Default Admin
                var adminCount = (long)new SQLiteCommand(
                    "SELECT COUNT(*) FROM Users WHERE Username='admin'", con).ExecuteScalar();

                if (adminCount == 0)
                    new SQLiteCommand("INSERT INTO Users VALUES ('admin','1234',1)", con).ExecuteNonQuery();

                // Default Email Row
                var emailCount = (long)new SQLiteCommand(
                    "SELECT COUNT(*) FROM EmailSettings", con).ExecuteScalar();

                if (emailCount == 0)
                    new SQLiteCommand("INSERT INTO EmailSettings VALUES('','','')", con).ExecuteNonQuery();

                // Default Shift
                var shiftCount = (long)new SQLiteCommand(
                    "SELECT COUNT(*) FROM ShiftSettings", con).ExecuteScalar();

                if (shiftCount == 0)
                {
                    new SQLiteCommand(@"
                    INSERT INTO ShiftSettings VALUES
                    ('A','06:00','14:00'),
                    ('B','14:00','22:00'),
                    ('C','22:00','06:00')", con).ExecuteNonQuery();
                }
            }
        }

        // ================= USER =================

        public static bool AddUser(string user, string pass)
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    con.Open();
                    var cmd = new SQLiteCommand("INSERT INTO Users VALUES (@u,@p,1)", con);
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", pass);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        // ================= PART =================

        public static List<string> GetParts()
        {
            var list = new List<string>();
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand("SELECT PartName FROM Parts", con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(reader["PartName"].ToString());
            }
            return list;
        }

        public static bool AddPart(string part)
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    con.Open();
                    var cmd = new SQLiteCommand("INSERT INTO Parts VALUES (@p)", con);
                    cmd.Parameters.AddWithValue("@p", part);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public static void DeletePart(string part)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand("DELETE FROM Parts WHERE PartName=@p", con);
                cmd.Parameters.AddWithValue("@p", part);
                cmd.ExecuteNonQuery();
            }
        }

        // ================= EMAIL =================

        public static void SaveEmailSettings(string sender, string password, string receiver)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand(
                    "UPDATE EmailSettings SET Sender=@s, Password=@p, Receiver=@r", con);

                cmd.Parameters.AddWithValue("@s", sender);
                cmd.Parameters.AddWithValue("@p", password);
                cmd.Parameters.AddWithValue("@r", receiver);
                cmd.ExecuteNonQuery();
            }
        }

        public static (string sender, string password, string receiver) GetEmailSettings()
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand("SELECT * FROM EmailSettings LIMIT 1", con);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return (
                        reader["Sender"]?.ToString(),
                        reader["Password"]?.ToString(),
                        reader["Receiver"]?.ToString()
                    );
                }
            }
            return ("", "", "");
        }

        // ================= SHIFT =================

        public static List<(string shift, TimeSpan start, TimeSpan end)> GetShifts()
        {
            var list = new List<(string, TimeSpan, TimeSpan)>();

            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand("SELECT * FROM ShiftSettings", con);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string shift = reader["ShiftName"].ToString();
                    TimeSpan.TryParse(reader["StartTime"].ToString(), out TimeSpan s);
                    TimeSpan.TryParse(reader["EndTime"].ToString(), out TimeSpan e);

                    list.Add((shift, s, e));
                }
            }

            return list;
        }

        public static void UpdateShift(string shift, string start, string end)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand(
                    "UPDATE ShiftSettings SET StartTime=@s, EndTime=@e WHERE ShiftName=@shift", con);

                cmd.Parameters.AddWithValue("@s", start);
                cmd.Parameters.AddWithValue("@e", end);
                cmd.Parameters.AddWithValue("@shift", shift);
                cmd.ExecuteNonQuery();
            }
        }

        // ================= VALIDATE USER =================
        public static bool ValidateUser(string username, string password, out bool isFirstLogin)
        {
            isFirstLogin = false;
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(
                        "SELECT Password, IsFirstLogin FROM Users WHERE Username=@u", con);
                    cmd.Parameters.AddWithValue("@u", username);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string dbPass = reader["Password"]?.ToString() ?? "";
                        isFirstLogin = Convert.ToInt32(reader["IsFirstLogin"]) == 1;
                        return dbPass == password;
                    }
                }
            }
            catch { }
            return false;
        }

        // ================= UPDATE PASSWORD =================
        public static void UpdatePassword(string username, string newPassword)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand(
                    "UPDATE Users SET Password=@p, IsFirstLogin=0 WHERE Username=@u", con);
                cmd.Parameters.AddWithValue("@p", newPassword);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.ExecuteNonQuery();
            }
        }

        // ================= PDF =================

        public static string GetPdfPath()
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand("SELECT FilePath FROM PdfSettings LIMIT 1", con);
                var result = cmd.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }
    }
}