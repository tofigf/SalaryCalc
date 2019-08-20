using DataAccessLayer;
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
            //AllowAnonymous
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
              || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }

            User user = filterContext.HttpContext.Session["LoggedUser"] as User;
            bool IsDenied = true;

            List<UserRoleDto> userRoleDtos = db.UserRoles.Where(w => w.PostionId == user.PostionId).Select(s => new UserRoleDto
                {
                    Name = s.Role.Name,
                    RoleId = s.RoleId,
                    Action = s.Role.Action,
                    Controller = s.Role.Controller
                }).ToList();
          
            string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();

                //User user = db.Users.Find(2);
                if (user != null)
                {
                    foreach (UserRoleDto userrule in userRoleDtos)
                    {
                        if (userrule.Controller.Trim().ToLower() == controller.ToLower() && userrule.Action.Trim().ToLower() == action.ToLower())
                        {
                            IsDenied = false;
                        }
                    }
                }

            if (IsDenied)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}