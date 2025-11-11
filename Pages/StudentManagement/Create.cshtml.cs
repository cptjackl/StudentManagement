using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Lab4.DataAccess;
using Microsoft.EntityFrameworkCore;

// Validation handled by coplilot
//prompt: now on StudentManagement/Create I want to add a check that stops the creation of a new record if the input id already exists in the database, to avoid an error. It should check the database and if there already is a record there is should not attempt to creat the record, and output a validation message to the page instead...
//        ok great, I just want the attempted ID in the error message now


namespace Lab4.Pages.StudentManagement
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
            return Page();
        }

        [BindProperty]
        public Student Student { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Prevent creating a student when the Id already exists in the database.
            // If a record exists, add a validation error and redisplay the page.
            if (await _context.Students.AnyAsync(s => s.Id == Student.Id))
            {
                ModelState.AddModelError("Student.Id", $"A student with Id '{Student.Id}' already exists.");
                return Page();
            }

            _context.Students.Add(Student);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
