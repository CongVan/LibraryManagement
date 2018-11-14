using LibraryManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagement.Filter
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public int isAdmin { get; set; } = 1;
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentContext.IsLogged() == false)
            {
                filterContext.Result = new RedirectResult("~/User/Login");
                return;
            }
            //if (CurrentContext.GetCurUser().RoleID != 1)
            //{
            //    filterContext.Result = new HttpUnauthorizedResult();
            //}
            base.OnActionExecuting(filterContext);
        }
    }
}