using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<SavedSession> SavedSessions { get; set; }
        public DbSet<CourseRating> CourseRatings { get; set; }
        public DbSet<AppRating> AppRatings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseTag>()
           .HasKey(t => new { t.CourseId, t.TagId });

            modelBuilder.Entity<CourseTag>()
                .HasOne(pt => pt.Course)
                .WithMany(p => p.CourseTags)
                .HasForeignKey(pt => pt.CourseId);

            modelBuilder.Entity<CourseTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.CourseTags)
                .HasForeignKey(pt => pt.TagId);



            modelBuilder.Entity<ClassUser>()
            .HasKey(t => new { t.ClassId, t.UserId });
        
            modelBuilder.Entity<ClassUser>()
                .HasOne(pt => pt.Class)
                .WithMany(p => p.ClassUsers)
                .HasForeignKey(pt => pt.ClassId);

            modelBuilder.Entity<ClassUser>()
                .HasOne(pt => pt.User)
                .WithMany(t => t.ClassUsers)
                .HasForeignKey(pt => pt.UserId);



            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
