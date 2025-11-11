using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab4.DataAccess;

//Table sorting and delete handling done by copilot
//prompt: next on AcademicRecordMangement/Index I want the same sort of table sorting and delete functionality from the studentmangement/Index page...
//        ok I just want the name of the student and the course name included in the conformation dialog box...
//        can you split the message into muliple lines so the name and title are seperate?


namespace Lab4.Pages.AcademicRecordManagement
{
    public class IndexModel : PageModel
    {
        private readonly Lab4.DataAccess.StudentRecordContext _context;

        public IndexModel(Lab4.DataAccess.StudentRecordContext context)
        {
            _context = context;
        }

        public IList<AcademicRecord> AcademicRecord { get;set; } = default!;

        // Sorting support (for clickable headers)
        public string CurrentSort { get; set; } = "";
        public string CourseSortParm { get; set; } = "";
        public string StudentSortParm { get; set; } = "";
        public string GradeSortParm { get; set; } = "";

        // Accept sortOrder and also accept an unconventional GET-based delete (action=delete)
        public async Task<IActionResult> OnGetAsync(string? sortOrder, string? action, string? courseCode, string? studentId)
        {
            // Handle delete via GET as requested in StudentManagement index (confirm is on client side)
            if (!string.IsNullOrEmpty(action) && action.Equals("delete", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(courseCode) || string.IsNullOrEmpty(studentId))
                {
                    return BadRequest();
                }

                var recordToDelete = await _context.AcademicRecords
                                                   .FirstOrDefaultAsync(ar => ar.CourseCode == courseCode && ar.StudentId == studentId);
                if (recordToDelete == null)
                {
                    return NotFound();
                }

                _context.AcademicRecords.Remove(recordToDelete);
                await _context.SaveChangesAsync();

                // Redirect to avoid repeated deletes on refresh
                return RedirectToPage();
            }

            CurrentSort = sortOrder ?? "";

            // toggle sort tokens
            CourseSortParm = sortOrder == "course" ? "course_desc" : "course";
            StudentSortParm = sortOrder == "student" ? "student_desc" : "student";
            GradeSortParm = sortOrder == "grade" ? "grade_desc" : "grade";

            // load records including navigation for display
            var records = await _context.AcademicRecords
                                        .Include(a => a.CourseCodeNavigation)
                                        .Include(a => a.Student)
                                        .ToListAsync();

            // sort in-memory (safe because some fields use computed/navigation values)
            AcademicRecord = sortOrder switch
            {
                "course" => records.OrderBy(r => r.CourseCodeNavigation?.DisplayText).ToList(),
                "course_desc" => records.OrderByDescending(r => r.CourseCodeNavigation?.DisplayText).ToList(),
                "student" => records.OrderBy(r => r.Student?.DisplayText).ToList(),
                "student_desc" => records.OrderByDescending(r => r.Student?.DisplayText).ToList(),
                "grade" => records.OrderBy(r => r.Grade).ToList(),
                "grade_desc" => records.OrderByDescending(r => r.Grade).ToList(),
                _ => records.OrderBy(r => r.CourseCodeNavigation?.DisplayText).ToList(),
            };

            return Page();
        }
    }
}
