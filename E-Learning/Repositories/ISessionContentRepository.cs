using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ISessionContentRepository
    {
        SessionContent Create(SessionContent SessionContent);
        IList<SessionContent> GetSessionContents(long sessionId);
        SessionContent Update(SessionContent sessionContentChanges);
        SessionContent Delete(long id);
        SessionContent FindById(long id);
    }
}
