using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagement.Controllers
{
    public class PublisherController : Controller
    {
        string msgSuccess = null;
        string msgError = null;
        // GET: Publisher
        public ActionResult Index()
        {
            ViewBag.msgSuccess = TempData["msgSuccess"]==null?null: TempData["msgSuccess"].ToString();
            ViewBag.msgError = TempData["msgError"]==null?null: TempData["msgError"].ToString();
            using (var ctx = new LibraryManagementEntities())
            {
                return View(ctx.Publishers.ToList());
            }
           
        }

        [HttpPost]
        public ActionResult InsertPublisher(Publisher ps)
        {
            
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    ps.Flag = true;
                    ctx.Publishers.Add(ps);
                    ctx.SaveChanges();
                    msgSuccess = "Thêm NXB thành công!";
                }
            }
            catch (Exception ex)
            {
                msgError = "Thêm NXB thất bại!\nLỗi: " + ex.Message;
            }
            TempData["msgError"] = msgError;
            TempData["msgSuccess"] = msgSuccess;
            return RedirectToAction("Index", "Publisher");
        }
        public ActionResult GetListPublisher()
        {
            using (var ctx=new LibraryManagementEntities())
            {

                return Json(ctx.Publishers.ToList(),JsonRequestBehavior.AllowGet);
            }
        }
    }
}