using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Required for [Authorize]
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    [Authorize] // Only logged-in users can view or delete messages
    public class ContactAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ContactAdmin
        // Shows all messages, newest first
        public async Task<IActionResult> Index()
        {
            var messages = await _context.ContactMessages
                .OrderByDescending(m => m.DateSent)
                .ToListAsync();
            return View(messages);
        }

        // GET: ContactAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contactForm = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contactForm == null) return NotFound();

            // PRO LOGIC: Once the Pastor opens the message, mark it as read automatically
            if (!contactForm.IsRead)
            {
                contactForm.IsRead = true;
                _context.Update(contactForm);
                await _context.SaveChangesAsync();
            }

            return View(contactForm);
        }

        // GET: ContactAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contactForm = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contactForm == null) return NotFound();

            return View(contactForm);
        }

        // POST: ContactAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contactForm = await _context.ContactMessages.FindAsync(id);
            if (contactForm != null)
            {
                _context.ContactMessages.Remove(contactForm);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactFormExists(int id)
        {
            return _context.ContactMessages.Any(e => e.Id == id);
        }
    }
}