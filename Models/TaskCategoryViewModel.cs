using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TaskTracker.Models;

public class TaskCategoryViewModel
{
    public List<Task>? Tasks { get; set; }
    public SelectList? Categories { get; set; }
    public SelectList? Priorities { get; set; }
    public SelectList? Status { get; set; }
    
    public string? TaskPriority { get; set; }
    public string? TaskStatus { get; set; }

    public string? TaskCategory { get; set; }

    public string? SearchString { get; set; }
    public bool DoSearchAndFilter { get; set; }
}