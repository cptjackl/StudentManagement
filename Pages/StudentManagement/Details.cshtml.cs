using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab4.DataAccess;


//Table generation and sorting handled by copilot
//prompt: Now on StudentManagement/Details I want a table of the a academic records associated with the student. The table should have clickable headers that sort the table just like StudentManagement/Index



namespace Lab4.Pages.StudentManagement
{
    public class DetailsModel : PageModel
    {
        private readonly Lab4.DataAccess.StudentRecordContext _context;

        public DetailsModel(Lab4.DataAccess.StudentRecordContext context)
        {
            _context = context;
        }

        public Student Student { get; set; } = default!;

        // Academic records for the view (sorted)
        public IList<AcademicRecord> Records { get; set; } = default!;

        // Sorting support for the records table
        public string CurrentSort { get; set; } = "";
        public string CourseSortParm { get; set; } = "";
        public string GradeSortParm { get; set; } = "";

        // Accept sortOrder so headers can sort the records table
        public async Task<IActionResult> OnGetAsync(string id, string sortOrder)
        {
            if (id == null)
            {
                return NotFound();
            }

            // include related academic records and course navigation so we can display course title
            var student = await _context.Students
                                        .Include(s => s.AcademicRecords)
                                            .ThenInclude(ar => ar.CourseCodeNavigation)
                                        .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            Student = student;

            // prepare sort toggles
            CurrentSort = sortOrder ?? "";
            CourseSortParm = sortOrder == "course" ? "course_desc" : "course";
            GradeSortParm = sortOrder == "grade" ? "grade_desc" : "grade";

            // materialize records and sort in-memory (Grade is computed/stored as nullable int)
            var records = Student.AcademicRecords?.ToList() ?? new List<AcademicRecord>();

            Records = sortOrder switch
            {
                "course" => records.OrderBy(r => (r.CourseCodeNavigation?.Title ?? r.CourseCode)).ToList(),
                "course_desc" => records.OrderByDescending(r => (r.CourseCodeNavigation?.Title ?? r.CourseCode)).ToList(),
                "grade" => records.OrderBy(r => r.Grade).ToList(),
                "grade_desc" => records.OrderByDescending(r => r.Grade).ToList(),
                _ => records.OrderBy(r => (r.CourseCodeNavigation?.Title ?? r.CourseCode)).ToList(),
            };

            return Page();
        }
    }
}
