using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IMessageRepository
    {
        IEnumerable<Message> GetMessages();
        Message GetMessage(long id);
        Message Delete(long id);
        Message Send(Message message);
        Message Update(Message messageChanges);

    }
}
