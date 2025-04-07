using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using TaskManegmentProject.DBcontcion;
using TaskManegmentProject.DBcontcion.ViewModels;
using TaskManegmentProject.Repos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManegmentProject.Controllers
{
        [AllowAnonymous]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IWorkSpaceRepository _workSpaceContextl;

        public AccountController(
            UserManager<ApplicationUser> userManger,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger ,
            IWorkSpaceRepository workSpaceContextl
            )
        {
            _userManager = userManger;
            _signInManager = signInManager;
            _logger = logger;
            _workSpaceContextl = workSpaceContextl;
        }




        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Login");
        }

        [HttpGet]
        public IActionResult Regiester()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
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
                 * دي بستجل دخول علي طول , لو مش عاوز اضيف climes 
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
                    UserName=regiesterUser.Name,
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
                WorkSpace work = new WorkSpace()
                {
                    Name = "Frist Work Space",
                    OwnerID = newUser.Id,
                    
                };
              await  _workSpaceContextl.CreateAsync(work);
              await  _workSpaceContextl.SaveAsync();

                WorkSpace WorkData = await _workSpaceContextl.GetByOwnerId(newUser.Id);


                return RedirectToAction("Index", "Home", WorkData);
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
