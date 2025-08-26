using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using ServiceRequestForm.Data;
using ServiceRequestForm.Models;
using System.Linq;

namespace ServiceRequestForm.Controllers
{
    public class User3Controller : BaseController
    {
        private readonly ApplicationDbContext _context;

        public User3Controller(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

public IActionResult Index()
{
    if (HttpContext.Session.GetString("Role") != "Approver")
        return RedirectToAction("Login", "Account");

    var formsToApprove = _context.ServiceRequests
        .Where(f => f.Status == "Sent To Approver")
        .OrderBy(f => f.Id)
        .ToList();

    return View(formsToApprove); 
}

        [HttpPost]
        public IActionResult ApproveForm(int id)
        {
            var form = _context.ServiceRequests.FirstOrDefault(f => f.Id == id);
        if (form != null && form.Status == "Sent To Approver")
        {
            form.Status = "Approved";
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
        }

        public IActionResult Grid(string search)
        {
            var requests = _context.ServiceRequests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                requests = requests.Where(r =>
                    EF.Functions.Like(r.RequestTitle, $"%{search}%") ||
                    EF.Functions.Like(r.Status, $"%{search}%"));
            }

            return View("Grid", requests.ToList());
        }


    }
}


