﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Directory> Directories { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<EmailMessage> EmailMessages { get; set; }

        public DbSet<Course>  Courses{ get; set; }

        public DbSet<Section> Sections { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionContent> SessionContents { get; set; }

        public DbSet<Tag> Tags { get; set; }

    }
}
