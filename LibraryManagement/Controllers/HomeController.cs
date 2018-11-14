using LibraryManagement.Filter;
using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagement.Controllers
{
    [CheckLogin]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var ctx=new LibraryManagementEntities())
            {
                var countBook = ctx.Books.Where(c=>c.Flag==true).Count();
                var countUser = ctx.Users.Where(c => c.Flag == true).Count();
                var countBill = ctx.BookBillDetails.Where(c => c.Returned == false).Count();
                var countView = 100;
                ViewBag.countBook = countBook;
                ViewBag.countUser = countUser;
                ViewBag.countBill = countBill;
                ViewBag.countView = countView;
            }
            
            return View();
        }
        public ActionResult GetBillOutDate()
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var bills = (from bb in ctx.BookBills
                            join bd in ctx.BookBillDetails on bb.ID equals bd.BookBillID
                            join b in ctx.Books on bd.BookID equals b.ID
                            join u in ctx.Users on bb.UserID equals u.ID
                            //where bd.Returned == false
                            select new
                            {
                                UserName = u.UserName,
                                FullName = u.FullName,
                                DateReturn = bd.DateReturn,
                                BookName=b.Title
                            }).ToList().Select(c=>new {
                                UserName = c.UserName,
                                FullName = c.FullName,
                                DateReturn = c.DateReturn.Value.ToString("dd-MM-yyyy"),
                                CountDate=(DateTime.Now-c.DateReturn).Value.TotalDays.ToString("N0"),
                                BookName = c.BookName
                            }).Take(10);
                return Json(bills, JsonRequestBehavior.AllowGet);

            }

            
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}