using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaskTracker.Data;
using TaskTracker.Models;
using Task = System.Threading.Tasks.Task;

public class EmailSenderService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly TaskTrackerContext _context;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        // Code for sending emails goes here
        // Replace this with your email sending logic
        SendEmail(_context);
        Console.WriteLine("Sending email at: " + DateTime.Now);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    static void SendEmail(TaskTrackerContext _context)
    {
        // Sender's email address and password
        string senderEmail = "your_email@example.com";
        string password = "your_password";

        // Recipient's email address
        string recipientEmail = "recipient@example.com";

        // Create an instance of the SmtpClient class
        SmtpClient smtpClient = new SmtpClient("smtp.example.com")
        {
            Port = 587, // SMTP port (587 for TLS, 465 for SSL)
            Credentials = new NetworkCredential(senderEmail, password),
            EnableSsl = true // Enable SSL/TLS
        };
        var pendingTasks = EmailSenderService.GetPendingTasks(_context,  "Pending");

        foreach (var task in pendingTasks)
        {
            // Access properties of the task as needed
            Console.WriteLine($"Task: {task.Title}, Due Date: {task.DueDate}");
        }
       
        // Create a MailMessage object
        MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail)
        {
            Subject = "Test Email",
            Body = "This is a test email sent from a .NET application."
        };

        try
        {
            // Send the email
            smtpClient.Send(mailMessage);
            Console.WriteLine("Email sent successfully at: " + DateTime.Now);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send email. Error message: " + ex.Message);
        }
    }

    private static List<TaskTracker.Models.Task> GetPendingTasks(TaskTrackerContext _context, string? taskStatus)
    {

        // Calculate the time 15 minutes from now
        DateTime fifteenMinutesFromNow = DateTime.Now.AddMinutes(15);

        var tasks = from t in _context.Task
                    where t.Status == taskStatus &&
                        t.ReminderTime != null &&
                        t.ReminderTime >= DateTime.Now &&
                        t.ReminderTime <= fifteenMinutesFromNow
                    select t;


        var sortedTasks = new List<TaskTracker.Models.Task>();
        var parentTasks = tasks.Where(t => t.ParentTaskId == null)
                           .OrderBy(t => t.StartDate)
                           .ThenBy(t => t.DueDate)
                           .ThenBy(t => t.Title)
                           .ThenBy(t => t.Category)
                           .ThenBy(t => t.Priority);

        foreach (var parent in parentTasks)
        {
            sortedTasks.Add(parent);
            var subtasks = tasks.Where(t => t.ParentTaskId == parent.Id)
                           .OrderBy(t => t.StartDate)
                           .ThenBy(t => t.DueDate)
                           .ThenBy(t => t.Title)
                           .ThenBy(t => t.Category)
                           .ThenBy(t => t.Priority);
            foreach (var subTask in subtasks)
            {
                sortedTasks.Add(subTask);
            }
        }

        var independentSubtasks = tasks.Where(t => t.ParentTaskId != null && !parentTasks.Any(p => p.Id == t.ParentTaskId))
                            .OrderBy(t => t.StartDate)
                            .ThenBy(t => t.DueDate)
                            .ThenBy(t => t.Title)
                            .ThenBy(t => t.Category)
                            .ThenBy(t => t.Priority);

        sortedTasks.AddRange(independentSubtasks);

        return sortedTasks;

    }

    System.Threading.Tasks.Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    System.Threading.Tasks.Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
