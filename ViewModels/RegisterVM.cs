using System.ComponentModel.DataAnnotations;

namespace TaskTracker.ViewModels;

public class RegisterVM
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
    
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords don't match.")]
    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }

}