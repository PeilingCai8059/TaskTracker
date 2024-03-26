using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    public string firstName{get;set;}
    [Required]
    public string lasName{get;set;}

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}