using System;
using System.Collections.Generic;
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
        public IActionResult Create()
        {
            LoadInfoAsync();
            return View();
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
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ParentTaskId,StartDate,DueDate,ReminderTime,Category,Priority,Status,Location,UserId")]Models.Task task)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (task.ParentTaskId != null)
            {
                task.ParentTask = await _context.Task.FindAsync(task.ParentTaskId);
            }
            if (ModelState.IsValid)
            { 
                task.UserId = currentUser.Id ;
                 _context.Add(task);
                await _context.SaveChangesAsync();
                if(task.ParentTask != null ){
                    task.ParentTask.subTasks ??= new List<Models.Task>(); 
                    task.ParentTask.subTasks.Add(task); 
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }

            return View(task);
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
    }
}
