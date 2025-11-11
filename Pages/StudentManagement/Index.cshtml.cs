using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab4.DataAccess;


//Table sorting handled by code in Index.cshtml.cs and Index.cshtml
//prompt: First how can I make the column heads of the table in StudentManagement/Index.cshtml clickable so that they sort the table by their column

//Delete functionality handled by copilot in Index.cshtml.cs and Index.cshtml
//prompt: Now, how can I have the page prompt the user with a dialog box in the browser to confirm that they want to to delete a student record instead of opening another page for it...
//        delete the record by specifying an action to be used in OnGetAsync...
//        Add logic to the OnGetAsync function to delete the appropriate academic records before deleteing the student record


namespace Lab4.Pages.StudentManagement
{

    public class IndexModel : PageModel
    {
        private readonly Lab4.DataAccess.StudentRecordContext _context;

        public IndexModel(Lab4.DataAccess.StudentRecordContext context)
        {
            _context = context;
        }

        public IList<Student> Student { get; set; } = default!;

        // current sort and the toggles for each column
        public string CurrentSort { get; set; } = "";
        public string NameSortParm { get; set; } = "";
        public string CoursesSortParm { get; set; } = "";
        public string AvgSortParm { get; set; } = "";

        // Accept the sortOrder query string and apply ordering
        // Also accept an unconventional "action=delete&id=..." GET request to delete a record
        public async Task<IActionResult> OnGetAsync(string? sortOrder, string? action, string? id)
        {
            // If teacher used GET to perform delete, handle it first and redirect to avoid repeat on refresh
            if (!string.IsNullOrEmpty(action) && action.Equals("delete", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest();
                }

                // Delete any AcademicRecord rows that reference this student first
                var childRecords = await _context.AcademicRecords
                                                 .Where(ar => ar.StudentId == id)
                                                 .ToListAsync();

                if (childRecords.Count > 0)
                {
                    _context.AcademicRecords.RemoveRange(childRecords);
                }

                var studentToDelete = await _context.Students.FindAsync(id);
                if (studentToDelete == null)
                {
                    return NotFound();
                }

                _context.Students.Remove(studentToDelete);
                await _context.SaveChangesAsync();

                // Redirect back to index page (no query) to prevent repeated delete on reload
                return RedirectToPage();
            }

            CurrentSort = sortOrder ?? "";

            // toggle values - clicking a column header toggles between ascending and descending
            NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            CoursesSortParm = sortOrder == "courses" ? "courses_desc" : "courses";
            AvgSortParm = sortOrder == "avg" ? "avg_desc" : "avg";

            // load students including related AcademicRecords (needed for computed properties)
            var students = await _context.Students
                                         .Include(s => s.AcademicRecords)
                                         .ToListAsync();

            // Perform in-memory ordering because NumberOfCourses and AvgGrad are computed properties
            Student = sortOrder switch
            {
                "name_desc" => students.OrderByDescending(s => s.Name).ToList(),
                "courses" => students.OrderBy(s => s.NumberOfCourses).ToList(),
                "courses_desc" => students.OrderByDescending(s => s.NumberOfCourses).ToList(),
                "avg" => students.OrderBy(s => s.AvgGrad).ToList(),
                "avg_desc" => students.OrderByDescending(s => s.AvgGrad).ToList(),
                _ => students.OrderBy(s => s.Name).ToList(),
            };

            return Page();
        }
    }
}
