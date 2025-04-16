using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BookGenerator.Data;
using BookGenerator.Models;

namespace BookGenerator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContent _db;
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(AppDbContent db, ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public Book? NewBook { get; set; }

        public void OnGet()
        {
            // Optionally handle the GET logic (e.g., to display some initial data)
        }

        // OnPost handler to handle book submission
        public async Task<IActionResult> OnPostAddToLibraryAsync()
        {
            if (ModelState.IsValid)
            {
                _db.Book.Add(NewBook); // Add the new book to the database
                await _db.SaveChangesAsync(); // Save the changes to the database
                return RedirectToPage("Library"); // Redirect to the Library page
            }

            return Page(); // Stay on the same page if validation fails
        }

        public async Task<IActionResult> OnPostAddBookAsync([FromBody] Book newBook)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid data: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User not logged in" });
            }

            // Add debug logging
            Console.WriteLine($"Adding book: {newBook.Title} for user: {user.Id}");

            newBook.UserId = user.Id;
            await _db.Book.AddAsync(newBook);
            await _db.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}
