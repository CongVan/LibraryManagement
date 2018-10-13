using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibraryManagement.Models;
namespace LibraryManagement.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {

            ViewBag.msgSuccess = TempData["msgSuccess"] == null ? null : TempData["msgSuccess"].ToString();
            ViewBag.msgError = TempData["msgError"] == null ? null : TempData["msgError"].ToString();
            using (var ctx = new LibraryManagementEntities())
            {
                var lstParent = ctx.Categories.Where(c => !c.ParentID.HasValue).ToList();
                var lstRet = new List<Dictionary<object, object>>();
                foreach (var parent in lstParent)
                {
                    var item = new Dictionary<object, object>();
                    item.Add("ID", parent.ID);
                    item.Add("Name", parent.Name);
                    item.Add("Flag", parent.Flag);
                    var lstChild = ctx.Categories.Where(c => c.ParentID == parent.ID).ToList();
                    item.Add("Child", lstChild);
                    //if (lstChild.Count > 0)
                    //{
                    //    item.Add("Child", lstChild);
                    //}
                    lstRet.Add(item);
                }
                ViewBag.LstCategory = lstRet;
            }
            return View();
        }

        public ActionResult GetListCategory()
        {
            using (var ctx=new LibraryManagementEntities())
            {
                var lstParent = ctx.Categories.Where(c => !c.ParentID.HasValue).ToList();
                var lstRet = new List<Dictionary<object, object>>();
                foreach (var parent in lstParent)
                {
                    var item = new Dictionary<object, object>();
                    item.Add("ID", parent.ID);
                    item.Add("Name", parent.Name);
                    item.Add("Flag", parent.Flag);
                    var lstChild = ctx.Categories.Where(c => c.ParentID == parent.ID).ToList();
                    item.Add("Child", lstChild);
                    //if (lstChild.Count>0)
                    //{
                    //    item.Add("Child", lstChild);
                    //}
                    lstRet.Add(item);
                }
                ViewBag.LstCategory = lstRet;
                return Json(lstRet, JsonRequestBehavior.AllowGet);
            }

            
        }
        public ActionResult GetListCategoryParent()
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var lstParent = ctx.Categories.Where(c => !c.ParentID.HasValue).ToList();
               
                return Json(lstParent, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult InsertCategory(Category cate)
        {
            
            using (var ctx=new LibraryManagementEntities())
            {
                try
                {
                    cate.ParentID = cate.ParentID == -1 ? null : cate.ParentID;
                    cate.Flag = true;

                    ctx.Categories.Add(cate);
                    ctx.SaveChanges();
                    TempData["msgSuccess"] = "Thêm danh mục thành công!";
                }
                catch (Exception ex)
                {
                    TempData["msgError"] = $"Thêm danh mục thất bại!\n{ex.Message}";
                }
            }
            return RedirectToAction("Index", "Category");
        }
        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            using (var ctx=new LibraryManagementEntities())
            {
                
                try
                {
                    var cate = ctx.Categories.Where(c => c.ID == id).FirstOrDefault();
                    if (cate != null)
                    {
                        ctx.Entry(cate).State = System.Data.Entity.EntityState.Deleted;
                        ctx.SaveChanges();
                        return Json(new { result = 1, msg = "OK" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { result = -1, msg = "Không tìm thấy danh mục!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { result = -1, msg = $"Có lỗi xảy ra.\nLỗi: {ex.Message}" }, JsonRequestBehavior.AllowGet);

                }
            }
        }
        public ActionResult GetCategory(int id)
        {
            using (var ctx=new LibraryManagementEntities())
            {
                return Json(
                    ctx.Categories.Where(c => c.ID == id).FirstOrDefault(),
                    JsonRequestBehavior.AllowGet
                );
            }
            
        }
        [HttpPost]
        public ActionResult UpdateCategory(Category cate)
        {
            
            using (var ctx=new LibraryManagementEntities())
            {
                try
                {
                    
                    var cateUpdate = ctx.Categories.Where(c => c.ID == cate.ID).FirstOrDefault();
                    if (cateUpdate != null) {
                        cateUpdate.Name = cate.Name;
                        cateUpdate.ParentID = cate.ParentID == -1 ? null : cate.ParentID;
                        cateUpdate.Flag = cate.Flag;
                        ctx.Entry(cateUpdate).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        TempData["msgSuccess"] = "Cập nhật danh mục sách thành công!";
                    }
                    else
                    {
                        TempData["msgError"] = "Không tìm danh mục sách.\nVui lòng thử lại!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["msgError"] = $"Có lỗi xảy ra! Lỗi( {ex.Message} )";
                }
                return RedirectToAction("Index", "Category");
            }
        }
    }
}