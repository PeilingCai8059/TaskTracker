using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TaskTracker.Models;

public class TaskInfoViewModel
{
    public Task Task { get; set; }
    public SelectList? Categories { get; set; }
    public SelectList? Priority { get; set; }
    public SelectList? Status { get; set; }

}