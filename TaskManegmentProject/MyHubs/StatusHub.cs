using Microsoft.AspNetCore.SignalR;
using TaskManegmentProject.Repos;
using TaskManegmentProject.Enums;

namespace TaskManegmentProject.MyHubs
{
    public class StatusHub : Hub
    {
        private readonly ITaskRepository _taskRepo;

        public StatusHub(
            ITaskRepository taskRepo,
             IHubContext<StatusHub> statusHub,
               INotificationRepositry notificationRepository
            )
        {
            _taskRepo = taskRepo;
        }

        public async Task ChangeTaskStatus(string taskId)
        {
            var task = await _taskRepo.GetByIdAsync(taskId);
            if (task != null)
            {
                if (task.Status == TaskManegmentProject.Enums.TaskStatus.Pending)
                {
                    task.Status = TaskManegmentProject.Enums.TaskStatus.InProgress;
                }
                else if (task.Status == TaskManegmentProject.Enums.TaskStatus.InProgress)
                {
                    task.Status = TaskManegmentProject.Enums.TaskStatus.Compleated;
                }
                else if (task.Status == TaskManegmentProject.Enums.TaskStatus.Compleated)
                {
                    return;
                }


                string Id = task.Id;
                string newStatus = task.Status.ToString();

                await _taskRepo.UpdateAsync(task);
                await _taskRepo.SaveAsync();

                await Clients.All.SendAsync("TaskStatusChanged", Id, newStatus);
            }
        }



    }
}

