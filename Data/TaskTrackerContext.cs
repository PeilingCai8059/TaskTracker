using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;

namespace TaskTracker.Data
{
    public class TaskTrackerContext : IdentityDbContext<AppUser>
    {
        public TaskTrackerContext (DbContextOptions<TaskTrackerContext> options)
            : base(options)
        {
        }

        public DbSet<TaskTracker.Models.Task> Task { get; set; } = default!;
        public DbSet<TaskTracker.Models.AppUser> AppUser { get; set; } = default!;
        public DbSet<TaskTracker.Models.User> User { get; set; } = default!;
        public DbSet<TaskTracker.Models.Category> Category { get; set; } = default!;
        public DbSet<TaskTracker.Models.Priority> Priority { get; set; } = default!;
        public DbSet<TaskTracker.Models.Status> Status { get; set; } = default!;
        public DbSet<TaskTracker.Models.Suggestion> Suggestion { get; set; } = default!;
    }
}
