using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManegmentProject.DBcontcion;
using TaskManegmentProject.DBcontcion.ViewModels;
using TaskManegmentProject.Repos;

namespace TaskManegmentProject.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IWorkSpaceRepository _workSpaceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public TaskController(
            ITaskRepository taskRepository,
            IWorkSpaceRepository workSpaceRepository,
            UserManager<ApplicationUser> userManager)
        {
            _taskRepository = taskRepository;
            _workSpaceRepository = workSpaceRepository;
            _userManager = userManager;
        }
        public async Task<IActionResult> Add(string workSpaceId)
        {
            
            ApplicationUser userData = await _userManager.GetUserAsync(User);

            WorkSpace workSpace = await _workSpaceRepository.GetByOwnerIdAndWorkSpcaeId(userData.Id,workSpaceId);
            if (workSpace == null) { 
            return NotFound();
            }

            TaskViewModel model = new TaskViewModel()
            {
                WorkSpaceId = workSpace.Id,
                Members = workSpace.Members
                
            };

            return View("Add",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TaskViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            // التكملة هنا 
            // انشاء التاسك 
            // التعيين 
            // الحفظ والانهاء 


            return RedirectToAction("Index", "Home", new { id = viewModel.WorkSpaceId });
        }
    }
}
