using Fab.Models.UserFolder;
using Fab.ViewModels.AccountFolder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FabAdmin.Controllers
{
    [Area("FabAdmin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAsync(LoginVM model)
        {
            AppUser user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
            };

             Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (User.Identity.IsAuthenticated)
            {
                // Kullanıcı giriş yapmışsa buraya gelir
                return RedirectToAction("Index", "About");
            }
            return RedirectToAction("Index", "Product");



        }




    }
}
