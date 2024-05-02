using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskTrackerContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public TaskController(TaskTrackerContext context,SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ILogger<AccountController> logger)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger =logger ;
        }

        // GET: Task
        public async Task<IActionResult> Index(string taskCategory, string taskPriority, string taskStatus, string searchString)
        {
            var currentUser = await _userManager.GetUserAsync(User);
           
            IQueryable<string> categoryQuery = from m in _context.Category
                                    where m.UserId == currentUser.Id
                                    orderby m.categoryName
                                    select m.categoryName;
            IQueryable<string> priorityQuery = from m in _context.Priority
                                    orderby m.priorityName
                                    select m.priorityName;
            IQueryable<string> statusQuery = from m in _context.Status
                                    orderby m.statusName
                                    select m.statusName;

            var tasks = from t in _context.Task
                where t.UserId == currentUser.Id || t.SharedWithUsers.Any(u => u == currentUser.Id)
                select t;
            
            Console.WriteLine($"Tasks received are");
            foreach (var task in tasks)
            {
                Console.WriteLine($"Received task from DB: {Newtonsoft.Json.JsonConvert.SerializeObject(task)}");
            }

            
            if (!String.IsNullOrEmpty(searchString)){
                tasks = tasks.Where( s => 
                    s.Title.ToLower()!.Contains(searchString.ToLower())
                    ||s.Description.ToLower()!.Contains(searchString.ToLower())) ;
            }
    
            if (!string.IsNullOrEmpty(taskCategory))
            {
                tasks = tasks.Where(x => x.Category == taskCategory);
            }
             if (!string.IsNullOrEmpty(taskPriority))
            {
                tasks = tasks.Where(x => x.Priority == taskPriority);
            }
            if (!string.IsNullOrEmpty(taskStatus))
            {
                tasks = tasks.Where(x => x.Status == taskStatus);
            }

            var sortedTasks = new List<Models.Task>();
            var parentTasks = tasks.Where(t => t.ParentTaskId == null)
                               .OrderBy(t => t.StartDate)
                               .ThenBy(t => t.DueDate)
                               .ThenBy(t => t.Title )
                               .ThenBy(t => t.Category )
                               .ThenBy(t => t.Priority );
        
            foreach (var parent in parentTasks)
            {
                sortedTasks.Add(parent);
                var subtasks =  tasks.Where(t => t.ParentTaskId == parent.Id)
                               .OrderBy(t => t.StartDate)
                               .ThenBy(t => t.DueDate)
                               .ThenBy(t => t.Title )
                               .ThenBy(t => t.Category )
                               .ThenBy(t => t.Priority );
                foreach (var subTask in subtasks){
                    sortedTasks.Add(subTask);
                }
            }  

            var independentSubtasks = tasks.Where(t => t.ParentTaskId != null && !parentTasks.Any(p => p.Id == t.ParentTaskId))
             .OrderBy(t => t.StartDate)
            .ThenBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ThenBy(t => t.Category)
            .ThenBy(t => t.Priority);

            sortedTasks.AddRange(independentSubtasks);  

            var taskCategoryVM = new TaskCategoryViewModel{
                Categories = new SelectList(await categoryQuery.Distinct().ToListAsync()),
                Priorities = new SelectList(await priorityQuery.Distinct().ToListAsync()),
                Status = new SelectList(await statusQuery.Distinct().ToListAsync()),
                Tasks = sortedTasks
            };
            return View(taskCategoryVM);
        }


        public async Task<IActionResult> GanttChart(string taskCategory, string taskPriority, string taskStatus, string searchString)
        {
            var currentUser = await _userManager.GetUserAsync(User);
           
            IQueryable<string> categoryQuery = from m in _context.Category
                                    where m.UserId == currentUser.Id
                                    orderby m.categoryName
                                    select m.categoryName;
            IQueryable<string> priorityQuery = from m in _context.Priority
                                    orderby m.priorityName
                                    select m.priorityName;
            IQueryable<string> statusQuery = from m in _context.Status
                                    orderby m.statusName
                                    select m.statusName;

            var tasks = from t in _context.Task
                        where t.UserId == currentUser.Id
                        select t ;
            
            if (!String.IsNullOrEmpty(searchString)){
                tasks = tasks.Where( s => 
                    s.Title.ToLower()!.Contains(searchString.ToLower())
                    ||s.Description.ToLower()!.Contains(searchString.ToLower())) ;
            }

            if (!string.IsNullOrEmpty(taskCategory))
            {
                tasks = tasks.Where(x => x.Category == taskCategory);
            }
             if (!string.IsNullOrEmpty(taskPriority))
            {
                tasks = tasks.Where(x => x.Priority == taskPriority);
            }
            if (!string.IsNullOrEmpty(taskStatus))
            {
                tasks = tasks.Where(x => x.Status == taskStatus);
            }
              var sortedTasks = new List<Models.Task>();
            var parentTasks = tasks.Where(t => t.ParentTaskId == null)
                               .OrderBy(t => t.StartDate)
                               .ThenBy(t => t.DueDate)
                               .ThenBy(t => t.Title )
                               .ThenBy(t => t.Category )
                               .ThenBy(t => t.Priority );
        
            foreach (var parent in parentTasks)
            {
                sortedTasks.Add(parent);
                var subtasks =  tasks.Where(t => t.ParentTaskId == parent.Id)
                               .OrderBy(t => t.StartDate)
                               .ThenBy(t => t.DueDate)
                               .ThenBy(t => t.Title )
                               .ThenBy(t => t.Category )
                               .ThenBy(t => t.Priority );
                foreach (var subTask in subtasks){
                    sortedTasks.Add(subTask);
                }
            }  

            var independentSubtasks = tasks.Where(t => t.ParentTaskId != null && !parentTasks.Any(p => p.Id == t.ParentTaskId))
             .OrderBy(t => t.StartDate)
            .ThenBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ThenBy(t => t.Category)
            .ThenBy(t => t.Priority);

            sortedTasks.AddRange(independentSubtasks); 
            
            var taskCategoryVM = new TaskCategoryViewModel{
                Categories = new SelectList(await categoryQuery.Distinct().ToListAsync()),
                Priorities = new SelectList(await priorityQuery.Distinct().ToListAsync()),
                Status = new SelectList(await statusQuery.Distinct().ToListAsync()),
                Tasks = sortedTasks
            };
            return View(taskCategoryVM);
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            LoadInfoAsync();
            return View(task);
        }

        // GET: Task/Create
        public async Task<IActionResult> Create()
        {
            var model = new Models.Task();
            
            LoadInfoAsync();
            var users = await _userManager.Users.ToListAsync();
            var usersSelectList = new List<SelectListItem>();
            foreach (var user in users)
            {
                usersSelectList.Add(new SelectListItem
                {
                    Value = user.Id.ToString(), // Assuming Id is the unique identifier for users
                    Text = user.FirstName + ' ' + user.LastName // Assuming Name is the property representing the user's name
                });
            }

            ViewBag.Users = usersSelectList;
            return View(model);
        }

        public  async Task<IActionResult>  CreateSubtask (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var MainTask = new List<Models.Task>();
            var task = await _context.Task.FindAsync(id);
            MainTask.Add(task);
            ViewBag.MainTask = new SelectList(MainTask, "Id","Title");
            ViewBag.MainTasCategory = new SelectList(MainTask, "Category","Category");
            LoadInfoAsync();
            return View();
        }

        private async void LoadInfoAsync()
        {
            var currentUser = await  _userManager.GetUserAsync(User);
            var categories = _context.Category.Where(t => t.UserId == currentUser.Id).ToList();
            var priorities = _context.Priority.ToList();
            var status = _context.Status.ToList();
            var parentTask  = _context.Task.Where(t => t.UserId == currentUser.Id && t.ParentTask == null ).ToList();
           
            ViewBag.Categories = new SelectList(categories,"categoryName", "categoryName");
            ViewBag.Priorities = new SelectList(priorities,"priorityName", "priorityName");
            ViewBag.Status = new SelectList(status,"statusName", "statusName");
            ViewBag.ParentTask = new SelectList(parentTask, "Id","Title");
        }
        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ParentTaskId,StartDate,DueDate,ReminderInterval,Category,Priority,Status,Location,UserId,IsRecurring,Frequency, IsGroupTask, SharedWithUsers")]Models.Task task)
        {
            Console.WriteLine("task.StartDate");
            Console.WriteLine(task.StartDate);
            Console.WriteLine(task.Title);
            //Console.WriteLine(task.);
            Console.WriteLine($"Received task: {Newtonsoft.Json.JsonConvert.SerializeObject(task)}");

            var currentUser = await _userManager.GetUserAsync(User);

            if (task.ParentTaskId != null)
            {
                task.ParentTask = await _context.Task.FindAsync(task.ParentTaskId);
            }
            if (ModelState.IsValid)
            { 
                task.UserId = currentUser.Id ;
                Console.WriteLine($"Task reminder interval is {task.ReminderInterval}");
                task.ReminderTime = CalculateReminderDateTime(task.ReminderInterval, task.DueDate);
                Console.WriteLine($"Task reminder time is {task.ReminderTime.ToString()}");
                _context.Add(task);
                if(!task.IsRecurring)
                {
                    task.Frequency = null;
                }
                else
                {
                    switch (task.Frequency)
                    {
                        case "Daily":
                            CreateRecurringTasks(task, TimeSpan.FromDays(1));
                            break;
                        case "Weekdays":
                            CreateRecurringTasks(task, TimeSpan.FromDays(1), onlyWeekdays: true);
                            break;
                        case "Weekends":
                            CreateRecurringTasks(task, TimeSpan.FromDays(1), onlyWeekends: true);
                            break;
                        case "Weekly":
                            CreateRecurringTasks(task, TimeSpan.FromDays(7));
                            break;
                        case "Biweekly":
                            CreateRecurringTasks(task, TimeSpan.FromDays(14));
                            break;
                        case "Monthly":
                            CreateRecurringTasks(task, null, monthly: true);
                            break;
                    }
                }
                await _context.SaveChangesAsync();
                if(task.ParentTask != null ){
                    task.ParentTask.subTasks ??= new List<Models.Task>(); 
                    task.ParentTask.subTasks.Add(task); 
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }else{
                 foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        var key = modelStateKey;
                        var errorMessage = error.ErrorMessage;
                        Console.WriteLine($"Error in {key}: {errorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }

            return View(task);
        }

        private void CreateRecurringTasks(Models.Task baseTask, TimeSpan? interval, bool onlyWeekdays = false, bool onlyWeekends = false, bool monthly = false, int occurrences = 30)
        {
            var tasks = new List<Models.Task>();
            var nextDate = baseTask.StartDate;

            for (int i = 0; i < occurrences; i++)
            {
                Console.WriteLine($"Creating Task #{i + 1} for date {nextDate}"); 
                var newTask = CloneTask(baseTask);
                newTask.StartDate = nextDate;
                newTask.DueDate = nextDate + (baseTask.DueDate - baseTask.StartDate);
                tasks.Add(newTask);

                // Determine the next date based on the recurrence type
                if (monthly)
                {
                    nextDate = nextDate.AddMonths(1); // Increment by one month
                }
                else
                {
                    do
                    {
                        nextDate = nextDate.Add(interval.GetValueOrDefault()); // Increment by the interval
                    }
                    while (
                        (onlyWeekdays && (nextDate.DayOfWeek == DayOfWeek.Saturday || nextDate.DayOfWeek == DayOfWeek.Sunday)) || 
                        (onlyWeekends && (nextDate.DayOfWeek != DayOfWeek.Saturday && nextDate.DayOfWeek != DayOfWeek.Sunday))
                    );
                }
            }

            _context.Task.AddRange(tasks); // Ensure that the context's DbSet name matches your actual DbSet
            _context.SaveChanges();
        }


        private Models.Task CloneTask(Models.Task taskToClone)
        {
            return new Models.Task
            {
                Title = taskToClone.Title,
                Description = taskToClone.Description,
                ParentTaskId = taskToClone.ParentTaskId,
                StartDate = taskToClone.StartDate,
                DueDate = taskToClone.DueDate,
                ReminderTime = taskToClone.ReminderTime,
                Category = taskToClone.Category,
                Priority = taskToClone.Priority,
                Status = taskToClone.Status,
                Location = taskToClone.Location,
                UserId = taskToClone.UserId,
                IsGroupTask = taskToClone.IsGroupTask,
                SharedWithUsers = taskToClone.SharedWithUsers
                // IsRecurring and Frequency are not copied since each occurrence is an independent task
            };
        }

        private DateTime? CalculateReminderDateTime(string reminderInterval, DateTime dueDate)
        {
            if (string.IsNullOrEmpty(reminderInterval))
                return null;

            TimeSpan timeSpan;
            switch (reminderInterval)
            {
                case "5 minutes":
                    timeSpan = TimeSpan.FromMinutes(5);
                    break;
                case "15 minutes":
                    timeSpan = TimeSpan.FromMinutes(15);
                    break;
                case "30 minutes":
                    timeSpan = TimeSpan.FromMinutes(30);
                    break;
                case "1 hour":
                    timeSpan = TimeSpan.FromHours(1);
                    break;
                case "12 hours":
                    timeSpan = TimeSpan.FromHours(12);
                    break;
                case "1 day":
                    timeSpan = TimeSpan.FromDays(1);
                    break;
                default:
                    return null;
            }

            return dueDate - timeSpan;
        }


        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            LoadInfoAsync();

            _logger.LogInformation($"Task data: {task.ReminderTime}");
            return View(task);
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ParentTaskId,StartDate,DueDate,ReminderTime,Category,Priority,Status,Location,UserId")] Models.Task task)
        {
            
            if (id != task.Id)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);
            task.UserId = currentUser.Id ;
            if (task.ParentTaskId != null)
            {
                task.ParentTask = await _context.Task.FindAsync(task.ParentTaskId);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(task);
        }

        // GET: Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
        {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Task.FindAsync(id);
            if (task != null )
            {   
                _context.Task.Remove(task);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Task.Any(e => e.Id == id);
        }

         public async Task<IActionResult> Calendar(string taskCategory, string taskPriority, string taskStatus, string searchString)
        {
            var currentUser = await _userManager.GetUserAsync(User);
           
            IQueryable<string> categoryQuery = from m in _context.Category
                                    where m.UserId == currentUser.Id
                                    orderby m.categoryName
                                    select m.categoryName;
            IQueryable<string> priorityQuery = from m in _context.Priority
                                    orderby m.priorityName
                                    select m.priorityName;
            IQueryable<string> statusQuery = from m in _context.Status
                                    orderby m.statusName
                                    select m.statusName;

            var tasks = from t in _context.Task
                        where t.UserId == currentUser.Id
                        select t ;
            
            if (!String.IsNullOrEmpty(searchString)){
                tasks = tasks.Where( s => 
                    s.Title.ToLower()!.Contains(searchString.ToLower())
                    ||s.Description.ToLower()!.Contains(searchString.ToLower())) ;
            }

            if (!string.IsNullOrEmpty(taskCategory))
            {
                tasks = tasks.Where(x => x.Category == taskCategory);
            }
             if (!string.IsNullOrEmpty(taskPriority))
            {
                tasks = tasks.Where(x => x.Priority == taskPriority);
            }
            if (!string.IsNullOrEmpty(taskStatus))
            {
                tasks = tasks.Where(x => x.Status == taskStatus);
            }
              var sortedTasks = new List<Models.Task>();
            var parentTasks = tasks.Where(t => t.ParentTaskId == null)
                               .OrderBy(t => t.StartDate)
                               .ThenBy(t => t.DueDate)
                               .ThenBy(t => t.Title )
                               .ThenBy(t => t.Category )
                               .ThenBy(t => t.Priority );
        
            foreach (var parent in parentTasks)
            {
                sortedTasks.Add(parent);
                var subtasks =  tasks.Where(t => t.ParentTaskId == parent.Id)
                               .OrderBy(t => t.StartDate)
                               .ThenBy(t => t.DueDate)
                               .ThenBy(t => t.Title )
                               .ThenBy(t => t.Category )
                               .ThenBy(t => t.Priority );
                foreach (var subTask in subtasks){
                    sortedTasks.Add(subTask);
                }
            }  

            var independentSubtasks = tasks.Where(t => t.ParentTaskId != null && !parentTasks.Any(p => p.Id == t.ParentTaskId))
             .OrderBy(t => t.StartDate)
            .ThenBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ThenBy(t => t.Category)
            .ThenBy(t => t.Priority);

            sortedTasks.AddRange(independentSubtasks); 

            ViewBag.ParentTask = new SelectList(parentTasks, "Id","Title");

            var taskCategoryVM = new TaskCategoryViewModel{
                Categories = new SelectList(await categoryQuery.Distinct().ToListAsync()),
                Priorities = new SelectList(await priorityQuery.Distinct().ToListAsync()),
                Status = new SelectList(await statusQuery.Distinct().ToListAsync()),
                Tasks = sortedTasks,
            };
            return View(taskCategoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> CalendarCreate([FromBody]Models.Task task)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {   
                if (task.ParentTaskId != null)
                {
                    task.ParentTask = await _context.Task.FindAsync(task.ParentTaskId);
                }
                task.UserId = currentUser.Id ;
                _context.Add(task);
                await _context.SaveChangesAsync();
                if(task.ParentTask != null ){
                    task.ParentTask.subTasks ??= new List<Models.Task>(); 
                    task.ParentTask.subTasks.Add(task); 
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Calendar));
            }else{
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Error in {entry.Key}: {entry.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()}");
                    }
                }
                return Json(new { success = false, message = "Invalid data", errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()) });
            }
        }
    }
}
