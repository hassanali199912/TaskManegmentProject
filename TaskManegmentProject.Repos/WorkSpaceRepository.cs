using TaskManegmentProject.DBcontcion;

namespace TaskManegmentProject.Repos
{
    public class WorkSpaceRepository : Repository<WorkSpace> ,IWorkSpaceRepository
    {
        private readonly TMContextDB _context;

        public WorkSpaceRepository(TMContextDB context) : base(context)
        {
            _context = context;

        }
    }
}
