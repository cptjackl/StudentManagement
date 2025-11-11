using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4.DataAccess;


//


namespace Lab4.Pages.AcademicRecordManagement
{
    public class EditModel : PageModel
    {
        private readonly Lab4.DataAccess.StudentRecordContext _context;

        public EditModel(Lab4.DataAccess.StudentRecordContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AcademicRecord AcademicRecord { get; set; } = default!;

        // Updated to accept the composite key used throughout the app
        public async Task<IActionResult> OnGetAsync(string? sortOrder, string? action, string? courseCode, string? studentId)
        {
            if (courseCode == null || studentId == null)
            {
                return NotFound();
            }

            var academicrecord =  await _context.AcademicRecords
                                                .Include(a => a.CourseCodeNavigation)
                                                .Include(a => a.Student)
                                                .FirstOrDefaultAsync(m => m.CourseCode == courseCode && m.StudentId == studentId);
            if (academicrecord == null)
            {
                return NotFound();
            }
            AcademicRecord = academicrecord;
            
            return Page();
        }

       
        public async Task<IActionResult> OnPostAsync()
        {
                AcademicRecord ac = _context.AcademicRecords
                                                .FirstOrDefault(a => a.CourseCode == AcademicRecord.CourseCode && a.StudentId == AcademicRecord.StudentId);

                if (ac == null) 
                {
                    return NotFound();
                }

                ac.Grade = AcademicRecord.Grade;

                _context.SaveChanges();

            return RedirectToPage("./Index");
        }

        private bool AcademicRecordExists(string courseCode, string studentId)
        {
            return _context.AcademicRecords.Any(e => e.CourseCode == courseCode && e.StudentId == studentId);
        }
    }
}
