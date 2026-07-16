using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace tclcnigeria.Controllers
{
    public class AdminResetController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public AdminResetController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Reset()
        {
            var user = await _userManager.FindByEmailAsync("adejazzmind@gmail.com");
            if (user == null) return Content("User not found");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, "Tclc@2026!");
            if (result.Succeeded)
                return Content("SUCCESS! Password is now: Tclc@2026! --- Login then tell me to delete this!");
            return Content("Failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
