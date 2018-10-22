using LibraryManagement.Models;
using System;
using System.Collections.Generic;
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
                            string linkToFile = $"/Public/BookImages/{i}.jpg";
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
    }
}