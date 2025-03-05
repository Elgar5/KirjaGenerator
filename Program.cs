using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

public class KäyttäjäTietokanta
{
    private string _connectionString = "Data Source=Käyttäjä.db";

    public static void Main(string[] args)
    {
        var userDb = new KäyttäjäTietokanta();
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
                CREATE TABLE IF NOT EXISTS Käyttäjät (
                    id INTEGER PRIMARY KEY,
                    käyttäjä_nimi TEXT NOT NULL UNIQUE,
                    salasana TEXT NOT NULL
                );";

                command.ExecuteNonQuery();
            }
        }
    }

    // tiivistää salasana  SHA256 avulla
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    // rekisteröinti metodi
    public bool RegisterUser(string käyttäjäNimi, string salasana)
    {
        // tarkistaa annetun tunnuksen
        if (string.IsNullOrWhiteSpace(käyttäjäNimi) || string.IsNullOrWhiteSpace(salasana))
        {
            Console.WriteLine("Käyttäjänimi ja salasana eivät voi olla tyhjiä.");
            return false;
        }

        // tiivistää salasanan
        string hashedSalasana = HashPassword(salasana);

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    INSERT INTO Käyttäjät (käyttäjä_nimi, salasana) 
                    VALUES (@käyttäjäNimi, @salasana);";

                    command.Parameters.AddWithValue("@käyttäjäNimi", käyttäjäNimi);
                    command.Parameters.AddWithValue("@salasana", hashedSalasana);

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
                Console.WriteLine("Käyttäjänimi on jo käytössä.");
                return false;
            }

            Console.WriteLine($"Virhe rekisteröinnissä: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Odottamaton virhe: {ex.Message}");
            return false;
        }
    }

    // kirjautumus metodi
    public bool LoginUser(string käyttäjäNimi, string salasana)
    {
        // tarkistaa jälleen
        if (string.IsNullOrWhiteSpace(käyttäjäNimi) || string.IsNullOrWhiteSpace(salasana))
        {
            Console.WriteLine("Käyttäjänimi ja salasana eivät voi olla tyhjiä.");
            return false;
        }

        // tiivistää annetun salasanan
        string hashedSalasana = HashPassword(salasana);

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    SELECT COUNT(*) 
                    FROM Käyttäjät 
                    WHERE käyttäjä_nimi = @käyttäjäNimi AND salasana = @salasana;";

                    command.Parameters.AddWithValue("@käyttäjäNimi", käyttäjäNimi);
                    command.Parameters.AddWithValue("@salasana", hashedSalasana);

                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Virhe kirjautumisessa: {ex.Message}");
            return false;
        }
    }

    // voit vaihtaa salasanaa
    public bool ChangePassword(string käyttäjäNimi, string vanhaaSalasana, string uusiSalasana)
    {
        // tarkistaa annetut asiat
        if (string.IsNullOrWhiteSpace(käyttäjäNimi) ||
            string.IsNullOrWhiteSpace(vanhaaSalasana) ||
            string.IsNullOrWhiteSpace(uusiSalasana))
        {
            Console.WriteLine("Kaikki kentät ovat pakollisia.");
            return false;
        }

        // vahvista vanha salasana
        if (!LoginUser(käyttäjäNimi, vanhaaSalasana))
        {
            Console.WriteLine("Vanha salasana on väärä.");
            return false;
        }

        string hashedUusiSalasana = HashPassword(uusiSalasana);

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    UPDATE Käyttäjät 
                    SET salasana = @uusiSalasana 
                    WHERE käyttäjä_nimi = @käyttäjäNimi;";

                    command.Parameters.AddWithValue("@uusiSalasana", hashedUusiSalasana);
                    command.Parameters.AddWithValue("@käyttäjäNimi", käyttäjäNimi);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Virhe salasanan vaihdossa: {ex.Message}");
            return false;
        }
    }
}