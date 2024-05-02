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
    public class SuggestionController : Controller
    {
        private readonly TaskTrackerContext _context;

        public SuggestionController(TaskTrackerContext context)
        {
            _context = context;
        }

        // GET: Suggestion
        public async Task<IActionResult> Index()
        {
            return View(await _context.Suggestion.ToListAsync());
        }

        // GET: Suggestion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suggestion = await _context.Suggestion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suggestion == null)
            {
                return NotFound();
            }

            return View(suggestion);
        }

        // GET: Suggestion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suggestion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Location,Category,Place,Address")] Suggestion suggestion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(suggestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suggestion);
        }

        // GET: Suggestion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suggestion = await _context.Suggestion.FindAsync(id);
            if (suggestion == null)
            {
                return NotFound();
            }
            return View(suggestion);
        }

        // POST: Suggestion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Location,Category,Place,Address")] Suggestion suggestion)
        {
            if (id != suggestion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suggestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuggestionExists(suggestion.Id))
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
            return View(suggestion);
        }

        // GET: Suggestion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suggestion = await _context.Suggestion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suggestion == null)
            {
                return NotFound();
            }

            return View(suggestion);
        }

        // POST: Suggestion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suggestion = await _context.Suggestion.FindAsync(id);
            _context.Suggestion.Remove(suggestion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuggestionExists(int id)
        {
            return _context.Suggestion.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetSuggestedPlaces(string location, string category)
        {
            Console.WriteLine("GetSuggestedPlaces called");
            Console.WriteLine($"Location {location} Category {category}");
            IQueryable<Suggestion> query = _context.Suggestion;

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(s => s.Location == location);
            }
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(s => s.Category == category);
            }

            List<Suggestion> suggestions = await query.ToListAsync();
            
            if (suggestions == null || suggestions.Count == 0)
            {
                return NotFound("No suggestions found.");
            }

            return Ok(suggestions);
        }
    }
}