using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

public class UserDataBase
{
    private string _connectionString = "Data Source=User.db";

    public static void Main(string[] args)
    {
        var userDb = new UserDataBase();
        userDb.CreateTable();

        // esimerkki
        bool registeredSuccessfully = userDb.RegisterUser("testUser", "password123");
        Console.WriteLine($"Registration result: {registeredSuccessfully}");

        bool loginSuccessful = userDb.LoginUser("testUser", "password123");
        Console.WriteLine($"Login result: {loginSuccessful}");
    }

    public void CreateTable()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    id INTEGER PRIMARY KEY,
                    user_name TEXT NOT NULL UNIQUE,
                    password TEXT NOT NULL
                );";

                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    // tiivistää salasanan SHA256 avulla
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }


    public bool RegisterUser(string userName, string password)
    {
        // tarkistaa annetun tunnuksen
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Username and password can not be empty.");
            return false;
        }

        // tiivistää passwordn
        string hashedPassword = HashPassword(password);

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    INSERT INTO Users (user_name, password) 
                    VALUES (@userName, @password);";

                    command.Parameters.AddWithValue("@userName", userName);
                    command.Parameters.AddWithValue("@password", hashedPassword);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        catch (SqliteException ex)
        {
            // tarkistaa että on uniikki
            if (ex.SqliteErrorCode == 19) // SQLite virhe jos ei uniikki
            {
                Console.WriteLine("Username is allready in use.");
                return false;
            }

            Console.WriteLine($"Error while registering: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled exeption: {ex.Message}");
            return false;
        }
    }


    public bool LoginUser(string userName, string password)
    {
        // tarkistaa jälleen
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Username and password can not be empty.");
            return false;
        }

        // tiivistää annetun salasanan
        string hashedPassword = HashPassword(password);

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    SELECT COUNT(*) 
                    FROM Users 
                    WHERE user_name = @userName AND password = @password;";

                    command.Parameters.AddWithValue("@userName", userName);
                    command.Parameters.AddWithValue("@password", hashedPassword);

                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return false;
        }
    }


    public bool ChangePassword(string userName, string oldPassword, string newPassword)
    {
        // tarkistaa annetut asiat
        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(oldPassword) ||
            string.IsNullOrWhiteSpace(newPassword))
        {
            Console.WriteLine("All fields required.");
            return false;
        }

        // vahvista vanha salasana
        if (!LoginUser(userName, oldPassword))
        {
            Console.WriteLine("Old password is incorrect");
            return false;
        }

        string hashedNewPassword = HashPassword(newPassword);

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    UPDATE Users 
                    SET password = @newPassword 
                    WHERE user_name = @userName;";

                    command.Parameters.AddWithValue("@newPassword", hashedNewPassword);
                    command.Parameters.AddWithValue("@userName", userName);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while changing password: {ex.Message}");
            return false;
        }
    }
    // metodi, joka liittää käyttäjä ID:n luodulle käyttäjätunnukselle joka tallennetaan databaseen. Käyttäjätunnus tai ID ei voi olla null.
    public int GetUserId(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            Console.WriteLine("Username cannot be empty.");
            return -1;
        }

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    SELECT id 
                    FROM Users 
                    WHERE user_name = @userName;";

                    command.Parameters.AddWithValue("@userName", userName);

                    object result = command.ExecuteScalar();
                    return result == null ? -1 : Convert.ToInt32(result);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user ID: {ex.Message}");
            return -1;
        }
    }
    public void DeleteUser(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            Console.WriteLine("Username cannot be empty.");
        }

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    DELETE
                    FROM Users 
                    WHERE user_name = @userName;";

                    command.Parameters.AddWithValue("@userName", userName);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while deleting user: {ex.Message}");
           
        }
    }
}