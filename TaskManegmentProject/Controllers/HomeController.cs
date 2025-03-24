using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManegmentProject.DBcontcion;
using TaskManegmentProject.Models;

namespace TaskManegmentProject.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public HomeController(
        UserManager<ApplicationUser> userManger,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManger;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _logger = logger;
    }


    public IActionResult Index()
    {

        return View();
    }




    // فيها كلام مهم كتير فوي
    //public async Task<IActionResult> Index()
    //{

    //    //if (User.Identity.IsAuthenticated)
    //    //{
    //    //    var getUser = await _userManager.GetUserAsync(User);
    //    //    _logger.LogInformation("User is logged in:");
    //    //    _logger.LogInformation($"User ID: {getUser.Id}");
    //    //    _logger.LogInformation($"User Name: {getUser.Name}");
    //    //    _logger.LogInformation($"User Email: {getUser.Email}");

    //    //    var calems = User.Claims;
    //    //    foreach(var cal in calems)
    //    //    {
    //    //        _logger.LogInformation($"{cal.Type}: {cal.Value}");

    //    //    }

    //    //    var userRole = await _userManager.GetRolesAsync(getUser);

    //    //    if (userRole.Any())
    //    //    {
    //    //        _logger.LogInformation("User Roles:");
    //    //        foreach (var role in userRole)
    //    //        {
    //    //            _logger.LogInformation($"Role: {role}");
    //    //        }
    //    //    }
          
    //    //        IdentityResult createRole = await _roleManager.CreateAsync(new IdentityRole { Name = "Viewer" });

    //    //        IdentityResult roleRes = await _userManager.AddToRoleAsync(getUser, "Admin");

    //    //    if (roleRes.Succeeded)
    //    //    {
    //    //            _logger.LogInformation("Successfully added 'Admin' role to the user.");
    //    //    }
    //    //    else{
    //    //            foreach (var er in roleRes.Errors)
    //    //            {
    //    //                Console.WriteLine(er.Description);
    //    //            }
    //    //    }
         


    //    //}

    //    IdentityRole? singlerole = await _roleManager.FindByNameAsync("Admin");
    //    Console.WriteLine(singlerole.Id);
    //    return View();
    //}

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
