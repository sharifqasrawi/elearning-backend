using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlNotificationRepository : INotificationRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlNotificationRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public async Task<Notification> Create(Notification notification)
        {
            dBContext.Notifications.Add(notification);
            await dBContext.SaveChangesAsync();
            return notification;
        }

        public Notification FindById(long id)
        {
            return dBContext.Notifications.Find(id);
        }

        public IList<Notification> GetNotifications()
        {
            return dBContext.Notifications.OrderByDescending(n => n.DateTime).ToList();
        }
    }
}
