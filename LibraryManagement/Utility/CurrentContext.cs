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
        public static List<Role> GetRole()
        {
            using (var ctx=new LibraryManagementEntities())
            {
                var role = ctx.Roles.ToList();
                return role;
            }
            
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
            using (var ctx=new LibraryManagementEntities())
            {
                return ctx.Menus.Where(c => c.Flag == 1 && c.ParentID==-1).ToList();
            }
        }
        public static List<Menu> getMenusChild(int id)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                return ctx.Menus.Where(c => c.Flag == 1 && c.ParentID==id).ToList();
            }
        }
    }
}