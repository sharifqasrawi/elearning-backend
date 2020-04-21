using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlEmailMessageRepository : IEmailMessageRepository
    {
        private readonly ApplicationDBContext dBContext;

        public SqlEmailMessageRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public EmailMessage Create(EmailMessage emailMessage)
        {
            emailMessage.SendDateTime = DateTime.Now;
            dBContext.EmailMessages.Add(emailMessage);
            dBContext.SaveChanges();
            return emailMessage;
        }

        public IEnumerable<EmailMessage> GetEmailMessages()
        {
            return dBContext.EmailMessages.OrderByDescending(e => e.SendDateTime);
        }
    }
}
