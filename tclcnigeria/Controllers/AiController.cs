using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;
using tclcnigeria.Services;

namespace tclcnigeria.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [IgnoreAntiforgeryToken]
    public class AiController : ControllerBase
    {
        private readonly IAiService _ai;
        private readonly ApplicationDbContext _db;

        public AiController(IAiService ai, ApplicationDbContext db)
        {
            _ai = ai;
            _db = db;
        }

        public class AskRequest
        {
            public string Question { get; set; } = string.Empty;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest(new { answer = "Please type a question." });

            var context = await BuildChurchContextAsync();
            var answer = await _ai.AskAsync(request.Question.Trim(), context);

            return Ok(new { answer });
        }

        private async Task<string> BuildChurchContextAsync()
        {
            var upcomingEvents = await _db.ChurchEvents
                .Where(e => e.EventDate >= DateTime.UtcNow)
                .OrderBy(e => e.EventDate)
                .Take(8)
                .Select(e => $"- {e.Title} on {e.EventDate:MMM dd, yyyy HH:mm} at {e.Location} ({e.Category})")
                .ToListAsync();

            var recentSermons = await _db.Sermons
                .OrderByDescending(s => s.DatePreached)
                .Take(5)
                .Select(s => $"- \"{s.Title}\" by {s.Speaker}, preached {s.DatePreached:MMM dd, yyyy}")
                .ToListAsync();

            var staticInfo = """
                Church name: The Church of the Living Christ (TCLC) Nigeria
                Founder: Rev. Dr. J.S.A Oladele
                Pastors: Pastor O.K. Obasa, Pastor Funmi Oluwalade

                Service times:
                - 1st service: Sunday 7:30am
                - Sunday school: Sunday 9:00-9:50am
                - 2nd service: Sunday 10:00am
                - Midweek service: Wednesday 6:00pm
                - Holy Ghost service: Fortnightly Tuesday 7:00am
                - Lord's Night: First Friday of the month

                Ministries: Men of Honour, Women of Glory, Youth Rising, Champion Voices (Choir),
                Royal Priesthood (Drama), Kingdom Heritage, Outreach, Care & Hospitality, Greeters Unit

                Giving: Tithe, Offering, Building Fund, Evangelism, Kingdom Investment, IANAC, and
                TCLC Convention accounts are listed on the Give page with one-click account-number copy.
                Do not read out account numbers here — direct people to the Give page for those.

                Prayer requests: submitted via the Prayer page; staff follow up directly.
                """;

            var eventsBlock = upcomingEvents.Count > 0
                ? "Upcoming events:\n" + string.Join("\n", upcomingEvents)
                : "Upcoming events: none currently scheduled.";

            var sermonsBlock = recentSermons.Count > 0
                ? "Recent sermons:\n" + string.Join("\n", recentSermons)
                : "Recent sermons: none listed yet.";

            return $"{staticInfo}\n\n{eventsBlock}\n\n{sermonsBlock}";
        }
    }
}
