using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    public class ExamsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ExamsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var exams = await _context.Exams
                .Where(e => e.IsPublished)
                .OrderBy(e => e.Title)
                .ToListAsync();
            return View(exams);
        }

        [HttpGet]
        public async Task<IActionResult> Take(int id)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id && e.IsPublished);
            if (exam == null) return NotFound();

            ViewBag.ExamTitle = exam.Title;
            ViewBag.ExamId = exam.Id;
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Take(int id, string fullName, string email)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions.OrderBy(q => q.Order))
                .FirstOrDefaultAsync(e => e.Id == id && e.IsPublished);

            if (exam == null) return NotFound();

            ViewBag.ExamTitle = exam.Title;
            ViewBag.ExamId = exam.Id;

            var approved = await _context.CtgEnrollments
                .AnyAsync(c => c.Email.ToLower() == email.ToLower() && c.Status == "Approved");

            if (!approved)
            {
                ViewBag.Error = "We couldn't verify your approved Workforce enrollment with this email. Please make sure you've been approved before attempting this exam.";
                return View();
            }

            var priorAttempt = await _context.ExamAttempts
                .Where(a => a.ExamId == id && a.Email.ToLower() == email.ToLower())
                .OrderByDescending(a => a.SubmittedAt)
                .FirstOrDefaultAsync();

            if (priorAttempt != null && !priorAttempt.RetakeApproved)
            {
                ViewBag.Error = "You have already taken this exam. If you need a retake, please contact the Workforce team.";
                return View();
            }

            ViewBag.FullName = fullName;
            ViewBag.Email = email;
            return View("Quiz", exam);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Submit(int examId, string fullName, string email)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null) return NotFound();

            var approved = await _context.CtgEnrollments
                .AnyAsync(c => c.Email.ToLower() == email.ToLower() && c.Status == "Approved");

            if (!approved) return Forbid();

            int score = 0;
            foreach (var q in exam.Questions)
            {
                var submitted = Request.Form["answer_" + q.Id];
                if (!string.IsNullOrEmpty(submitted) && submitted.ToString().ToUpper() == q.CorrectOption)
                {
                    score++;
                }
            }

            int total = exam.Questions.Count;
            int percent = total > 0 ? (int)Math.Round((double)score / total * 100) : 0;
            bool passed = percent >= exam.PassingScorePercent;

            var attempt = new ExamAttempt
            {
                ExamId = examId,
                FullName = fullName,
                Email = email,
                Score = score,
                TotalQuestions = total,
                PercentScore = percent,
                Passed = passed,
                SubmittedAt = DateTime.UtcNow,
                RetakeApproved = false
            };

            _context.ExamAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            ViewBag.ExamTitle = exam.Title;
            ViewBag.Score = score;
            ViewBag.Total = total;
            ViewBag.Percent = percent;
            ViewBag.Passed = passed;
            ViewBag.PassingScore = exam.PassingScorePercent;

            return View("Result");
        }
    }
}
