using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;

namespace TaskTracker.Data
{
    public class TaskTrackerContext : DbContext
    {
        public TaskTrackerContext (DbContextOptions<TaskTrackerContext> options)
            : base(options)
        {
        }

        public DbSet<TaskTracker.Models.Task> Task { get; set; } = default!;
        public DbSet<TaskTracker.Models.User> User { get; set; } = default!;
        public DbSet<TaskTracker.Models.Category> Category { get; set; } = default!;
    }
}
