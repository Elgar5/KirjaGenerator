using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

public class KäyttäjäTietokanta
{
    public static void Main(string[] args)
    {

    }

    private string _connectionString = "Data Source=Käyttäjä.db";


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
                    käyttäjä_nimi TEXT NOT NULL,
                    salasana TEXT NOT NULL
                );";
            }
        }
    }
}