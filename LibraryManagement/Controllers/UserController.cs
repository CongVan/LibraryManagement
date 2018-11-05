using LibraryManagement.Models;
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
        public ActionResult LoadAllUser()
        {
            using (var ctx=new LibraryManagementEntities())
            {
                return Json(
                    ctx.Users.Where(c => c.Flag == true).ToList(),JsonRequestBehavior.AllowGet
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
                    user.Password = StringUtility.StringUtility.HashSHA1(user.UserName);
                    user.RoleID = 1;
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
        
    }
}