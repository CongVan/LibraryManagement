using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibraryManagement.Filter;
using LibraryManagement.Models;
namespace LibraryManagement.Controllers
{
    [CheckLogin]
    public class BookBillController : Controller
    {
        // GET: BookPay
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BorrowBook()
        {
            return View();
        }
        public ActionResult PayBook()
        {
            return View();
        }
        public ActionResult LostBook()
        {
            return View();
        }
        #region BorrowBook
        public ActionResult SearchUser()
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var users = ctx.Users.Where(c => c.Flag == true).ToList();
                return Json(users, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult GetInfoUser(int? userId)
        {
            if (!userId.HasValue)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            using (var ctx = new LibraryManagementEntities())
            {
                var users = (from user in ctx.Users
                             let countBillPay = (from billPay in ctx.BookBills
                                                 join billDetail in ctx.BookBillDetails on billPay.ID equals billDetail.BookBillID
                                                 where billPay.UserID == user.ID && billPay.Type.Value == 1
                                                 select billDetail).Count()
                             let countBillReturn = (from billRe in ctx.BookBills
                                                    join billDetail in ctx.BookBillDetails on billRe.ID equals billDetail.BookBillReturnID
                                                    where billRe.UserID == user.ID && billRe.Type.Value == 2 && billDetail.Returned == true
                                                    select billDetail).Count()
                                                    where user.ID==userId.Value
                             select new
                             {
                                 ID = user.ID,
                                 UserName = user.UserName,
                                 FullName = user.FullName,
                                 Email = user.Email,
                                 Status = user.Status,
                                 PhoneNumber = user.PhoneNumber,
                                 CountPay = countBillPay,
                                 CountReturn = countBillReturn,
                                 CountNoReturn = countBillPay - countBillReturn
                             }).ToList().FirstOrDefault();

                return Json(users, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult SearchBook()
        {
            using (var ctx = new LibraryManagementEntities())
            {
                var books = (from book in ctx.Books
                             join pub in ctx.Publishers on book.PublisherID equals pub.ID
                             where book.Flag == true && book.Quantity > 0
                             select new
                             {
                                 ID = book.ID,
                                 Title = book.Title,
                                 Quantity = book.Quantity,
                                 PublisherName = pub.Name,
                                 Author = book.Author,
                                 Image = book.Image,
                             }).ToList().Select(c => new
                             {
                                 ID = c.ID,
                                 Title = c.Title,
                                 Quantity = c.Quantity,
                                 PublisherName = c.PublisherName,
                                 Author = c.Author,
                                 Image = c.Image.Split(';').FirstOrDefault(),
                             }).ToList();
                return Json(books, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult GetInfoBook(int id)
        {
            using (var ctx = new LibraryManagementEntities())
            {
                return Json(
                    ctx.Books.Where(c => c.Flag == true && c.Quantity.Value > 0 && c.ID == id).Select(
                        c => new
                        {
                            ID = c.ID,
                            Title = c.Title,
                            Quantity = c.Quantity,
                            NumberBorrow = 1,
                            Image = c.Image,
                        }
                        ).ToList().Select(c => new
                        {
                            ID = c.ID,
                            Title = c.Title,
                            Quantity = c.Quantity,
                            NumberBorrow = c.NumberBorrow,
                            Image = c.Image.Split(';').FirstOrDefault(),
                        }).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult PayBook(int idUser, string idBooks)
        {

            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    string[] arrIdBook = idBooks.Split(',');
                    var bill = new BookBill();
                    bill.Date = DateTime.Now;
                    bill.UserID = idUser;
                    bill.Type = 1;
                    bill.Quantity = arrIdBook.Length;
                    ctx.BookBills.Add(bill);
                    ctx.SaveChanges();
                    string code = $"0000{bill.ID}";
                    string billCode = $"PM{code.Substring(code.Length - 4)}";
                    bill.Code = billCode;
                    ctx.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
                    var setting = ctx.Settings.Where(c => c.Content == "NgayTraSach").FirstOrDefault();
                    foreach (string sid in arrIdBook)
                    {
                        var id = int.Parse(sid);
                        var billDetail = new BookBillDetail();
                        var book = ctx.Books.Where(c => c.ID == id).FirstOrDefault();

                        book.Quantity--;
                        billDetail.BookBillID = bill.ID;
                        billDetail.Quantity = 1;
                        billDetail.Price = book.Price;
                        billDetail.BookID = book.ID;
                        billDetail.DateReturn = DateTime.Now.AddDays(setting.Value.Value);
                        ctx.Entry(book).State = System.Data.Entity.EntityState.Modified;
                        ctx.BookBillDetails.Add(billDetail);
                        ctx.SaveChanges();

                    }
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

            return Json(-1, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region PayBook
        public ActionResult GetBookBill(int? userId)
        {
            if (!userId.HasValue)
            {

                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            using (var ctx = new LibraryManagementEntities())
            {
                var bills = (from b in ctx.BookBills
                             let countDetail = (from bD in ctx.BookBillDetails
                                                where bD.BookBillID.Value == b.ID
                                                select bD.ID).Count()
                             where b.UserID == userId && b.Type==1
                             select new
                             {
                                 ID = b.ID,
                                 Code = b.Code,
                                 Date = b.Date,
                                 Quantity = countDetail
                             }).ToList().Select(b => new
                             {
                                 ID = b.ID,
                                 Code = b.Code,
                                 Date = b.Date.Value.ToString("dd-MM-yyyy"),
                                 Quantity = b.Quantity
                             }).ToList();

                return Json(bills, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBillDetail(int? id)
        {
            if (!id.HasValue)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            using (var ctx = new LibraryManagementEntities())
            {
                var lst = (from bD in ctx.BookBillDetails
                           join b in ctx.Books on bD.BookID equals b.ID
                           where bD.BookBillID == id
                           select new
                           {
                               ID = bD.ID,
                               Image = b.Image,
                               Title = b.Title,
                               Status = bD.Returned,

                           }).ToList().Select(c => new
                           {
                               ID = c.ID,
                               Image = c.Image.Split(';').FirstOrDefault(),
                               Title = c.Title,
                               Status = c.Status,
                           }).ToList();
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReturnBook(int idUser, string idBooks)
        {
            try
            {
                using (var ctx = new LibraryManagementEntities())
                {
                    var arrIdBook = idBooks.Split(',').ToList().Select(int.Parse).ToList();
                    var billReturn = new BookBill();
                    billReturn.UserID = idUser;
                    billReturn.Type = 2;
                    billReturn.Date = DateTime.Now;
                    ctx.BookBills.Add(billReturn);
                    ctx.SaveChanges();
                    string code = $"0000{billReturn.ID}";
                    string billCode = $"PT{code.Substring(code.Length - 4)}";
                    //var billDetails = ctx.BookBillDetails.ToList().Where(c => arrIdBook.Exists(b=>b==c.ID.ToString())==true).ToList() ;
                    arrIdBook.ForEach( id=> {
                        var billDetail = ctx.BookBillDetails.Where(c => c.ID == id).FirstOrDefault();
                        billDetail.Returned = true;
                        billDetail.BookBillReturnID = billReturn.ID;
                        billDetail.DateReturned = DateTime.Now;
                        ctx.Entry(billDetail).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                    });
                    //ctx.Entry(billDetails).State = System.Data.Entity.EntityState.Modified;
                    
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            
        }
        #endregion

    }
}