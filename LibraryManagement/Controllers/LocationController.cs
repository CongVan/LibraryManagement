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
                var lcs = ctx.Locations.ToList();
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
        public ActionResult GetBookCases()
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var lcs = ctx.Locations.ToList();
                return Json(lcs, JsonRequestBehavior.AllowGet);
            }
        }
    }
}