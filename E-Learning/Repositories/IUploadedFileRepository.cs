using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IUploadedFileRepository
    {
        IEnumerable<UploadedFile> GetUploadedFiles();
        UploadedFile Create(UploadedFile uploadedFile);
        UploadedFile FindById(int id);
        UploadedFile Delete(int id);


    }
}
