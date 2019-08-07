using SalaryCalc.Dal;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Auth
{
    public class RolesAuth : ActionFilterAttribute
    {
       private readonly DataContext db = new DataContext();

      
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            User user = filterContext.HttpContext.Session["LoggedUser"] as User;

            List<UserRoleDto> userRoleDtos = db.UserRoles.Where(w => w.PostionId == user.PostionId).Select(s => new UserRoleDto
                {
                    Name = s.Role.Name,
                    RoleId = s.RoleId,
                    Action = s.Role.Action,
                    Controller = s.Role.Controller,
                    Area = s.Role.Area
                }).ToList();
            
          
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
              || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }

           
            bool IsDenied = false;

            if (filterContext.RouteData.Values["Area"] != null)
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                string area = filterContext.RouteData.Values["Area"].ToString();

                IsDenied = true;
                //User login edende  Session["LoggedUser"] id -sini veririk
                // User user = db.Users.Find(2);
              
                if (user != null)
                {
                  
                    foreach (var userrule in userRoleDtos)
                    {
                        if (userrule.Controller.Trim().ToLower() == controller.ToLower() && userrule.Action.Trim().ToLower() == action.ToLower() && userrule.Area.ToLower() == area.ToLower())
                        {
                            IsDenied = false;
                        }
                    }
                }
            }
            else
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();


                IsDenied = true;
                //User user = db.Users.Find(2);
                //User user = filterContext.HttpContext.Session["LoggedUser"] as User;
                if (user != null)
                {
                    foreach (var userrule in userRoleDtos)
                    {
                        if (userrule.Controller.Trim().ToLower() == controller.ToLower() && userrule.Action.Trim().ToLower() == action.ToLower())
                        {
                            IsDenied = false;
                        }
                    }
                }
            }

            if (IsDenied)
            {
                //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Error403" }));
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}