using TaskManegmentProject.DBcontcion;

namespace TaskManegmentProject.Repos
{
    public interface IWorkSpaceRepository :IRepository<WorkSpace>
    {
        Task<WorkSpace> GetByOwnerId(string id);
        Task<List<WorkSpace>> GetAllWorkSpaceByOwnerId(string id);

    }
}
