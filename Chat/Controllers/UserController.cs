using Chat.Context;
using Chat.Models;
using Chat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers;

public class UserController : Controller
{
    private ChatContext _db;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public UserController(ChatContext db, UserManager<User> userManager, IWebHostEnvironment hostingEnvironment)
    {
        _db = db;
        _userManager = userManager;
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        
        UserViewModel userViewModel = new UserViewModel
        {
            Id = currentUser.Id,
            UserName = currentUser.UserName,
            Email = currentUser.Email,
            DateOfBirth = currentUser.DateOfBirth,
            AvatarFileName = currentUser.AvatarFileName
        };

        return View(userViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }

        var user = _db.Users.FirstOrDefault(u => u.Id == currentUser.Id);

        UserViewModel userViewModel = new UserViewModel()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            AvatarFileName = user.AvatarFileName,
        };
        return View(userViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserViewModel model)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        var user = _db.Users.FirstOrDefault(u => u.Id == model.Id);
        
        if (model.AvatarFile != null && model.AvatarFile.Length > 0)
        {
            string extension = Path.GetExtension(model.AvatarFile.FileName);
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                ViewBag.Error = "Расширение файла должно быть .jpg, .jpeg или .png";
                return RedirectToAction("Edit");
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
            model.AvatarFileName = user.AvatarFileName;
        }
        
        var currentDate = DateTime.Now;
        var minDateOfBirth = currentDate.AddYears(-18);

        if (model.DateOfBirth > minDateOfBirth)
        {
            ModelState.AddModelError("DateOfBirth", "Вы должны быть старше 18 лет.");
            return View(model);
        }

        if (string.IsNullOrEmpty(model.UserName))
        {
            model.UserName = user.UserName;
        }
        if (string.IsNullOrEmpty(model.Email))
        {
            model.Email = user.Email;
        }
        
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.DateOfBirth = model.DateOfBirth;
        user.AvatarFileName = model.AvatarFileName;
        
        var roles = await _userManager.GetRolesAsync(currentUser);
        foreach (var role in roles)
            if (currentUser.Id == model.Id || role == "admin")
            {
                _db.Users.Update(user);
                _db.SaveChanges();
            }

        return RedirectToAction("Index");
    }
    
    
}