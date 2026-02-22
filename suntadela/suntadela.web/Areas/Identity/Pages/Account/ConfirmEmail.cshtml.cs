using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using suntadela.web.Models;

namespace suntadela.web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ConfirmEmailModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ConfirmEmailModel> _logger;

    public ConfirmEmailModel(
        UserManager<ApplicationUser> userManager,
        ILogger<ConfirmEmailModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string? userId, string? code, string? returnUrl = null)
    {
        if (userId == null || code == null)
            return RedirectToPage("/Index");

        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {UserId} confirmed their email.", userId);
            StatusMessage = "Thank you for confirming your email.";
        }
        else
        {
            StatusMessage = "Error confirming your email.";
        }

        return Page();
    }
}
