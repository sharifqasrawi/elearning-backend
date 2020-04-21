using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IEmailMessageRepository
    {
        EmailMessage Create(EmailMessage emailMessage);
        IEnumerable<EmailMessage> GetEmailMessages();
    }
}
