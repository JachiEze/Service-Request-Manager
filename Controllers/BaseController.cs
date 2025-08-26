using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;  
using ServiceRequestForm.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ServiceRequestForm.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

protected bool IsValidSession()
{
    var username = HttpContext.Session.GetString("Username");
    var sessionId = HttpContext.Session.Id;

    if (string.IsNullOrEmpty(username))
        return false;

    var user = _context.UserAccounts.FirstOrDefault(u => u.Username == username);
    if (user == null)
        return false;

    var now = DateTime.UtcNow;

    if (user.ActiveSessionId == sessionId &&
        user.LastActivity.HasValue &&
        user.LastActivity.Value > now.AddMinutes(-15))
    {
        
        user.LastActivity = now;
        _context.SaveChanges();
        return true;
    }

    
    if (user.ActiveSessionId == sessionId)
    {
        user.ActiveSessionId = null;
        user.LastActivity = null;
        _context.SaveChanges();
    }

    return false;
}

        public override void OnActionExecuting(ActionExecutingContext context) 
        {
            if (!IsValidSession() && !(context.Controller is AccountController))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }

            base.OnActionExecuting(context);
        }
    }
}


