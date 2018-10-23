using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagement.Controllers
{
    public class BookController : Controller
    {
        // GET: Book
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAllBook()
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var books = (from book in ctx.Books
                             join cate in ctx.Categories on book.CategoryID equals cate.ID
                             join pub in ctx.Publishers on book.PublisherID equals pub.ID
                             join loc in ctx.Locations on book.LocationID equals loc.ID
                             select new
                             {
                                 id = book.ID,
                                 image = book.Image??"",
                                 title = book.Title,
                                 price = book.Price??0,
                                 quantity=book.Quantity??0,
                                 publishName = pub.Name,
                                 cateName = cate.Name,
                                 numberOfPages = book.NumberOfPages??0,
                                 locationName=loc.Name,
                                 importDate= book.ImportDate,
                                 flag=book.Flag,
                             }).ToList().Select(c=>new {
                                 id = c.id,
                                 image=c.image.Split( ';').FirstOrDefault(),
                                 title = c.title,
                                 price = c.price.ToString("N0"),
                                 quantity=c.quantity.ToString("N0"),
                                 publishName = c.publishName,
                                 cateName = c.cateName,
                                 numberOfPages = c.numberOfPages.ToString("N0"),
                                 locationName = c.locationName,
                                 importDate = c.importDate.Value.ToString("dd/MM/yyyy"),
                                 flag = c.flag,

                             }).ToList();

                return Json(books, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddBook()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadImage()
        {
            List<HttpPostedFileBase> allFiles = Enumerable.Range(0, Request.Files.Count)
                                                .Select(x => Request.Files[x])
                                                .Where(x => !string.IsNullOrEmpty(x.FileName))
                                                .ToList();
            TempData["ListImageUpload"] = allFiles;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddBook(Book book)
        {
            IEnumerable<HttpPostedFileBase> files = TempData["ListImageUpload"] as IEnumerable<HttpPostedFileBase>;
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    book.ImportDate = DateTime.Now;
                    book.Flag = true;
                    ctx.Books.Add(book);
                    ctx.SaveChanges();
                    string auDirPath = Server.MapPath("~/Public/BookImages");
                    string targetDirPath = Path.Combine(auDirPath, book.ID.ToString());
                    Directory.CreateDirectory(targetDirPath);
                    List<string> fileNames = new List<string>();
                    int i = 0;
                    foreach (var file in files)
                    {
                        if (file.ContentLength > 0 && file != null)
                        {
                            string pathOfFile = Path.Combine(targetDirPath, $"{i}.jpg");
                            string linkToFile = $"/Public/BookImages/{book.ID}/{i}.jpg";
                            fileNames.Add(linkToFile);
                            file.SaveAs(pathOfFile);
                            i++;
                        }
                    }
                    book.Image = String.Join(";", fileNames);
                    ctx.Entry(book).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
                    ViewBag.msgSuccess = "Nhập sách thành công!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.msgError = ex.Message;
            }
            return View();
        }
        [HttpPost]
        public ActionResult DeleteBook(int id)
        {
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var b = ctx.Books.Where(c => c.ID == id).FirstOrDefault();
                    if (b != null)
                    {
                        b.Flag = false;
                        ctx.Entry(b).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        return Json(new { result = 1, msg = "OK" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { result = -1, msg = "Không tìm thấy sách!" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }
        public ActionResult ParitalBookDetail()
        {
            return View();
        }
        public ActionResult BookDetail()
        {
            return View();
        }
    }
}