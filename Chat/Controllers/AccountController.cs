using Chat.Models;
using Chat.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IWebHostEnvironment _hostingEnvironment;
    
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment hostingEnvironment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var userUserName = await _userManager.FindByNameAsync(model.UserName);
        if (userUserName is not null)
        {
            ViewBag.Error = "Такой логин занят";
            return View(model);
        }
        var userEmail = await _userManager.FindByEmailAsync(model.Email);
        if (userEmail is not null)
        {
            ViewBag.Error = "Такая почта занята";
            return View(model);
        }
        
        if (ModelState.IsValid)
        {
            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                string extension = Path.GetExtension(model.AvatarFile.FileName);
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    ViewBag.Error = "Расширение файла должно быть .jpg, .jpeg или .png";
                    return View();
                }
                
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AvatarFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.AvatarFile.CopyTo(stream);
                }

                model.AvatarFileName = uniqueFileName;
            }
            else
            {
                model.AvatarFileName =
                    "d16c062c-6fa8-44f1-9524-8ce7b9a5bd2e_User.jpg";
            }
            
            var currentDate = DateTime.Now;
            var minDateOfBirth = currentDate.AddYears(-18);

            if (model.DateOfBirth > minDateOfBirth)
            {
                ModelState.AddModelError("DateOfBirth", "Вы должны быть старше 18 лет.");
                return View(model);
            }
            
            User user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                AvatarFileName = model.AvatarFileName,
                DateOfBirth = model.DateOfBirth
                
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _userManager.FindByEmailAsync(model.EmailOrUserName);
            if (user is null)
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUserName);
            }
            
            if (user is not null)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return RedirectToAction(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}