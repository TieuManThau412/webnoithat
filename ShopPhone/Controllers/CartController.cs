using ShopNoiThat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNoiThat.Controllers
{
    public class CartController : Controller
    {
        // khởi tạo session:
        private const string SessionCart = "SessionCart";
        // GET: Cart
        ShopNoiThatDbContext db = new ShopNoiThatDbContext();
        public ActionResult Index()
        {
            var cart = Session[SessionCart];
            var list = new List<Cart_item>();
            if (cart != null)
            {
                list = (List<Cart_item>)cart;
            }
            return View(list);
        }
        public ActionResult card_header()
        {
            var cart = Session[SessionCart];
            var list = new List<Cart_item>();
            float priceTotol = 0;
            if (cart != null)
            {
                list = (List<Cart_item>)cart;
                foreach (var item1 in list)
                {
                    if (item1.product.pricesale > 0)
                    {
                        int temp = (((int)item1.product.price) - ((int)item1.product.price / 100 * (int)item1.product.pricesale)) * ((int)item1.quantity);

                        priceTotol += temp;
                    }
                    else
                    {
                        int temp = (int)item1.product.price * (int)item1.quantity;
                        priceTotol += temp;
                    }

                }
            }
            ViewBag.CartTotal = priceTotol;
            return View(list);
        }
        public RedirectToRouteResult updateitem(long P_SanPhamID, int P_quantity)
        {
            var cart = Session[SessionCart];
            var list = (List<Cart_item>)cart;
            Cart_item itemSua = list.FirstOrDefault(m => m.product.ID == P_SanPhamID);
            if (itemSua != null)
            {
                if (itemSua.quantity + P_quantity > itemSua.product.number - itemSua.product.sold)
                {
                    Message.set_flash("Số lượng bạn chọn đã lớn hơn số lượng trong kho", "danger");
                    return RedirectToAction("Index");
                }
                itemSua.quantity = P_quantity;
            }
            return RedirectToAction("Index");
        }
        public RedirectToRouteResult deleteitem(long productID)
        {
            var cart = Session[SessionCart];
            var list = (List<Cart_item>)cart;

            Cart_item itemXoa = list.FirstOrDefault(m => m.product.ID == productID);
            if (itemXoa != null)
            {
                list.Remove(itemXoa);
                if (list.Count == 0)
                {
                    Session["SessionCart"] = null;
                }
            }
            return RedirectToAction("Index");
        }
        public JsonResult Additem(long productID, int quantity)
        {
            var item = new Cart_item();
            Mproduct product = db.Products.Find(productID);
            var cart = Session[SessionCart];
            if (cart != null)
            {
                var list = (List<Cart_item>)cart;
                if (list.Exists(m => m.product.ID == productID))
                {
                    int quantity1 = 0;
                    bool bad = false;
                    foreach (var item1 in list)
                    {
                        if (item1.product.ID == productID)
                        {
                            if ((item1.quantity + quantity) > (item1.product.number - item1.product.sold))
                            {
                                bad = true;

                            }
                            else
                            {
                                item1.quantity += quantity;
                                quantity1 = item1.quantity;
                            }

                        }
                    }
                    int priceTotol = 0;


                    foreach (var item1 in list)
                    {
                        if (item1.product.pricesale > 0)
                        {
                            int temp = (((int)item1.product.price) - ((int)item1.product.price / 100 * (int)item1.product.pricesale)) * ((int)item1.quantity);

                            priceTotol += temp;
                        }
                        else
                        {
                            int temp = (int)item1.product.price * (int)item1.quantity;
                            priceTotol += temp;
                        }

                    }
                    return Json(new
                    {
                        ProductPrice = ((int)product.price) - (((int)product.price / 100) * ((int)product.pricesale)),
                        bad = bad,
                        price = product.price,
                        priceSale = product.pricesale,
                        quantity = quantity1,
                        priceTotol = priceTotol,
                        productID = productID,
                        meThod = "updateQuantity"
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    item.meThod = "cartExist";
                    item.f = false;
                    if (quantity > (product.number - product.sold))
                    {
                        item.f = true;
                        item.quantity = 0;
                    }
                    else
                    {
                        item.quantity = quantity;
                        list.Add(item);
                        item.product = product;
                        item.countCart = list.Count();
                        item.meThod = "cartExist";
                        int priceTotol = 0;
                        foreach (var item1 in list)
                        {
                            if (item1.product.pricesale > 0)
                            {
                                int temp = (((int)item1.product.price) - ((int)item1.product.price / 100 * (int)item1.product.pricesale)) * ((int)item1.quantity);
                                priceTotol += temp;

                            }
                            else
                            {
                                int temp = (int)item1.product.price * (int)item1.quantity;
                                priceTotol += temp;
                            }
                        }
                        item.priceTotal = priceTotol;
                        item.priceSaleee = (int)product.price - (int)product.price / 100 * (int)product.pricesale;
                    }
                    return Json(item, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                item.product = product;
                item.meThod = "cartEmpty";
                item.countCart = 1;


                item.f = false;
                if (quantity > (product.number - product.sold))
                {
                    item.f = true;
                    item.quantity = 0;
                }
                else
                {
                    item.quantity = quantity;
                    var list = new List<Cart_item>();
                    list.Add(item);
                    Session[SessionCart] = list;
                    if (item.product.pricesale > 0)
                    {
                        item.priceTotal = (((int)item.product.price) - ((int)item.product.price / 100 * (int)item.product.pricesale)) * ((int)item.quantity);
                    }
                    else
                    {
                        item.priceTotal = (int)product.price;
                    }
                    item.priceTotal = (((int)item.product.price) - ((int)item.product.price / 100 * (int)item.product.pricesale)) * ((int)item.quantity);
                }
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LichSuMuaHang(int? id)
        {
            var listOrderid = db.Orders.Where(g => g.userid == id).Select(g => g.ID).ToList();
            var listproid = db.Orderdetails.Where(g => listOrderid.Contains(g.orderid)).ToList();
            ViewData["pro"] = db.Products.ToList();
            return View(listproid);
        }
        public ActionResult DatHang()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DatHang(Product_User n, HttpPostedFileBase UploadImage)
        {
            if (ModelState.IsValid)
            {

                var fileName = Path.GetFileName(UploadImage.FileName);
                var path = Path.Combine(Server.MapPath("~/Upload"), fileName);
                UploadImage.SaveAs(path);
                n.PhoTo = UploadImage.FileName;
                var model = new Product_User();
                model.IdUser =Convert.ToInt32(Session["id"].ToString());
                model.PhoTo = n.PhoTo;
                model.Description = n.Description;
                model.CreateDate = DateTime.Now;
                model.NameProduct = n.NameProduct;
                model.IsPhanHoi = false;
                model.Image = null;
                model.Price = 0;
                db.Product_Users.Add(model);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Site");
        }
        public ActionResult DanhSachDH()
        {
            var Id= Convert.ToInt32(Session["id"].ToString());
            var list = db.Product_Users.Where(g => g.IdUser == Id && g.IsPhanHoi==true).ToList();
            ViewData["list"] = list;
            return View();
        }
        public JsonResult LayId(int? Id)
        {
            var ten = db.Product_Users.Find(Id).NameProduct;
            var id = db.Products.FirstOrDefault(g => g.name == ten).ID;
            return Json(new { Id = id }, JsonRequestBehavior.AllowGet);
        }
    }
}