using LibraryManagement.Filter;
using LibraryManagement.Models;
using LibraryManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagement.Controllers
{
    public class UserController : Controller
    {
        
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoadAllUser(int? type)
        {
            type = type.HasValue ? type : 1;
            using (var ctx=new LibraryManagementEntities())
            {
                return Json(
                    ctx.Users.Where(c => c.Flag == true && c.RoleID==type).ToList(),JsonRequestBehavior.AllowGet
                    );
            }
            
        }
        [HttpPost]
        public ActionResult InsertUser(User user)
        {
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    user.Password = Utility.StringUtility.HashSHA1(user.UserName);
                    user.RoleID = user.RoleID??1;
                    user.Flag = true;
                    user.ConfirmEmail = false;
                    user.Status = 1;
                    ctx.Users.Add(user);
                    ctx.SaveChanges();
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            
           
        }
        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var user = ctx.Users.Where(c => c.ID == id).FirstOrDefault();
                    ctx.Entry(user).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Login()
        {
            return View();
        }
        [CheckLogin]
        public ActionResult Profile()
        {
            var user = CurrentContext.GetCurUser();
            return View(user);
        }
        public ActionResult LogOut()
        {   
            CurrentContext.Detroy();
            
            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            using (var ctx=new LibraryManagementEntities())
            {
                var pass = Utility.StringUtility.HashSHA1(user.Password);
                var u = ctx.Users.Where(c => c.UserName == user.UserName && c.Password == pass).FirstOrDefault();
                if (u != null)
                {
                    Session["isLogin"] = 1;
                    Session["User"] = u;
                    if (Response.Cookies["userID"] != null)
                    {
                        Response.Cookies["userID"].Value = u.ID.ToString();
                        Response.Cookies["userID"].Expires = DateTime.Now.AddDays(7);
                    }
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
            }

        }
        public ActionResult AccessDenied(string next)
        {
            ViewBag.Next = next;
            return View();
        }
    }
}