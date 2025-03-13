using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;

    public LoginModel(AppDbContext db)
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

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == Username);
        if (user == null || user.PasswordHash != HashPassword(Password))
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return Page();
        }

        // Tallenna käyttäjänimi istuntoon
        HttpContext.Session.SetString("Username", Username);

        return RedirectToPage("/Index");
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
