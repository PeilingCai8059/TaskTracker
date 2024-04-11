using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class Category
{
    public int categoryId { get; set; }
    
    [Required]
    [Display(Name = "Category Name")]
    public string categoryName{get;set;}

    public string? UserId { get; set; } 

    public AppUser? User { get; set; } 
    
}