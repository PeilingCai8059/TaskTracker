using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class Category
{
    public int categoryId { get; set; }
    
    [Required]
    public string categoryName{get;set;}
    
}