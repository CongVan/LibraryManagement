using LibraryManagement.Models;
using LibraryManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LibraryManagement.Filter
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public bool isAdmin { get; set; } = false;
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentContext.IsLogged() == false)
            {
                filterContext.Result = new RedirectResult("~/User/Login");
                return;
            }
            var roleUser = CurrentContext.GetRole();
            if (isAdmin && roleUser.RoleName!="Admin")
            {
                filterContext.Result = new RedirectResult("~/User/AccessDenied");
            }
            if (roleUser.RoleName == "Admin")
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            var url = filterContext.HttpContext.Request.RawUrl.Split('/').ElementAt(1);
            
            using (var ctx=new LibraryManagementEntities())
            {
                var menu = (from mr in ctx.Menu_Role
                            join m in ctx.Menus on mr.MenuID equals m.ID
                            join r in ctx.Roles on mr.RoleID equals r.RoleValue
                            where String.IsNullOrEmpty(url)==false && m.Path.Contains(url) 
                            && m.Flag == 1 && mr.Flag == 1 && roleUser.RoleValue == r.RoleValue
                            orderby m.Priority
                            select m).FirstOrDefault();
                if (menu==null)
                {
                    var menuApprove = (from m in ctx.Menus
                                join mr in ctx.Menu_Role on m.ID equals mr.MenuID
                                join r in ctx.Roles on mr.RoleID equals r.RoleValue
                                where mr.Flag == 1 && m.Flag == 1 &&  r.RoleValue== roleUser.RoleValue
                                orderby m.Priority
                                select m).FirstOrDefault();
                    filterContext.Result = new RedirectResult(menuApprove.Path);
                    //var router = new RouteValueDictionary(new
                    //{
                    //    action = "AccessDenied",
                    //    controller = "User",
                    //    next = menuApprove==null?"":menuApprove.Path
                    //});
                    //filterContext.Result = new RedirectToRouteResult(router);
                }
            }
            base.OnActionExecuting(filterContext);


        }
    }
}