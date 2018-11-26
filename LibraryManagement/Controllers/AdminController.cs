using LibraryManagement.Filter;
using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagement.Controllers
{
    [CheckLogin]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.msgSuccess = TempData["msgSuccess"] == null ? null : TempData["msgSuccess"].ToString();
            ViewBag.msgError = TempData["msgError"] == null ? null : TempData["msgError"].ToString();
            return View();
        }

        [HttpPost]
        public ActionResult InsertMenu(Menu menu)
        {
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    menu.Flag = 1;
                    menu.Visible = true;
                    ctx.Menus.Add(menu);
                    ctx.SaveChanges();
                    TempData["msgSuccess"] = "Cập nhật danh mục sách thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["msgError"] = $"Có lỗi xảy ra! Lỗi( {ex.Message} )";
            }
            return RedirectToAction("Index", "Admin");
        }
        public JsonResult GetIcons()
        {
            string auDirPath = Server.MapPath("~/assets/m_icon.txt");
            var s = System.IO.File.ReadAllLines(auDirPath).ToList();
            var i = 1;
            var lst = s.Select(c => new
            {
                id = i++,
                text = c.Trim(),
                code= " material-icons"
            }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
            //try
            //{   // Open the text file using a stream reader.
            //    string auDirPath = Server.MapPath("~/assets/m_icon.txt");
            //    using (StreamReader sr = new StreamReader(auDirPath))
            //    {
            //        // Read the stream to a string, and write the string to the console.
            //        String line = sr.ReadToEnd();

            //        Console.WriteLine(line);
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("The file could not be read:");
            //    Console.WriteLine(e.Message);
            //}
        }
        public JsonResult GetMenusParent()
        {
            using (var ctx=new LibraryManagementEntities())
            {
                var menus = (from m in ctx.Menus
                             where m.ParentID==-1 && m.Flag==1
                             let hasChild = (from c in ctx.Menus
                                             where c.ParentID == m.ID
                                             select c.ID).Count()
                             select new
                             {
                                 ID = m.ID,
                                 HasChild = hasChild,
                                 Title = m.Title,
                                 Priority=m.Priority,
                                 Visible=m.Visible
                             }).OrderBy(c=>c.Priority).ToList();

                return Json(menus, 
                    JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetMenusChildren(int id)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                return Json(
                    ctx.Menus.Where(c => c.ParentID == id && c.Flag == 1).OrderBy(c => c.Priority).ToList(),
                    JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetMenu(int id)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                return Json(
                    ctx.Menus.Where(c => c.ID == id && c.Flag==1).FirstOrDefault(),
                    JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UpdateMenu(Menu menu)
        {
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var m = ctx.Menus.Where(c => c.ID == menu.ID ).FirstOrDefault();
                    if (m != null)
                    {
                        m.Title = menu.Title;
                        m.Priority = menu.Priority;
                        m.Visible = menu.Visible??true;
                        m.Path = menu.Path;
                        m.ParentID = menu.ParentID;
                        m.Icon = menu.Icon;
                        ctx.Entry(m).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        TempData["msgSuccess"] = "Cập nhật Menu thành công!";
                    }
                    else
                    {
                        TempData["msgError"] = $"Không tìm thấy Menu";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msgError"] = $"Có lỗi xảy ra! Lỗi( {ex.Message} )";
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public JsonResult DeleteMenu(int id)
        {
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var m = ctx.Menus.Where(c => c.ID == id).FirstOrDefault();
                    if (m != null)
                    {
                        m.Flag = 0;
                        ctx.Entry(m).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        return Json(new { result = 1, msg = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //TempData["msgError"] = $"Không tìm thấy Menu";
                        return Json(new { result = -1, msg = "Không tìm thấy Menu" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }
        public ActionResult PermisionMenu()
        {
           

            return View();
        }

        #region Permission Menu
        public JsonResult GetListGroupUser()
        {
            using (var ctx=new LibraryManagementEntities())
            {
                var users = ctx.Roles.Where(c=>c.RoleName!="Admin").Select(c=>new { id=c.ID,text=c.RoleName}).ToList();
                return Json(users, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetListRoleMenu(int? idRole)
        {
            if (!idRole.HasValue)
            {
                return Json(new List<string>() { }, JsonRequestBehavior.AllowGet);
            }
            using (var ctx = new LibraryManagementEntities())
            {
                var menus = (from m in ctx.Menus
                             let granted = (from r in ctx.Menu_Role
                                            where r.RoleID == idRole && r.MenuID == m.ID && r.Flag==1
                                            select r.ID
                                            ).Count()
                             where m.Flag == 1
                             select new
                             {
                                 IDMenu = m.ID,
                                 TitleMenu = m.Title,
                                 IsGranted = granted,
                                 Icon=m.Icon
                             }).ToList().Select(m=>new {
                                 IDMenu = m.IDMenu,
                                 TitleMenu = m.TitleMenu,
                                 IsGranted = m.IsGranted>0?true:false,
                                 Icon = m.Icon
                             });
                return Json(menus, JsonRequestBehavior.AllowGet);
            }
            
        }
        [HttpPost]
        public JsonResult ApproveMenuRole(int? idMenu,int? idGroup)
        {
            if(!idMenu.HasValue || !idGroup.HasValue)
            {
                return Json(new { data = -1, msg = "Không tìm thấy Menu/ Group" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var mr = new Menu_Role();
                    mr.MenuID = idMenu;
                    mr.RoleID = idGroup;
                    mr.Flag = 1;
                    ctx.Menu_Role.Add(mr);
                    ctx.SaveChanges();
                    return Json(new { data = 1, msg = "OK" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { data = -1, msg = ex.Message}, JsonRequestBehavior.AllowGet);
                
            }
            
        }
        [HttpPost]
        public JsonResult DenyMenuRole(int? idMenu, int? idGroup)
        {
            if (!idMenu.HasValue || !idGroup.HasValue)
            {
                return Json(new { data = -1, msg = "Không tìm thấy Menu/ Group" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var mr = ctx.Menu_Role.Where(c => c.MenuID == idMenu && c.RoleID == idGroup).FirstOrDefault();
                    if (mr != null)
                    {
                        mr.Flag = 0;
                        ctx.Entry(mr).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        return Json(new { data = 1, msg = "OK" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = -1, msg = "Không tìm thấy Menu/ Group" }, JsonRequestBehavior.AllowGet);
                    }
                    
                    
                }
            }
            catch (Exception ex)
            {
                return Json(new { data = -1, msg = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }
        #endregion
    }
}