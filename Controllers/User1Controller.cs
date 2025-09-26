using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using ServiceRequestForm.Data;
using ServiceRequestForm.Models;
using System.Linq;
using System.Collections.Generic;

namespace ServiceRequestForm.Controllers
{
    public class User1Controller : BaseController
    {
        private readonly ApplicationDbContext _context;

    
        private static readonly Dictionary<string, string[]> EntityDepartmentMap = new()
        {
            { "Science", new[] { "Physics", "Computer Science", "Chemistry", "Soil Science", "Bio Science" } },
            { "Engineering", new[] { "Mechanical", "Electrical", "Software" } },
            { "Medical Science", new[] { "Medicine", "Dentistry", "Surgery", "Pharmacy" } },
            { "Social Management", new[] { "Economics", "Social Science", "Sports" } },
            { "Arts", new[] { "Government", "Literature" } }
        };

        private static readonly Dictionary<string, string[]> CostCentreGlAccountMap = new()
        {
            { "CONS 007", new[] { "007 1001", "007 1002", "007 1003" } },
            { "DONS 711", new[] { "711 004", "711 007", "711 009" } },
            { "ABJ 801", new[] { "801 001", "801 002", "801 003" } }
        };

        public User1Controller(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Requester")
            return RedirectToAction("Login", "Account");

        var username = HttpContext.Session.GetString("Username");
        var draftForm = _context.ServiceRequests
            .FirstOrDefault(f => f.RequestedBy == username && 
                                 (f.Status == "Draft" || f.Status == "Returned To User"));

            ViewBag.Entities = EntityDepartmentMap;
            ViewBag.CostCentres = CostCentreGlAccountMap;
            ViewBag.PersonnelTypes = new[] { "Expatriate", "Local" };
            ViewBag.Locations = new[] { "LOS", "ABJ", "PHC", "OFF" };
            ViewBag.ServiceSchemes = new[] { "Rotational", "Residential" };
            ViewBag.Error = TempData["Error"];
            ViewBag.JobImpacts = new[] 
            { 
                "High impact", 
                "Strategic importance", 
                "High consequence of failure", 
                "Sensitivity", 
                "Business continuity", 
                "High visibility", 
                "Specialized skills" 
            };

            return View(draftForm);
        }

        [HttpGet]
        public JsonResult GetDepartments(string entity)
        {
            if (string.IsNullOrEmpty(entity) || !EntityDepartmentMap.ContainsKey(entity))
                return Json(new string[0]);

            return Json(EntityDepartmentMap[entity]);
        }

        [HttpGet]
        public JsonResult GetGlAccounts(string costCentre)
        {
            if (string.IsNullOrEmpty(costCentre) || !CostCentreGlAccountMap.ContainsKey(costCentre))
                return Json(new string[0]);

            return Json(CostCentreGlAccountMap[costCentre]);
        }

        [HttpPost]
        public IActionResult CreateForm()
        {
          var username = HttpContext.Session.GetString("Username");
        var inProgressForm = _context.ServiceRequests
            .Any(f => f.RequestedBy == username && f.Status != "Approved");

        if (inProgressForm)
        {
            TempData["Notification"] = "You cannot request a new form until the current one is approved.";
            return RedirectToAction("Index");
        }

        var newForm = new ServiceRequest
        {
            Status = "Draft",
            RequestedBy = username
        };
        _context.ServiceRequests.Add(newForm);
        _context.SaveChanges();

        return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SendForm(ServiceRequest form)
        {
           var username = HttpContext.Session.GetString("Username");
        var existingForm = _context.ServiceRequests
            .FirstOrDefault(f => f.Id == form.Id && f.RequestedBy == username);

        if (existingForm != null && 
            (existingForm.Status == "Draft" || existingForm.Status == "Returned To User"))
        {
                existingForm.RequestTitle = form.RequestTitle;
                existingForm.Entity = form.Entity;
                existingForm.Department = form.Department;
                existingForm.NumberOfPersonnel = form.NumberOfPersonnel;
                existingForm.ServiceDescription = form.ServiceDescription;
                existingForm.Status = "Sent To Validator";
                existingForm.ProposedServiceStartDate = form.ProposedServiceStartDate;
                existingForm.PersonnelType = form.PersonnelType;
                existingForm.Location = form.Location;
                existingForm.ServiceScheme = form.ServiceScheme;
                existingForm.User1Comment = form.User1Comment;
                existingForm.CostCentre = form.CostCentre;
                existingForm.GlAccount = form.GlAccount;
                existingForm.BudgetOwner = form.BudgetOwner;
                existingForm.JobImpact = form.JobImpact;

                _context.SaveChanges();
                TempData["Notification"] = "Successfully Sent";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Grid(string search)
        {
            var username = HttpContext.Session.GetString("Username");

            var requests = _context.ServiceRequests
                .Where(r => r.RequestedBy == username)
                .AsQueryable();

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






