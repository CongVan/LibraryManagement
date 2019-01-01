using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagement.Utility
{
    public class CurrentContext
    {
        public static bool IsLogged()
        {
            var flag = HttpContext.Current.Session["isLogin"];
            if (flag == null || (int)flag == 0)
            {
                if (HttpContext.Current.Request.Cookies["userID"] != null)
                {

                    int id = Convert.ToInt32(HttpContext.Current.Request.Cookies["userID"].Value);
                    using (var ctx = new LibraryManagementEntities())
                    {
                        var user = ctx.Users.Where(u => u.ID == id).FirstOrDefault();
                        HttpContext.Current.Session["isLogin"] = 1;
                        HttpContext.Current.Session["User"] = user;

                    }
                    return true;

                }

                return false;


            }
            return true;
        }
        public static User GetCurUser()
        {
            return (User)HttpContext.Current.Session["User"];
        }
        public static Role GetRole()
        {
            var rol = new Role();
            using (var ctx=new LibraryManagementEntities())
            {
                if (HttpContext.Current.Request.Cookies["userID"] != null)
                {
                    int id = Convert.ToInt32(HttpContext.Current.Request.Cookies["userID"].Value);
                    var user = ctx.Users.Where(u => u.ID == id).FirstOrDefault();
                    rol = ctx.Roles.Where(c => c.RoleValue == user.RoleID).FirstOrDefault();
                }
            }
            return rol;
        }

        public static void Detroy()
        {
            HttpContext.Current.Session["isLogin"] = 0;
            HttpContext.Current.Session["User"] = null;
            //HttpCookie login = new HttpCookie("userID");
            //login.Expires = DateTime.Now.AddDays(-7d);


            HttpContext.Current.Response.Cookies["userID"].Expires = DateTime.Now.AddDays(-1);


        }
        public static string GetUname(int idUser)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                return ctx.Users.Where(c => c.ID == idUser).FirstOrDefault().UserName;

            }
        }
        public static List<Menu> getMenus()
        {
            var lst = new List<Menu>();
            using (var ctx=new LibraryManagementEntities())
            {
                if (HttpContext.Current.Request.Cookies["userID"] != null)
                {
                    int id = Convert.ToInt32(HttpContext.Current.Request.Cookies["userID"].Value);
                    var user = ctx.Users.Where(u => u.ID == id).FirstOrDefault();
                    if (user.RoleID != 3)
                    {
                        lst = (from m in ctx.Menus
                               join mr in ctx.Menu_Role on m.ID equals mr.MenuID
                               join r in ctx.Roles on mr.RoleID equals r.RoleValue
                               where r.RoleValue == user.RoleID && m.ParentID==-1 && m.Flag==1 && mr.Flag==1
                               orderby m.Priority
                               select m).ToList();
                    }
                    else
                    {
                        lst= ctx.Menus.Where(c => c.Flag == 1 && c.ParentID == -1).ToList();
                    }
                }
                    
            }
            return lst;
        }
        public static List<Menu> getMenusChild(int id)
        {
            var lst = new List<Menu>();
            using (var ctx = new LibraryManagementEntities())
            {
                if (HttpContext.Current.Request.Cookies["userID"] != null)
                {
                    int uId = Convert.ToInt32(HttpContext.Current.Request.Cookies["userID"].Value);
                    var user = ctx.Users.Where(u => u.ID == uId).FirstOrDefault();
                    if (user.RoleID != 3)
                    {
                        lst = (from m in ctx.Menus
                               join mr in ctx.Menu_Role on m.ID equals mr.MenuID
                               join r in ctx.Roles on mr.RoleID equals r.RoleValue
                               where r.RoleValue == user.RoleID && m.ParentID == id && m.Flag == 1 && mr.Flag==1
                               orderby m.Priority
                               select m).ToList();
                    }
                    else
                    {
                        lst= ctx.Menus.Where(c => c.Flag == 1  && c.ParentID == id).ToList();
                    }
                }

            }
            return lst;
        }
        
    }
}