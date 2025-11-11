using Lab4.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


//This page was mostly done manually by referene other pages and coursework

//Issue on validation solved with help from copilot
//prompt:I ahve things mostly going now, but when a duplicate record is found it the dropdown menu of all options and doesnt let you pick new ones



namespace Lab4.Pages.AcademicRecordManagement
{
    public class CreateModel : PageModel
    {
        private readonly Lab4.DataAccess.StudentRecordContext _context;

        public CreateModel(Lab4.DataAccess.StudentRecordContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "DisplayText");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "DisplayText");
            return Page();
        }

        [BindProperty]
        public AcademicRecord AcademicRecord { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            AcademicRecord.CourseCodeNavigation = await _context.Courses.FirstOrDefaultAsync(c => c.Code == AcademicRecord.CourseCode)!;
            AcademicRecord.Student = await _context.Students .FirstOrDefaultAsync(s => s.Id == AcademicRecord.StudentId);

            AcademicRecord ac = _context.AcademicRecords
                                                .FirstOrDefault(a => a.CourseCode == AcademicRecord.CourseCode && a.StudentId == AcademicRecord.StudentId);

            if (ac == null)
            {
                _context.AcademicRecords.Add(AcademicRecord);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
           
            ModelState.AddModelError("AcademicRecord.Grade", $"That Record Already Exists!");

            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "DisplayText", AcademicRecord.CourseCode);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "DisplayText", AcademicRecord.StudentId);

            return Page();


        }
    }
}
