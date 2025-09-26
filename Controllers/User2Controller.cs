using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ServiceRequestForm.Data;
using ServiceRequestForm.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ServiceRequestForm.Controllers
{
    public class User2Controller : BaseController
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

        public User2Controller(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            if (HttpContext.Session.GetString("Role") != "Validator")
                return RedirectToAction("Login", "Account");

            var forms = _context.ServiceRequests
                .Where(f => f.Status == "Sent To Validator")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                forms = forms.Where(f => EF.Functions.Like(f.RequestTitle, $"%{search}%"));
            }

            var result = forms.OrderBy(f => f.Id).ToList();

            ViewBag.Entities = EntityDepartmentMap;
            ViewBag.CostCentres = CostCentreGlAccountMap;
            ViewBag.PersonnelTypes = new[] { "Expatriate", "Local" };
            ViewBag.Locations = new[] { "LOS", "ABJ", "PHC", "OFF" };
            ViewBag.ServiceSchemes = new[] { "Rotational", "Residential" };
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

            return View(result);
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
        public IActionResult UpdateForm(ServiceRequest model)
        {
            var existingForm = _context.ServiceRequests.FirstOrDefault(f => f.Id == model.Id);
            if (existingForm != null)
            {
                existingForm.RequestTitle = model.RequestTitle;
                existingForm.Entity = model.Entity;
                existingForm.Department = model.Department;
                existingForm.NumberOfPersonnel = model.NumberOfPersonnel;
                existingForm.ServiceDescription = model.ServiceDescription;
                existingForm.ProposedServiceStartDate = model.ProposedServiceStartDate;
                existingForm.PersonnelType = model.PersonnelType;
                existingForm.Location = model.Location;
                existingForm.ServiceScheme = model.ServiceScheme;
                existingForm.User1Comment = model.User1Comment;
                existingForm.CostCentre = model.CostCentre;
                existingForm.GlAccount = model.GlAccount;
                existingForm.BudgetOwner = model.BudgetOwner;
                existingForm.JobImpact = model.JobImpact;
                existingForm.User2Comment = model.User2Comment;

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SendToUser3(int id)
        {
            var form = _context.ServiceRequests.FirstOrDefault(f => f.Id == id);
            if (form != null && form.Status == "Sent To Validator")
            {
                form.Status = "Sent To Approver";
                _context.SaveChanges();
                TempData["Notification"] = "Successfully Sent";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ReturnToUser1(int id)
        {
            var form = _context.ServiceRequests.FirstOrDefault(f => f.Id == id);
            if (form != null && form.Status == "Sent To Validator")
            {
                form.Status = "Returned To User";
                _context.SaveChanges();
                TempData["Notification"] = "Successfully Returned";
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












