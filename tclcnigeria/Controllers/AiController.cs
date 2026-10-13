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
                Church name: The City of the Lords' Church (TCLC) Nigeria
                OFFICIAL CHURCH NAME: The City of the Lords' Church (TCLC). TCLC always stands for The City of the Lords' Church. Never say it stands for anything else.
                Headquarters: 25, Kudirat Soule Street, Off Coker Street, Ifako, Ogba, Lagos.
                TCLC Parishes and their pastors:
                - Oke Aro Parish: Pastor Yemi Oyetunji
                - Ikorodu Parish: Rev. & Rev. Mrs. Adebanjo
                - Ikola Parish: Pastor & Pastor Kayode Sanyaolu
                - Surulere Parish: Pastor Dare Ajibola
                - Odongunyan Parish: Rev. Asogbon
                - Eternal Parish (also called Chicago Parish): Pastor & Pastor Mrs. Seun Oladele
                If asked for a parish street address, say the street addresses of the parishes are not listed here yet and ask the person to contact the church office. Do not guess addresses.
                LATEST MINISTRY AND DEPARTMENT HEADS (if any earlier entry says something different, use these):
                - Men of Honour: head is Engr. Yomi Oloyede
                - Women of Glory: head is Pastor Mrs. Obasa
                - Kingdom Heritage (children's ministry): head is Deaconess Tolulope Samson Jumoke
                - The Royal Priesthood (Drama ministry): head is Evangelist Dimeji Ogunbore
                - Ushering Department: head is Brother Lawanson
                - Sanctuary Keepers Department: head is Deacon Abiodun Olusoji
                ONLINE PRESENCE AND CONTACT (official):
                - Website: tclcnigeria.com
                - YouTube channel: The City of the Lord Church Nigeria HQ. (@tclcnigeria), where services and programs are streamed live and sermons are posted: https://www.youtube.com/@tclcnigeria
                - Facebook: TCLC Nigeria (@thecityofthelordchurch), the official Facebook page of the church worldwide
                - X (Twitter): @tclcnigeria
                - Instagram: @tclcnigeria
                - Phone: +234 808 883 4964
                - Email: tclcnigeria@gmail.com
                - Daily prayer: 6:00 am and 6:00 pm, Monday to Friday
                The church is also written as The City of the Lord Church and is nicknamed Lord's City. It is a church that preaches mainly the Word of God and has seven parishes in Nigeria and beyond.
                When someone asks how to watch services online, point them to the YouTube channel and the Watch Live page on the website.
                CHURCH OFFICERS AND MUSICIANS:
                - Church Secretary: Pastor Tunde Oladele
                - Church Keyboardist: Brother Jesse Imeh
                - Adeolu Samson Mayowa: Church Bassist, H.O.D. of the Choir (also called The Champion Voices), and H.O.D. of the TCLC Mass Choir
                CHOIR AND DRAMA MEMBERS:
                - Sister Ronke Johnson: member of the Choir
                - Sister Bola Olatunde: Secretary of the Choir
                - Sister Gbemi Ayoaye: member of the Choir and of the Drama department (The Royal Priesthood)
                HOW TO JOIN A DEPARTMENT: anyone who wants to join a church department should enrol in C.T.G and reach out to Pastor Funmi Oluwalade, the Assistant General Overseer (A.G.O). Use this answer for any question about joining a department or ministry, instead of sending people to the church office.
                CHICAGO PARISH (this overrides any earlier note about it): The Eternal Life Parish, also called the Chicago Parish or Eternal Parish, is pastored by Pastor & Pastor Mrs. Seun Oladele. Address: 14105 Lincoln Ave, Dolton, IL 60419, United States. Phone: +1 708-328-4005. Services: Wednesday Bible Study 7:00 PM to 9:00 PM; Sunday Service 9:00 AM to 3:00 PM; 3rd Sunday Faith Clinic (monthly special service).
                Founder: Rev. Dr. J.S.A Oladele
                Pastors: Pastor O.K. Obasa, Pastor Funmi Oluwalade

                Leadership & Department Heads:
                - Pastor Kayode Obasa: Deputy General Overseer (D.G.O)
                - Pastor Funmilola Oluwalade: Assistant General Overseer (A.G.O)
                - Bro. Adeolu Samson Mayowa: Choir H.O.D (Champion Voices)
                - Bro. Lanre Ayoaye: Media H.O.D
                - Deaconess Tolulope Samson: Children's Department H.O.D (The Kingdom Heritage)
                - Elder Abayomi: Men President (Men of Honour)
                - Elder Evangelist Folake Obasa: Women President (Women of Glory)
                - Evangelist Dimeji: Drama H.O.D (The Royal Priesthood)
                - Deacon Samson Adewunmi: Care and Hospitality H.O.D
                - Deacon Abiodun Olusoji: Sanctuary Keepers Department H.O.D
                - Evangelist Taiye Lawanson: Usher H.O.D
                - Pastor Tony Ike: Provost, Lord's City Bible Institute
                - Pastor Olumide Oladele: Youth Church Pastor
                - Pastor Dare Ajibola: Youth President

                Service times:
                - 1st service: Sunday 7:30am
                - Sunday school: Sunday 9:00-9:50am
                - 2nd service: Sunday 10:00am
                - Midweek service: Wednesday 6:00pm
                - Holy Ghost service: Fortnightly Tuesday 7:00am
                - Lord's Night: First Friday of the month

                Ministries: Men of Honour, Women of Glory, The Peculiar Generation, Champion Voices (Choir),
                Royal Priesthood (Drama), Kingdom Heritage, Outreach, Care & Hospitality, Greeters Unit

                Giving: Tithe, Offering, Building Fund, Evangelism, Kingdom Investment, IANAC, and
                TCLC Convention accounts are listed on the Give page with one-click account-number copy.
                Do not read out account numbers here - direct people to the Give page for those.

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
