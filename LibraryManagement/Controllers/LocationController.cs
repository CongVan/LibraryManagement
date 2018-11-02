using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagement.Controllers
{
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult Index()
        {

            using (var ctx = new LibraryManagementEntities())
            {
                var lcs = ctx.Locations.Where(c => !c.ParentID.HasValue).ToList();
                return View(lcs);
            }

        }
        [HttpPost]
        public ActionResult InsertBookCase(Location lc)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                try
                {
                    lc.Type = 1;
                    lc.Flag = true;
                    lc.ParentID = null;
                    ctx.Locations.Add(lc);
                    ctx.SaveChanges();
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public ActionResult InsertShelf(Location lc)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                try
                {
                    lc.Type = 2;
                    lc.Flag = true;
                    lc.ParentID = lc.ParentID;
                    ctx.Locations.Add(lc);
                    ctx.SaveChanges();
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult GetBookCases()
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var lcs = ctx.Locations.Where(c => !c.ParentID.HasValue).Select(c => new
                {
                    ID = c.ID,
                    Name = c.Name,
                    Flag = c.Flag,
                    HasChild = ctx.Locations.Where(a => a.ParentID == c.ID).FirstOrDefault() != null
                }).ToList();

                return Json(lcs, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetBookCasesForSelect()
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var lcs = ctx.Locations.Where(c => !c.ParentID.HasValue).ToList();
                var lstRet = new List<Dictionary<object, object>>();
                foreach (var lc in lcs)
                {
                    var bs = ctx.Locations.Where(c => c.ParentID == lc.ID).Select(c => new { id = c.ID, text = c.Name }).ToList();
                    var item = new Dictionary<object, object>();
                    if (bs == null || bs.Count == 0)
                    {
                        item.Add("id", lc.ID);
                        item.Add("text", lc.Name);
                    }
                    else
                    {
                        item.Add("id", lc.ID);
                        item.Add("text", lc.Name);
                        item.Add("children", bs);
                    }
                    lstRet.Add(item);
                }
                return Json(lstRet, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult getBookCaseWithID(int id)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var lcs = ctx.Locations.Where(c => c.ID == id).Select(c=>new {
                    ID=c.ID,
                    Name=c.Name,
                    ParentID=c.ParentID,
                    Flag=c.Flag??false
                }).FirstOrDefault();

                return Json(lcs, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult UpdateBookCase(Location lc)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var lcUpdate = ctx.Locations.Where(c => c.ID == lc.ID).FirstOrDefault();
                if (lcUpdate != null)
                {
                    lcUpdate.ParentID = lc.ParentID;
                    lcUpdate.Name = lc.Name;
                    lcUpdate.Flag = lc.Flag;
                    ctx.Entry(lcUpdate).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpPost]
        public ActionResult DeleteBookCase(int id)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var lc = ctx.Locations.Where(c => c.ID == id).FirstOrDefault();
                if (lc != null)
                {
                    ctx.Entry(lc).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();
                    return Json(new { result = 1, msg = "OK" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = -1, msg = "Có lỗi xảy ra" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult GetShelf(int id)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                return Json(
                    ctx.Locations.Where(c => c.ParentID == id).ToList(),
                    JsonRequestBehavior.AllowGet);
            }

        }
    }
}