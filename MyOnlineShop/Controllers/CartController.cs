using MyOnlineShop.Models;
using MyOnlineShop.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using static MyOnlineShop.Controllers.ShopController;

namespace MyOnlineShop.Controllers
{
    
    public class CartController : Controller
    {
        MyOnlineShopDb db = new MyOnlineShopDb();


        [NoDirectAccess]
        [Authorize]
        //GET: Cart
        public ActionResult Index()
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Check if cart is empty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            // Calculate total and save to ViewBag

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // Return view with list
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            // Init CartVM
            CartVM model = new CartVM();

            // Init quantity
            int qty = 0;

            // Init price
            decimal price = 0m;

            // Check for cart session
            if (Session["cart"] != null)
            {
                // Get total qty and price
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;

            }
            else
            {
                // Or set qty and price to 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            // Return partial view with model
            return PartialView(model);
        }

        [Authorize]
        public ActionResult AddToCartPartial(int id)
        {
            // Init CartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Init CartVM
            CartVM model = new CartVM();


            // Get the product
            Product product = db.Products.Find(id);

            // Check if the product is already in cart
            var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

            // If not, add new
            if (productInCart == null)
            {
                cart.Add(new CartVM()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = 1,
                    Price = (decimal)product.Price,
                    Image = product.ProductURL
                });
            }
            else
            {
                // If it is, increment
                productInCart.Quantity++;
            }


            // Get total qty and price and add to model

            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Save cart back to session
            Session["cart"] = cart;

            // Return partial view with model
            return PartialView(model);
        }

        [Authorize]
        // GET: /Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;


            // Get cartVM from list
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            // Increment qty
            model.Quantity++;

            // Store needed data
            var result = new { qty = model.Quantity, price = model.Price };

            // Return json with data
            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [Authorize]
        // GET: /Cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            // Init cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;


            // Get model from list
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            // Decrement qty
            if (model.Quantity > 1)
            {
                model.Quantity--;
            }
            else
            {
                model.Quantity = 0;
                cart.Remove(model);
            }

            // Store needed data
            var result = new { qty = model.Quantity, price = model.Price };

            // Return json
            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [Authorize]
        // GET: /Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            // Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;


            // Get model from list
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            // Remove model from list
            cart.Remove(model);


        }

        public ActionResult PaypalPartial()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            return PartialView(cart);
        }

        public ActionResult Checkout()
        {
            Session["cart"] = null;
            return View();
        }

        // POST: /Cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            // Get cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Get username
            string username = User.Identity.Name;

            int orderId = 0;

            using (IdentityDbContext dab = new IdentityDbContext())
            {
                // Init OrderDTO
                Order orderDTO = new Order();

                // Get user id
                var q = dab.Users.FirstOrDefault(x => x.UserName == username);
                string userId = q.Id;

                // Add to Orders and save
                orderDTO.UserId = userId;
                orderDTO.CreatedAt = DateTime.Now;

                db.Orders.Add(orderDTO); 
               
                dab.SaveChanges();
                
                // Get inserted id
                orderId = orderDTO.Id;

                // Init OrderDetailsDTO
                OrderDetails orderDetailsDTO = new OrderDetails();

                // Add to OrderDetails
                foreach (var item in cart)
                {
                    orderDetailsDTO.OrderId = orderId;
                    orderDetailsDTO.UserId = userId; 
                    orderDetailsDTO.ProductId = item.ProductId;
                    orderDetailsDTO.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDTO); 

                    dab.SaveChanges();
                    db.SaveChanges();
                }
            }

           

            // Reset session
            Session["cart"] = null;

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
