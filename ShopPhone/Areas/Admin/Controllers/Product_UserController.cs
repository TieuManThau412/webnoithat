using ShopNoiThat.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNoiThat.Areas.Admin.Controllers
{
    public class Product_UserController : Controller
    {
        // GET: Admin/Product_User
        ShopNoiThatDbContext db = new ShopNoiThatDbContext();

        public ActionResult Index()
        {
            ViewData["list"] = db.Product_Users.Where(g=>g.IsPhanHoi==false).ToList();
            ViewData["User"] = db.users.ToList();
            return View();
        }
        public ActionResult PhanHoi(int? id)
        {
            ViewBag.listCate = db.Categorys.Where(m => m.status != 0 && m.ID > 2).ToList();
            var ph = db.Product_Users.Find(id);
            return View(ph);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult PhanHoi(Product_User mproduct, HttpPostedFileBase UploadImage,string details="")
        {

            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(UploadImage.FileName);
                var path = Path.Combine(Server.MapPath("~/Upload"), fileName);
               var p= Path.Combine(Server.MapPath("~/public/images"), fileName);
                UploadImage.SaveAs(path);
                UploadImage.SaveAs(p);
                mproduct.Image = UploadImage.FileName;
                mproduct.IsPhanHoi = true;
                db.Entry(mproduct).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.listCate = db.Categorys.Where(m => m.status != 0 && m.ID > 2).ToList();
                Message.set_flash("Phản hồi thành công", "success");
                Mproduct k = new Mproduct();
                string slug = Mystring.ToSlug(mproduct.NameProduct.ToString());
                if (db.Categorys.Where(m => m.slug == slug).Count() > 0)
                {
                    Message.set_flash("Sản phẩm đã tồn tại trong bảng Category", "danger");
                    return View(mproduct);
                }
                if (db.topics.Where(m => m.slug == slug).Count() > 0)
                {
                    Message.set_flash("Sản phẩm đã tồn tại trong bảng Topic", "danger");
                    return View(mproduct);
                }
                if (db.Products.Where(m => m.slug == slug).Count() > 0)
                {
                    Message.set_flash(" Sản phẩm đã tồn tại trong bảng Product", "danger");
                    return View(mproduct);
                }
                // lấy tên loại sản phẩm
                var catid = Convert.ToInt32(Request["catid"]);
                var namecateDb = db.Categorys.Where(m => m.ID == catid).First();
                string namecate = Mystring.ToStringNospace(namecateDb.name);
                k.img = UploadImage.FileName;
                k.slug = slug;
                k.sold = 0;
                k.created_at = DateTime.Now;
                k.updated_at = DateTime.Now;
                k.created_by = int.Parse(Session["Admin_id"].ToString());
                k.updated_by = int.Parse(Session["Admin_id"].ToString());
                k.name = mproduct.NameProduct;
                k.catid= catid;
                k.Submenu = Convert.ToInt32(Request["Submenu"]);
                k.detail = details;
                k.number = Convert.ToInt32(Request["number"]);
                k.price = mproduct.Price;
                k.pricesale = Convert.ToDouble(Request["pricesale"]);
                k.metakey = Request["metakey"];
                k.metadesc = Request["metadesc"];
                k.status = 1;
                db.Products.Add(k);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            Message.set_flash("Phản hồi thất bại", "danger");
            ViewBag.listCate = db.Categorys.Where(m => m.status != 0 && m.ID > 2).ToList();
            return View(mproduct);
        }
    }
}