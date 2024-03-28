using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class User
{
    public int userId { get; set; }
    
    [Required]
    public string firstName{get;set;}
    [Required]
    public string lastName{get;set;}

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}