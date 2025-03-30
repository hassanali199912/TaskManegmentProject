using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManegmentProject.DBcontcion;
using TaskManegmentProject.Models;
using TaskManegmentProject.Repos;

namespace TaskManegmentProject.Controllers;

[Authorize]
public class HomeController : Controller
{

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IWorkSpaceRepository _workSpaceRepository;
    private readonly IMemberWorkSpaceRepository _memberWorkSpaceRepository;

    public HomeController(
        UserManager<ApplicationUser> userManger,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger,
        IWorkSpaceRepository workSpaceRepository,
        IMemberWorkSpaceRepository memberWorkSpaceRepository)
    {
        _userManager = userManger;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _logger = logger;
        _workSpaceRepository = workSpaceRepository;
        _memberWorkSpaceRepository = memberWorkSpaceRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int id=1)
    {
        

            ApplicationUser  authUser =  await _userManager.GetUserAsync(User);
            List<WorkSpace> WorkData = 
                await _workSpaceRepository.GetAllWorkSpaceByOwnerId(authUser.Id);
        ViewData["workSpaceList"] = WorkData;

        if (id > WorkData.Count())
        {
            id = WorkData.Count();
        }
        if (id < 0)
        {
            id =1;
        }


        if (WorkData != null && WorkData.Count() !=0)
        {
            ViewData["SelectedWorkSpace"] = WorkData[id-1];
           return View("Index", WorkData[id-1]);
        }
        
        return View("Index");




    }


    [HttpPost]
    public async Task<IActionResult> AddWorkSpace(string workSpaceName)
    {
        ApplicationUser getUser = await _userManager.GetUserAsync(User);
        if(getUser == null)
        {
            return Unauthorized();
        }
        if (string.IsNullOrEmpty(workSpaceName))
        {
            return BadRequest(new { success = false, message = "WorkSpace name cannot be empty." });
        }

        WorkSpace newWork = new WorkSpace()
        {
            Name = workSpaceName,
            OwnerID = getUser.Id

        };
            await _workSpaceRepository.CreateAsync(newWork);
            await _workSpaceRepository.SaveAsync();

        return Json(new {
            success = true,
            workSpace = new
            {
                id = newWork.Id,
                name = newWork.Name
            }
        });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMemberToWorkSpace(string workSpaceId, string email)
    {
        if(workSpaceId == null || email == null)
        {
            return BadRequest(new
            {
                message = "Invalid Data"
            });
        }

        ApplicationUser getUserByEmail = await _userManager.FindByEmailAsync(email);

        if(getUserByEmail == null)
        {
            return NotFound(new
            {
                message = "Invalid User Data"
            });
        }


        MemberWorkSpace addNewMember = new MemberWorkSpace() {
            OwnerID = getUserByEmail.Id,
            WorkSpaceID = workSpaceId,
        };

        await _memberWorkSpaceRepository.CreateAsync(addNewMember);
        await _memberWorkSpaceRepository.SaveAsync();

        return Ok(new
        {
            message = "Data Reached Successfuly",
            data = new {
                email = getUserByEmail.Email,
                imageUrl = getUserByEmail.ImageUrl,
                name=getUserByEmail.Name,
                id =getUserByEmail.Id
            }

        });
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
