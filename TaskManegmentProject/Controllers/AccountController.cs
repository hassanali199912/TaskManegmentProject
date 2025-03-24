using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using TaskManegmentProject.DBcontcion;
using TaskManegmentProject.DBcontcion.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManegmentProject.Controllers
{
        [AllowAnonymous]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManger,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManger;
            _signInManager = signInManager;
            _logger = logger;
        }




        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpGet]
        public IActionResult Regiester()
        {
            return View("Regiester");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUser loginUser)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Invalid Email Or Password");
                    return View("Login", loginUser);

                }

                ApplicationUser user = await _userManager.FindByEmailAsync(loginUser.Email);
                if(user == null)
                {
                    ModelState.AddModelError("", "Email Not Registed , Sign Up Frist");
                    return View("Login", loginUser);

                }

                /*
                 * دي بستجل دخول علي طول , مش مش هاعرف اضيف climes 
             var resule = 
                await _signInManager.
                PasswordSignInAsync(user, loginUser.Password,true,lockoutOnFailure:false);
             if (!resule.Succeeded)
             {
                 ModelState.AddModelError("", "Invalid Creditional");
                 return View("Login", loginUser);
                }

                 */

                bool founded = await _userManager.CheckPasswordAsync(user, loginUser.Password);
                if (!founded)
                {
                    ModelState.AddModelError("", "Invalid Creditional");
                    return View("Login", loginUser);

                }

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("Name", user.Name));
                claims.Add(new Claim("Password", user.PasswordHash));

                await _signInManager.SignInWithClaimsAsync(user, true, claims);
                _logger.LogInformation("User {Email} Login Successfuly ", loginUser.Email);


                return RedirectToAction("Index", "Home");
            }catch(Exception e) {
                _logger.LogError("An Error Happen During login {Email}", loginUser.Email);

                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View("Login", loginUser);

            }

        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Regiester(RegiesterUser regiesterUser)
        
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("ErrorMessage", "Invalid Credintional Data");
                    return View("Regiester", regiesterUser);
                }

                ApplicationUser existUser = await _userManager.FindByEmailAsync(regiesterUser.Email);
                if(existUser != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Email Already Exist , Go To Login ");
                    return View("Regiester", regiesterUser);
                }

                ApplicationUser newUser = new ApplicationUser
                {
                    UserName=regiesterUser.Email,
                    Name = regiesterUser.Name,
                    Email = regiesterUser.Email,
                                        
                };

                var result = await _userManager.CreateAsync(newUser, regiesterUser.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("ErrorMessage", error.Description);
                    }
                    //ModelState.AddModelError("", "Error Happen , Try Again");

                    return View("Regiester", regiesterUser);
                }

                //IdentityResult addRole =  await _userManager.AddToRoleAsync(newUser, "Admin");
                List<Claim> claims = new List<Claim>{

                    new Claim("Name", regiesterUser.Name),
                    new Claim("Email", regiesterUser.Email)
                    //,new Claim("Role",addRole.ToString())

                };
                await _signInManager.SignInWithClaimsAsync(newUser,true,claims);
                _logger.LogInformation("User {Email} Register Successfuly ", regiesterUser.Email);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                _logger.LogError("An Error Happen During Register {Email}", regiesterUser.Email);

                ModelState.AddModelError("ErrorMessage", "An unexpected error occurred. Please try again later.");
                return View("Regiester", regiesterUser);

            }


        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
