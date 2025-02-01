using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Models;
using Note2Book.ViewModels;

namespace Note2Book.Controllers;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly DataContext _context;
    private readonly IWebHostEnvironment _env;
    
    public UserController(DataContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

   
    [HttpGet]
    [Route("Login")]
    public IActionResult Login()
    {
        var response = new LoginViewModel();
        return View(response);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid) return View(loginViewModel);

        // Поиск пользователя по логину
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == loginViewModel.Login);
    
        if (user == null)
        {
            TempData["Error"] = "Пользователь не найден";
            return View(loginViewModel);
        }

        // Проверка пароля
        if (user.Password != loginViewModel.Password)
        {
            TempData["Error"] = "Неправильный пароль";
            return View(loginViewModel);
        }

        // Успешная аутентификация, создание куки
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
    
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        
        Response.Cookies.Append("UserId", user.Id.ToString());
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    [Route("Register")]
    public IActionResult Register()
    {
        var response = new RegisterViewModel();
        return View(response);
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid) return View(registerViewModel);

        if (registerViewModel.Password != registerViewModel.RepeatPassword)
        {
            return View(registerViewModel);
        }
        
        var user = new User
        {
            Name = registerViewModel.Name,
            Login = registerViewModel.Login,
            Password = registerViewModel.Password,
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("Update")]
    public async Task<IActionResult> Update(User user, IFormFile? profileImage)
    {
        var dbUser = await _context.Users.SingleAsync(c => c.Id == user.Id);
        if (profileImage != null)
        {
            // Путь к папке profiles
            var profilesFolder = Path.Combine(_env.WebRootPath, "images", "profiles");

            // Создание папки, если её нет
            if (!Directory.Exists(profilesFolder))
            {
                Directory.CreateDirectory(profilesFolder);
            }

            // Генерация уникального имени файла
            var uniqueFileName = Guid.NewGuid() + "_" + profileImage.FileName;
            var imagePath = Path.Combine(profilesFolder, uniqueFileName);

            // Сохранение файла
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                profileImage.CopyTo(stream);
            }

            // Обновление пути к фотографии профиля
            dbUser.ProfileImage = $"/images/profiles/{uniqueFileName}";
        }

        dbUser.Login = user.Login;
        dbUser.Name = user.Name;
        if (!string.IsNullOrEmpty(user.Password))
            dbUser.Password = user.Password;

        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}