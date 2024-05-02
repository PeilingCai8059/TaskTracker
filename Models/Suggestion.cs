using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class Suggestion
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)] // Adjust the length as needed
    public string Location { get; set; }

    [Required]
    [StringLength(50)] // Adjust the length as needed
    public string Category { get; set; }

    [StringLength(100)] // Adjust the length as needed
    public string? Place { get; set; }

    [StringLength(100)] // Adjust the length as needed
    public string? Address { get; set; }
}