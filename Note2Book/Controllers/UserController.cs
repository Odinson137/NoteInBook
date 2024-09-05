using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.ViewModels;

namespace Note2Book.Controllers;

public class UserController : Controller
{
    private readonly DataContext _context;
    
    public UserController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var assdf = await _context.Chapters.FirstAsync();
        var model = new
        {
            Name = "sdf",
            Test = "sdf"
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        var response = new LoginViewModel();
        return View(response);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid) return View(loginViewModel);

        // var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);
        //
        // if (user != null)
        // {
        //     //User is found, check password
        //     var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
        //     if (passwordCheck)
        //     {
        //         //Password correct, sign in
        //         var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
        //         if (result.Succeeded)
        //         {
        //             return RedirectToAction("Index", "Race");
        //         }
        //     }
        //     //Password is incorrect
        //     TempData["Error"] = "Wrong credentials. Please try again";
        //     return View(loginViewModel);
        // }
        // //User not found
        // TempData["Error"] = "Wrong credentials. Please try again";
        return View(loginViewModel);
    }

    [HttpGet]
    public IActionResult Register()
    {
        var response = new RegisterViewModel();
        return View(response);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid) return View(registerViewModel);

        // var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
        // if (user != null)
        // {
        //     TempData["Error"] = "This email address is already in use";
        //     return View(registerViewModel);
        // }

        // var newUser = new AppUser()
        // {
        //     Email = registerViewModel.EmailAddress,
        //     UserName = registerViewModel.EmailAddress
        // };
        // var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);
        //
        // if (newUserResponse.Succeeded)
        //     await _userManager.AddToRoleAsync(newUser, UserRoles.User);

        return RedirectToAction("Index", "Race");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        return RedirectToAction("Index", "Race");
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}