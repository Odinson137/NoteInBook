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
    
    public UserController(DataContext context)
    {
        _context = context;
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

}