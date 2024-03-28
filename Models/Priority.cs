using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;


public class Priority
{
    public int priorityId { get; set; }
    public string priorityName { get; set; }
    // Add more properties if needed
}