using LibraryManagement.Models;
using Newtonsoft.Json;
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
                                 image = book.Image ?? "",
                                 title = book.Title,
                                 price = book.Price ?? 0,
                                 quantity = book.Quantity ?? 0,
                                 publishName = pub.Name,
                                 cateName = cate.Name,
                                 numberOfPages = book.NumberOfPages ?? 0,
                                 locationName = loc.Name,
                                 importDate = book.ImportDate,
                                 flag = book.Flag,
                             }).ToList().Select(c => new
                             {
                                 id = c.id,
                                 image = c.image.Split(';').FirstOrDefault(),
                                 title = c.title,
                                 price = c.price.ToString("N0"),
                                 quantity = c.quantity.ToString("N0"),
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
        [HttpPost]
        public ActionResult DeleteImage(int? id, string image)
        {
            if (!id.HasValue)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var book = ctx.Books.Where(c => c.ID == id.Value).FirstOrDefault();
                    var images = book.Image.Split(';').ToList();
                    var imgDelete = images.Where(c => c.Substring(c.LastIndexOf("/") + 1) == image).FirstOrDefault();
                    images.Remove(imgDelete);
                    book.Image = String.Join(";", images);
                    ctx.Entry(book).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
                }




                string auDirPath = Server.MapPath("~/Public/BookImages");
                string targetDirPath = Path.Combine(auDirPath, id.ToString(), image);
                System.IO.File.Delete(targetDirPath);
            }
            catch (Exception ex)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);

            }
            return Json(1, JsonRequestBehavior.AllowGet);

        }

        public ActionResult BookDetail(int? id)
        {
            ViewBag.msgError = TempData["msgError"] == null ? null : TempData["msgError"].ToString();
            ViewBag.msgSuccess = TempData["msgSuccess"] == null ? null : TempData["msgSuccess"].ToString();
            using (var ctx = new LibraryManagementEntities())
            {
                var book = ctx.Books.Where(c => c.ID == id).FirstOrDefault();
                try
                {
                    string auDirPath = Server.MapPath("~/Public/BookImages");
                    string targetDirPath = Path.Combine(auDirPath, book.ID.ToString());
                    var dir = new DirectoryInfo(targetDirPath);
                    FileInfo[] Files = dir.GetFiles();

                    var lstFile = new List<Dictionary<string, string>>();

                    foreach (FileInfo file in Files)
                    {
                        var dic = new Dictionary<string, string>();
                        dic.Add("caption", file.Name);
                        dic.Add("filename", file.Name);
                        //dic.Add("uploadUrl", $"/Public/BookImages/{book.ID}");
                        dic.Add("url", $"/Book/DeleteImage/?id={book.ID}&image={file.Name}");
                        dic.Add("size", file.Length.ToString());
                        dic.Add("key", $"{book.ID} - {file.Name.Substring(0, file.Name.IndexOf(".jpg"))} - {DateTime.Now.ToString("hhmmss")}");

                        lstFile.Add(dic);
                    }
                    ViewBag.lstFile = JsonConvert.SerializeObject(lstFile);
                }
                catch (Exception ex)
                {

                    ViewBag.lstFile = "[]";
                }
                
                return View(book);
            }

        }

        [HttpPost]
        public ActionResult UpdateBook(Book book)
        {
            
            IEnumerable<HttpPostedFileBase> files = TempData["ListImageUpload"] as IEnumerable<HttpPostedFileBase>;
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var bUpdate = ctx.Books.Where(c => c.ID == book.ID).FirstOrDefault();
                    if (bUpdate == null)
                    {
                        TempData["msgError"] = "Không tìm thấy sách cập nhật!";
                        return RedirectToAction("BookDetail", "Book", new { id = book.ID });
                    }
                    var images = bUpdate.Image.Split(';').ToList();
                    var lastName = images.Last();
                    int lastIndex = int.Parse(lastName.Substring(lastName.LastIndexOf("/") + 1, lastName.IndexOf(".jpg") - lastName.LastIndexOf("/") - 1));

                    string auDirPath = Server.MapPath("~/Public/BookImages");
                    string targetDirPath = Path.Combine(auDirPath, book.ID.ToString());
                    Directory.CreateDirectory(targetDirPath);
                    List<string> fileNames = new List<string>();
                    int i = lastIndex + 1;
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
                    bUpdate.Author = book.Author;
                    bUpdate.CategoryID = book.CategoryID;
                    bUpdate.Description = book.Description;
                    bUpdate.LocationID = book.LocationID;
                    bUpdate.NumberOfPages = book.NumberOfPages;
                    bUpdate.Price = book.Price;
                    bUpdate.PublishDate = book.PublishDate;
                    bUpdate.PublisherID = book.PublisherID;
                    bUpdate.Quantity = book.Quantity;
                    bUpdate.Title = book.Title;
                    bUpdate.Image = String.Join(";", images.Concat(fileNames));
                    ctx.Entry(bUpdate).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
                    TempData["msgSuccess"] = "Cập nhật thành công!";
                    //return Json(new { result = 1, msg = "OK" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TempData["msgError"] = ex.Message;
                //return Json(new { result = -1, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("BookDetail", "Book",new { id=book.ID });
        }
    }
}