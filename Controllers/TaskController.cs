using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskTrackerContext _context;

        public TaskController(TaskTrackerContext context)
        {
            _context = context;
        }

        // GET: Task
        public async Task<IActionResult> Index(string taskCategory, string taskPriority, string taskStatus, string searchString)
        {
            IQueryable<string> categoryQuery = from m in _context.Category
                                    orderby m.categoryName
                                    select m.categoryName;
            IQueryable<string> priorityQuery = from m in _context.Priority
                                    orderby m.priorityName
                                    select m.priorityName;
            IQueryable<string> statusQuery = from m in _context.Status
                                    orderby m.statusName
                                    select m.statusName;
            var tasks = from t in _context.Task
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

            var taskCategoryVM = new TaskCategoryViewModel{
                Categories = new SelectList(await categoryQuery.Distinct().ToListAsync()),
                Priorities = new SelectList(await priorityQuery.Distinct().ToListAsync()),
                Status = new SelectList(await statusQuery.Distinct().ToListAsync()),
                Tasks = await tasks.ToListAsync()
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

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            LoadInfo();
            return View();
        }
        private void LoadInfo()
        {
            var categories = _context.Category.ToList();
            var priorities = _context.Priority.ToList();
            var status = _context.Status.ToList();
            var parentTask  = _context.Task.Where(t => t.ParentTask == null).ToList();
           
            ViewBag.Categories = new SelectList(categories,"categoryName", "categoryName");
            ViewBag.Priorities = new SelectList(priorities,"priorityName", "priorityName");
            ViewBag.Status = new SelectList(status,"statusName", "statusName");
            ViewBag.ParentTask = new SelectList(parentTask, "Id","Title");
        }
        // POST: Task/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ParentTaskId,StartDate,DueDate,ReminderTime,Category,Priority,Status,Location")]Models.Task task)
        {
            if (task.ParentTaskId != null)
            {
                task.ParentTask = await _context.Task.FindAsync(task.ParentTaskId);
            }
            if (ModelState.IsValid)
            { 
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
            LoadInfo();
            return View(task);
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ParentTaskId,StartDate,DueDate,ReminderTime,Category,Priority,Status,Location")] Models.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }
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
