using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VeeStoreA.Models;

namespace VeeStoreA.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private VeeStoreDbEntities db = new VeeStoreDbEntities();
        [AllowAnonymous]
        // GET: Transaction
        public ActionResult Index()
        {
            //This controller is only for helper methods so there is no index and no authentication. Simply redirect to home page
            return RedirectToAction("Index", "Home");
        }
        // This method will return the unpaid cart of the current logged-in user. If the customer or the cart does not exist, both will be created and returned
        private Cart GetUsersCart()
        {

            string loggedInEmail = User.Identity.Name; // Get the logged in user email
            DateTime currentTime = DateTime.Now;
            // Does the customer exist in the customers table?
            if (db.Customers.Count(c => c.Email.Equals(loggedInEmail)) == 0) // If the count is zero this means there is no customer so we add the customer
            {

                // Adding the new customer
                db.Customers.Add(new Customer { Email = loggedInEmail, Name = loggedInEmail.Split('@')[0],JoinedAt= currentTime,CurrencyId=1
            });
                db.SaveChanges();
            }
            Cart cart = null;
            try
            {
                // Check if the customer has an unpaid cart
                cart = db.Carts.First(c => c.CustomerEmail.Equals(loggedInEmail) && c.Status.Equals("Unpaid"));
            }
            catch (Exception)
            {   // If not, create a new one
                cart = new Cart { CustomerEmail = loggedInEmail, Status = "Unpaid" };
                cart.CreatedAt = currentTime;
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            return cart;
        }
        // This method will add the given product to the cart of the logged in user.
        public ActionResult AddToCart(int? id)
        {
            if (id == null) // Is the id of the product provided as a parameter?
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Return BadRequest error if id is not provided (null)
            }
            Product product = db.Products.Find(id); // Is there a product with the given id?

            if (product == null) { return HttpNotFound(); } // If the product does not exist (null) return NotFound error


            Cart cart = GetUsersCart();// Get the unpaid Cart of the logged in customer


            try
            {
                // Check if the item is already in the cart
                CartItem cartitem = db.CartItems.First(c => (c.CartId == cart.Id && c.ProductId == product.Id));
                cartitem.Quantity++; // Increment quantity if item is in cart
            }
            catch (Exception)
            {
                // If item is not in cart, add it.
                CartItem cartitem = new CartItem { ProductId = product.Id, CartId = cart.Id, Quantity = 1,AddedAt= DateTime.Now };
                db.CartItems.Add(cartitem);
            }
            db.SaveChanges();

            return RedirectToAction("Details", "Carts", new { id = cart.Id });  // Finally, take the user to their cart.

        }

        public ActionResult MyCart()
        {
            // Get the unpaid cart of the logged in user
            Cart cart = GetUsersCart();
            // Redirect user to their unpaid cart
            return RedirectToAction("Details", "Carts", new { id = cart.Id });
        }
        public ActionResult Checkout(int? id,String deliveryMethod)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            if (cart.Customer.Email != User.Identity.Name && User.Identity.Name != "admin@admin.com")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (cart.CartItems.Count() == 0)
            {
                TempData["error"] = "Cart Is Empty";
                return RedirectToAction("Details", "Carts", new { id = id });

            }
           
            //Check Stock Availability
            foreach (CartItem cartItem in cart.CartItems)
            {
                if(cartItem.Product.CardCodes.Where(cc=>cc.Status=="New").Count() < cartItem.Quantity)
                {
                    TempData["error"] = "There are not enough codes in stock for " + cartItem.Product.Name;
                    return RedirectToAction("Details", "Carts", new { id = id });
                }
            }
            //Flag Cart As Paid
            cart.Status = "Paid";
            cart.PaidAt = DateTime.Now;
            db.SaveChanges();

            //Flag Each Product As Used And Deliver
            foreach (CartItem cartItem in cart.CartItems)
            {
                CardCode cardCode = cartItem.Product.CardCodes.Where(cc => cc.Status == "New").First();
                cardCode.Status = "Used";
                cardCode.UsedAt = DateTime.Now;
                cardCode.CustomerEmail = cart.CustomerEmail;
                db.SaveChanges();
                TempData["error"] = "Your Code Is: " + cardCode.Code;

            }

           

            
            return RedirectToAction("Details", "Carts", new { id = id });


        }
        public ActionResult Apply(string code)
        {
            IEnumerable<CouponCode> couponCodes = db.CouponCodes.Where(c=>c.Code==code);
           
            Cart cart = GetUsersCart();
       
            if (couponCodes.Count() == 0)
            {
                TempData["error"] = "Coupoun Code is not valid";

            }
            else
            {
                CouponCode couponCode = couponCodes.First();
                cart.CouponCodeId = couponCode.Id;
                db.SaveChanges();
                TempData["info"] = "Applied Coupon Code";
            }
            


            return RedirectToAction("Details","Carts", new { id = cart.Id });

        }

    }
}