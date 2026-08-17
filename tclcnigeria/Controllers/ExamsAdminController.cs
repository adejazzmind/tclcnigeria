using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    [Authorize(Roles = "CTGStaff")]
    public class ExamsAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ExamsAdminController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var exams = await _context.Exams
                .Include(e => e.Questions)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
            return View(exams);
        }

        [HttpGet]
        public IActionResult Create() => View(new Exam());

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create(Exam model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.CreatedAt = DateTime.UtcNow;
            model.CreatedBy = User.Identity?.Name;
            model.IsPublished = false;

            _context.Exams.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Questions), new { examId = model.Id });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                exam.IsPublished = !exam.IsPublished;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Questions(int examId)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions.OrderBy(q => q.Order))
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null) return NotFound();

            return View(exam);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddQuestion(int examId, string questionText, string optionA, string optionB, string? optionC, string? optionD, string correctOption)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam == null) return NotFound();

            var order = await _context.ExamQuestions.CountAsync(q => q.ExamId == examId);

            var question = new ExamQuestion
            {
                ExamId = examId,
                QuestionText = questionText,
                OptionA = optionA,
                OptionB = optionB,
                OptionC = string.IsNullOrWhiteSpace(optionC) ? null : optionC,
                OptionD = string.IsNullOrWhiteSpace(optionD) ? null : optionD,
                CorrectOption = correctOption.ToUpper(),
                Order = order
            };

            _context.ExamQuestions.Add(question);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Questions), new { examId });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteQuestion(int id, int examId)
        {
            var question = await _context.ExamQuestions.FindAsync(id);
            if (question != null)
            {
                _context.ExamQuestions.Remove(question);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Questions), new { examId });
        }
    }
}
