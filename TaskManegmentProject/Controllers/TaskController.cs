using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TaskManegmentProject.DBcontcion;
using TaskManegmentProject.DBcontcion.ViewModels;
using TaskManegmentProject.Enums;
using TaskManegmentProject.MyHubs;
using TaskManegmentProject.Repos;

namespace TaskManegmentProject.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IWorkSpaceRepository _workSpaceRepository;
        private readonly INotificationRepositry _notificationRepository;
        private readonly IHubContext<NotifcationHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public TaskController(
            ITaskRepository taskRepository,
            IWorkSpaceRepository workSpaceRepository, 
            INotificationRepositry notificationRepository,
            IHubContext<NotifcationHub> hubContext,
            UserManager<ApplicationUser> userManager)
        {
            _taskRepository = taskRepository;
            _workSpaceRepository = workSpaceRepository;
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
            _userManager = userManager;
        }
        public async Task<IActionResult> Add(string workSpaceId)
        {

            ApplicationUser userData = await _userManager.GetUserAsync(User);

            WorkSpace workSpace = await _workSpaceRepository.GetByOwnerIdAndWorkSpcaeId(userData.Id, workSpaceId);
            if (workSpace == null)
            {
                return NotFound();
            }

            ViewData["membersList"] = workSpace.Members;
            TaskViewModel model = new TaskViewModel()
            {
                WorkSpaceId = workSpace.Id,

            };

            return View("Add", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TaskViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ApplicationUser userData = await _userManager.GetUserAsync(User);

                WorkSpace workSpace = await _workSpaceRepository.GetByOwnerIdAndWorkSpcaeId(userData.Id,viewModel.WorkSpaceId);

                ViewData["membersList"] = workSpace.Members;

                return View(viewModel);
            }
            ApplicationUser getUser = await _userManager.GetUserAsync(User);

            MyTask newTask = new MyTask()
            {
                CreatedBy = getUser.Id,
                WorkSpaceId = viewModel.WorkSpaceId,
                Title = viewModel.Title,
                Description = viewModel.Description,
                Status = viewModel.Status,
                Priority = viewModel.Priority,
                AssignTo = viewModel.AssignTo
            };
            Notification newNotification = new Notification()
            {
                UserId = getUser.Id,
                WorkspaceId = viewModel.WorkSpaceId,
                Action = Enums.NotificationAction.TaskCreated,
                IsReaded = false
            };

            await _notificationRepository.CreateAsync(newNotification);
            Notification newNotificationData = 
                await _notificationRepository.
                GetNotificationByIdAsync(newNotification.Id);


            await _hubContext.Clients.All.SendAsync("ReceiveNotification", newNotificationData);
            await _taskRepository.CreateAsync(newTask);
            await _taskRepository.SaveAsync();

            return RedirectToAction("Index", "Home", new
            {
                id = viewModel.WorkSpaceId
            });
        }
    }
}
