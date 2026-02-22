using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace suntadela.web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterConfirmationModel : PageModel
{
    public string? Email { get; set; }

    public IActionResult OnGet(string? email, string? returnUrl = null)
    {
        if (email == null)
            return RedirectToPage("/Index");

        Email = email;
        return Page();
    }
}
