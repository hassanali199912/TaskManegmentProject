using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManegmentProject.DBcontcion;

namespace TaskManegmentProject.Repos
{
    public class NotificationRepositry : Repository<Notification>, INotificationRepositry
    {
        public NotificationRepositry(TMContextDB DB) : base(DB)
        {
        }
    }
}
