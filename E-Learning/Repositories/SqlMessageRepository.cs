using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlMessageRepository : IMessageRepository
    {
        private readonly ApplicationDBContext dBContext;

        public SqlMessageRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public Message Delete(long id)
        {
            var msg = dBContext.Messages.Find(id);
            if (msg != null)
            {
                dBContext.Messages.Remove(msg);
                dBContext.SaveChanges();
            }
            return msg;
        }

        public Message GetMessage(long id)
        {
            return dBContext.Messages.Find(id);
        }

        public IEnumerable<Message> GetMessages()
        {
            return dBContext.Messages.OrderByDescending(m => m.DateTime)
                    .ThenBy(m => m.IsSeen);
        }

        public Message Send(Message message)
        {
            dBContext.Messages.Add(message);
            dBContext.SaveChanges();

            return message;
        }

        public Message Update(Message messageChanges)
        {
            var message = dBContext.Messages.Attach(messageChanges);
            message.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dBContext.SaveChanges();

            return messageChanges;
        }
    }
}
