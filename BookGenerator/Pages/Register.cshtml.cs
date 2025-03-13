using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _db;

    public RegisterModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public string Username { get; set; }
    
    [BindProperty]
    public string Password { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ModelState.AddModelError("", "Username and password are required.");
            return Page();
        }

        // Tarkista, onko käyttäjänimi jo käytössä
        if (await _db.Users.AnyAsync(u => u.Username == Username))
        {
            ModelState.AddModelError("", "Username already exists.");
            return Page();
        }

        // Luo hashattu salasana
        string passwordHash = HashPassword(Password);

        // Lisää uusi käyttäjä tietokantaan
        var user = new User { Username = Username, PasswordHash = passwordHash };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Account/Login");
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
