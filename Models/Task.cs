using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models;

public class Task
{
    public int Id { get; set; }

    public string? UserId { get; set; } 

    public AppUser? User { get; set; } 

    [StringLength(40, MinimumLength = 3)]
    [Required]
    public string Title { get; set; }
    public string? Description { get; set; }
    [Display(Name = "Parent Task")]
    public int? ParentTaskId { get; set; } 
    public Task? ParentTask { get; set; }
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
    [ValidateDueDate]
    public DateTime DueDate { get; set; }

    public string? ReminderInterval { get; set; }  

    [Display(Name = "Reminder")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? ReminderTime { get; set; }

    public String? Category  { get; set; }
    
    public String? Priority  { get; set; }
    
    public String? Status  { get; set; }

    public String? Location  { get; set; }

    public ICollection<User>? Collaborators {get;set;} 
     [Required]
     public bool IsRecurring { get; set; } // Assuming it's a boolean for Yes/No
        
        public string? Frequency { get; set; } // Can be null if IsRecurring is false
}

public class ValidateDueDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var task = (Task)validationContext.ObjectInstance;

        if (task.DueDate < task.StartDate)
        {
            return new ValidationResult("Due Date cannot be before the Start Date.");
        }

        return ValidationResult.Success;
    }
}