using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Data; // Update this with your actual namespace
using System;
using System.Linq;

namespace TaskTracker.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TaskTrackerContext(
                serviceProvider.GetRequiredService<DbContextOptions<TaskTrackerContext>>()))
            {
                // Look for any existing data.
                if (context.Category.Any() || context.Task.Any() || context.User.Any())
                {
                    return;   // DB has been seeded
                }
                context.Priority.AddRange(
                    new Priority { priorityName = "Low" },
                    new Priority { priorityName = "Medium" },
                    new Priority { priorityName = "High" },
                    new Priority { priorityName = "Urgent" }
                );

                context.Status.AddRange(
                    new Status { statusName = "Pending" },
                    new Status { statusName = "In-Progress" },
                    new Status { statusName = "Complete" },
                    new Status { statusName = "On Hold" }, 
                    new Status { statusName = "Cancelled" }, 
                    new Status { statusName = "Deferred" } 
                );

                // Seed Categories
                context.Category.AddRange(
                    new Category { categoryName = "Work" },
                    new Category { categoryName = "Personal" },
                    new Category { categoryName = "Health" }, 
                    new Category { categoryName = "Education" }, 
                    new Category { categoryName = "Finance" }, 
                    new Category { categoryName = "Home" } 
                );
// Add more categories as needed

                // Seed Users
                context.User.AddRange(
                    new User
                    {
                        firstName = "John",
                        lastName = "Doe",
                        Email = "john.doe@example.com",
                        Password = "[hashed_password]"
                    }
                );
                context.SaveChanges();
                // Seed Tasks
                context.Task.AddRange(
                    new TaskTracker.Models.Task
                    {
                        user = context.User.FirstOrDefault(u => u.firstName == "John" && u.lastName == "Doe"),
                        Title = "Finish Project Proposal",
                        Description = "Draft and finalize the project proposal for upcoming meeting.",
                        StartDate = DateTime.Parse("2024-03-26"),
                        DueDate = DateTime.Parse("2024-04-02"),
                        Category = "Work",
                        Priority = "High",
                        Status = "In Progress",
                        Location = "Office"
                    },
                    new TaskTracker.Models.Task
                    {
                        user = context.User.FirstOrDefault(u => u.firstName == "John" && u.lastName == "Doe"),
                        Title = "Grocery Shopping",
                        Description = "Buy groceries for the week.",
                        StartDate = DateTime.Parse("2024-03-27"),
                        DueDate = DateTime.Parse("2024-03-27"),
                        Category = "Personal",
                        Priority = "Medium",
                        Status = "Pending",
                        Location = "Supermarket"
                    },
                    new TaskTracker.Models.Task
                    {
                        user = context.User.FirstOrDefault(u => u.firstName == "John" && u.lastName == "Doe"),
                        Title = "Finish Report",
                        Description = "Complete the quarterly report for the finance department.",
                        StartDate = DateTime.Parse("2024-03-26"),
                        DueDate = DateTime.Parse("2024-03-30"),
                        Category = "Work",
                        Priority = "High",
                        Status = "In-Progress",
                        Location = "Office"
                    },
                    new TaskTracker.Models.Task
                    {
                        user = context.User.FirstOrDefault(u => u.firstName == "John" && u.lastName == "Doe"),
                        Title = "Gym Workout",
                        Description = "Go to the gym for a workout session.",
                        StartDate = DateTime.Parse("2024-03-28"),
                        DueDate = DateTime.Parse("2024-03-28"),
                        Category = "Personal",
                        Priority = "Low",
                        Status = "Pending",
                        Location = "Fitness Center"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
