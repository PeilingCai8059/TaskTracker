using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class Status
{
    public int statusId { get; set; }
    public string statusName { get; set; }
    // Add more properties if needed
}