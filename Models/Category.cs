using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class Category
{
    public int Id { get; set; }
    
    [Required]
    public string name{get;set;}
    
}