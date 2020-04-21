using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlUploadedFileRepository : IUploadedFileRepository
    {
        private readonly ApplicationDBContext dBContext;

        public SqlUploadedFileRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public UploadedFile Create(UploadedFile uploadedFile)
        {
            dBContext.UploadedFiles.Add(uploadedFile);
            dBContext.SaveChanges();
            return uploadedFile;
        }

        public UploadedFile FindById(int id)
        {
            return dBContext.UploadedFiles.Find(id);
        }
        public UploadedFile Delete(int id)
        {
            var file = dBContext.UploadedFiles.Find(id);
            if (file != null)
            {
                dBContext.UploadedFiles.Remove(file);
                dBContext.SaveChanges();
            }
            return file;
        }
        public IEnumerable<UploadedFile> GetUploadedFiles()
        {
            return dBContext.UploadedFiles.Include("UploadDirectory");
        }
    }
}
