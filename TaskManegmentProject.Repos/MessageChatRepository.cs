using TaskManegmentProject.DBcontcion;

namespace TaskManegmentProject.Repos
{
    public class MessageChatRepository: Repository<MessageChat> , IMessageChatRepository
    {

        private readonly TMContextDB _context;

        public MessageChatRepository(TMContextDB context) : base(context)
        {
            _context = context;

        }
    }
}
