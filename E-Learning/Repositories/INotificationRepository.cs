using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> Create(Notification notification);
        Notification FindById(long id);
        IList<Notification> GetNotifications();
    }
}
