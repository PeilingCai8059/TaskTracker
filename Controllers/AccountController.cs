using TaskTracker.Models;
using TaskTracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Controllers;

public class AccountController(TaskTrackerContext context,SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : Controller
{
    
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            //login
            var result = await signInManager.PasswordSignInAsync(model.Email!, model.Password!, model.RememberMe, false);
        
            var user = await userManager.GetUserAsync(User);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Invalid login attempt");
        }
        else{
             Console.WriteLine("invalid in login " );
        }
        return View(model);
    }

    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM model, string? returnUrl = null)
        {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            AppUser user = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password!);
            if (result.Succeeded)
            {
                context.Category.AddRange(
                    new Category { categoryName = "Work", UserId = user.Id },
                    new Category { categoryName = "Personal", UserId = user.Id },
                    new Category { categoryName = "Health", UserId = user.Id }, 
                    new Category { categoryName = "Education", UserId = user.Id }, 
                    new Category { categoryName = "Finance", UserId = user.Id }, 
                    new Category { categoryName = "Home", UserId = user.Id } 
                );
                 await context.SaveChangesAsync();
                await signInManager.SignInAsync(user, false);

                return RedirectToLocal(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        
        return RedirectToAction("Index","Home");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {

        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("index", "Home");
    }
}