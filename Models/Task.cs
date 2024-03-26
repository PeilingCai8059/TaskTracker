using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class Task
{
    public int Id { get; set; }

    [Required]
    public User user { get; set; }

    [StringLength(20, MinimumLength = 3)]
    [Required]
    public string Title { get; set; }
    public string? Description { get; set; }
    public Task? ParentTask {get;set;}
    public ICollection<Task>? subTasks {get;set;}
    
    [Required] 
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; }

    [Required]
    [Display(Name = "Due Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [Display(Name = "Reminder")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? ReminderTime { get; set; }

    [Required]
    public String Category  { get; set; }
    [Required]
    public String Priority  { get; set; }
    [Required]
    public String Status  { get; set; }

    public String? location  { get; set; }

    [Required]
    public ICollection<User>? Collaborators {get;set;} 
}