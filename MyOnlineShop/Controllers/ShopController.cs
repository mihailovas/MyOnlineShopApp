using MyOnlineShop.Models;
using MyOnlineShop.Models.ViewModel;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyOnlineShop.Controllers
{
    
    public class ShopController : Controller
    {
        private MyOnlineShopDb db = new MyOnlineShopDb();

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer == null ||
                        filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
            {
                filterContext.Result = new RedirectToRouteResult(new
                               RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
            }
        }
    }


    // GET: Shop
    public ActionResult Index()
        {
            return RedirectToAction("Products", "Shop");
        }


        // GET: Shop/Categories
        public ActionResult Categories()
        {
            //Declare a list of models
            List<CategoryVM> categoryVMList;



            //init the list

            categoryVMList = db.Categories
                             .ToArray()
                             .Select(x => new CategoryVM(x))
                             .ToList();


            //Return view with list
            return View(categoryVMList);
        }
        
        [Authorize(Roles ="Admin")]
        //GET: Shop/Categories/AddNewCategory
        [HttpGet]
        public ActionResult AddNewCategory()
        {
            return View();
        }


        //POST: Shop/AddNewCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewCategory([Bind(Include = "Id,Name,CategoryURL")] Category category)
        {
            if (ModelState.IsValid)
            {


                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Categories");

            }
            TempData["SM"] = "You have added a category!";
            return View(category);
        }


        //GET: /Shop/Category/Name
        public ActionResult Category(String name)
        {
            //declare a list of productVM
            List<ProductVM> productVMlist;

            //Get category id
            
        Category catDto = db.Categories.Where(x => x.Name == name).FirstOrDefault();
            if (catDto == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            int catId = catDto.Id;

            //init the list
            productVMlist = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();


            //get category name
           
            var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
            ViewBag.CategoryName = productCat.CategoryName;


            //return view with list
            return View(productVMlist);
        }

        [Authorize(Roles ="Admin")]
        // GET: Shop/EditCategory/id
        public ActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Shop/EditCategory/id
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory([Bind(Include = "Id,Name,CategoryURL")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Categories");
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        // GET: Categories/Delete/5
        public ActionResult DeleteCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = db.Categories.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);

        }


        // POST: Categories/Delete/5
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Categories");
        }

        [Authorize(Roles = "Admin")]
        //GET: Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Init model
            ProductVM model = new ProductVM();
            //Add select list of categories to model

            model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            return View(model);
        }

        //POST: Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model)
        {
            //Check model state
            if (!ModelState.IsValid)
            {

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                return View(model);

            }
            //Make sure product name is unique

            if (db.Products.Any(x => x.Name == model.Name))
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                ModelState.AddModelError("", "That product name is taken!");
                return View(model);
            }



            //Declare product id
            int id;

            //init and save Product DataObject

            Product product = new Product();
            product.Name = model.Name;
            product.Price = model.Price;
            product.Description = model.Description;
            product.ProductURL = model.ProductURL;
            product.CategoryId = model.CategoryId;


            Category catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
            product.CategoryName = catDTO.Name;

            db.Products.Add(product);
            db.SaveChanges();

            //Get id
            id = product.Id;




            //Set TempData message
            TempData["SM"] = "You have added a product!";
            //Redirect
            return RedirectToAction("AddProduct");
        }



        [NoDirectAccess]
        //GET: Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            //Declare a list of ProductVM
            List<ProductVM> listOfProductVM;

            //Set page number
            var pageNumber = page ?? 1;


            //Init list
            listOfProductVM = db.Products
                               .ToArray()
                               .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                               .Select(x => new ProductVM(x))
                               .ToList();

            //populate category select list 
            ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            //set selected category
            ViewBag.SelectedCat = catId.ToString();





            //return view with list

            return View(listOfProductVM);
        }

        [Authorize(Roles = "Admin")]
        //GET: Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            //declare productVM
            ProductVM model;

            //get the product
            Product dto = db.Products.Find(id);

            //make sure product exists
            if (dto == null)
            {
                return Content("That product does not exist!");
            }

            //init model
            model = new ProductVM(dto);

            //make a select list 
            model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");


            //return view  with a model
            return View(model);
        }

        ////POST: Shop/EditProduct/id
        //[HttpPost]
        //public ActionResult EditProduct(ProductVM model)
        //{
        //    //Get product id
        //    int id = model.Id;
        //    //populate categories select list

        //    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

        //    //check model state
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    //make sure product name unique

        //    if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
        //    {
        //        ModelState.AddModelError("", "That product name is taken!");
        //        return View(model);
        //    }

        //    //update product
        //    try { 
        //    Product dto = db.Products.Find(id);

        //    dto.Name = model.Name;
        //    dto.Price =model.Price;
        //    dto.ProductURL = model.ProductURL;
        //    dto.Description = model.Description;
        //    dto.CategoryId = model.CategoryId;

        //    Category catdto = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
        //    dto.CategoryName = catdto.Name;

        //    db.SaveChanges();



        //    //set tempdata
        //    TempData["SM"] = "You have edited the product!";

        //    //redirect 
        //    return RedirectToAction("EditProduct");
        //    }
        //    catch(Exception e)
        //    {
        //        return HttpNotFound();
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct([Bind(Include = "Id,Name,Description,Price,CategoryName,CategoryId,ProductURL")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Products");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            //set tempdata
            TempData["SM"] = "You have edited the product!";

           //redirect 
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        // GET: Shop/DeleteProduct/id
        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ProductVM model = new ProductVM(product);
            return View(model);
        }

        // POST: Shop/DeleteProduct/id
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public ActionResult ProductDeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Products");
        }


        // GET: /shop/product-details/name
        [Authorize]
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            // Declare the VM and DTO
            ProductVM model;
            Product dto;

            // Init product id
            int id = 0;


            // Check if product exists
            if (!db.Products.Any(x => x.Name.Equals(name)))
            {
                return RedirectToAction("Index", "Shop");
            }

            // Init productDTO
            dto = db.Products.Where(x => x.Name == name).FirstOrDefault();

            // Get id
            id = dto.Id;

            // Init model
            model = new ProductVM(dto);




            // Return view with model
            return View("ProductDetails", model);
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