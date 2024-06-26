using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class AppUser : IdentityUser
{
    [StringLength(100)]
    [MaxLength(100)]
    [Required]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [StringLength(100)]
    [MaxLength(100)]
    [Required]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    
}