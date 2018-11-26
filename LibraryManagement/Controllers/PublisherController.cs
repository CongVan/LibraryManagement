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
    public class PublisherController : Controller
    {
        string msgSuccess = null;
        string msgError = null;
        // GET: Publisher
        public ActionResult Index()
        {
            ViewBag.msgSuccess = TempData["msgSuccess"] == null ? null : TempData["msgSuccess"].ToString();
            ViewBag.msgError = TempData["msgError"] == null ? null : TempData["msgError"].ToString();
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
        public ActionResult GetListAllPublisher()
        {
            using (var ctx = new LibraryManagementEntities())
            {

                return Json(ctx.Publishers.Where(c => c.Flag.Value == true).Select(c=>new { id=c.ID,text=c.Name}).ToList()
                    , JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetListPublisher(int? id)
        {
            using (var ctx = new LibraryManagementEntities())
            {

                return Json(ctx.Publishers.Where(c => id.HasValue == false || (id.HasValue && c.ID == id.Value)).ToList()
                    , JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult UpdatePublisher(Publisher ps)
        {
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var pUpdate = ctx.Publishers.Where(c => c.ID == ps.ID).FirstOrDefault();
                    if (pUpdate != null)
                    {
                        pUpdate.Name = ps.Name;
                        pUpdate.Flag = ps.Flag;
                        ctx.Entry(pUpdate).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        TempData["msgSuccess"] = "Cập nhật nhà xuất bản thành công!";
                    }
                    else
                    {
                        TempData["msgError"] = "Không tìm thấy nhà xuất bản.\nVui lòng thử lại!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msgError"] = $"Có lỗi xảy ra! Lỗi( {ex.Message} )";
            }

            return RedirectToAction("Index", "Publisher");

        }
        [HttpPost]
        public ActionResult DeletePublisher(int? id)
        {

            if (!id.HasValue)
            {
                return Json(new { result = -1, msg = "Mã NXB không hợp lệ!" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var ps = ctx.Publishers.Where(c => c.ID == id.Value).FirstOrDefault();
                    if (ps != null)
                    {
                        ctx.Entry(ps).State = System.Data.Entity.EntityState.Deleted;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        return Json(new { result = -1, msg = "Không tìm thấy NXB!" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, msg = $"Có lỗi xảy ra.\nLỗi: {ex.Message}" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { result = 1, msg = "OK" }, JsonRequestBehavior.AllowGet);
        }
    }
}