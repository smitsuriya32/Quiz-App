using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exam_Management.Filter
{
    public class LoginFilter : IAuthorizationFilter
    {
        // Implemented Method
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!IsAnonymousUser(filterContext.ActionDescriptor))
            {
                // Accessing the Session
                if (HttpContext.Current.Session["Id"] == null)
                {
                    // Store the Data or Messages in TempData
                    filterContext.Controller.TempData["AlertMsg"] = "Unauthorized access";

                    // Redirect to login page
                    filterContext.Result = new RedirectResult("/Registration/Login");
                }
            }
        }

        private bool IsAnonymousUser(ActionDescriptor actionDescriptor)
        {
            var controllerName = actionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = actionDescriptor.ActionName;

            // Check if the action is in the Home controller and is allowed to be accessed anonymously
            if (controllerName.Equals("Registration", StringComparison.OrdinalIgnoreCase))
            {
                var anonymousActions = new[] { "Login", "Create" }; // Add actions in the Home controller that don't require authentication
                return Array.Exists(anonymousActions, a => a.Equals(actionName, StringComparison.OrdinalIgnoreCase));
            }

            // For actions in other controllers, return false to require authentication
            return false;
        }
    }
}