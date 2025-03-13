using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }  // Ensisijainen avain (Primary Key)

    [Required]
    public string Username { get; set; }

    [Required]
    public string PasswordHash { get; set; } // Tallennetaan hashattuna!

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
