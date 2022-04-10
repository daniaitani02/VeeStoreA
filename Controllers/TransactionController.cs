using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
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
                db.Customers.Add(new Customer
                {
                    Email = loggedInEmail,
                    Name = loggedInEmail.Split('@')[0],
                    JoinedAt = currentTime,
                    CurrencyId = 1
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
                CartItem cartitem = new CartItem { ProductId = product.Id, CartId = cart.Id, Quantity = 1, AddedAt = DateTime.Now };
                db.CartItems.Add(cartitem);
            }
            db.SaveChanges();

            return RedirectToAction("Details", "Carts", new { id = cart.Id });  // Finally, take the user to their cart.

        }
        public ActionResult DecrementItem(int? id)

        {
            CartItem cartitem = db.CartItems.Find(id);
            if(cartitem.Quantity == 1)
            {
                db.CartItems.Remove(cartitem);
                db.SaveChanges();

            }
            else
            {
                cartitem.Quantity -= 1;

                db.SaveChanges();
            }
          
            Cart cart = GetUsersCart();
            // Redirect user to their unpaid cart
            return RedirectToAction("Details", "Carts", new { id = cart.Id });

        
        }
        public ActionResult IncrementItem(int? id)
        {
           
            CartItem cartitem = db.CartItems.Find(id);
            cartitem.Quantity += 1;
           
                db.SaveChanges();
            Cart cart = GetUsersCart();
            // Redirect user to their unpaid cart
            return RedirectToAction("Details", "Carts", new { id = cart.Id });
            
        }
        public ActionResult DeleteItem(int? id)

        {
            CartItem cartitem = db.CartItems.Find(id);
            db.CartItems.Remove(cartitem);
            db.SaveChanges();

            Cart cart = GetUsersCart();

            // Redirect user to their unpaid cart
            return RedirectToAction("Details", "Carts", new { id = cart.Id });


        }
        public ActionResult MyCart()
        {

            // Get the unpaid cart of the logged in user
            Cart cart = GetUsersCart();
            // Redirect user to their unpaid cart
            return RedirectToAction("Details", "Carts", new { id = cart.Id });
        }

        public ActionResult Receipt(int? id)
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
            return View(cart);
        }
        public async Task<ActionResult> Checkout(int? id, string deliveryMethod, int creditCardId)
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
                if (cartItem.Product.CardCodes.Where(cc => cc.Status == "New").Count() < cartItem.Quantity)
                {
                    TempData["error"] = "There are not enough codes in stock for " + cartItem.Product.Name;
                    return RedirectToAction("Details", "Carts", new { id = id });
                }
            }
            //Flag Cart As Paid
            cart.Status = "Paid";
            cart.PaidAt = DateTime.Now;
            cart.CreditCardId = creditCardId;

            db.SaveChanges();
            var recieptTable = "";
            var whatsappCodeTable = "";
            string text = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/Reciept_Template.txt"));
            var currecnySymbol = cart.Customer.Currency.Symbol;
            var currecnyMutliplier = cart.Customer.Currency.Multiplier;
            var totalCart = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price) * currecnyMutliplier;
            var discountAmount = cart.CouponCode != null ? (totalCart * ((double)cart.CouponCode.DiscountPercentage / 100)) : 0;
            var cardInfo = cart.CreditCard.Name + " (XXXX XXXX " + cart.CreditCard.Number.ToString().Substring(cart.CreditCard.Number.ToString().Length - 4) + ")";
            //Flag Each Product As Used And Deliver
            foreach (CartItem cartItem in cart.CartItems)
            {
                foreach (int value in Enumerable.Range(1, cartItem.Quantity))
                {


                    CardCode cardCode = cartItem.Product.CardCodes.Where(cc => cc.Status == "New").First();
                    cardCode.Status = "Used";
                    cardCode.UsedAt = DateTime.Now;
                    cardCode.CartId = cart.Id;
                    db.SaveChanges();

                    recieptTable += @"<tr>
                                    <td style=""font-family: 'Montserrat',Arial,sans-serif; font-size: 14px; padding-top: 10px; padding-bottom: 10px; width: 80%; "" width=""80%"">" + cardCode.Product.Name + @"
                                       <br>Code: <strong style=""color:blue;"">" + cardCode.Code + @" </strong ></td >
                                    <td align = ""right"" style = ""font-family: 'Montserrat',Arial,sans-serif; font-size: 14px; text-align: right; width: 20%;"" width = ""20%"">" + (cartItem.Product.Price * currecnyMutliplier).ToString() + currecnySymbol + "</td></tr>";
                   whatsappCodeTable += @"*PlayStation 10$ Gift Card*
- *Code:* _"+cardCode.Code+@"_
- *Price:* _"+ (cartItem.Product.Price * currecnyMutliplier).ToString() + currecnySymbol + @"_


";

                }

            }

            //TempData["receiptTable"] = recieptTable;
            if (deliveryMethod == "emailDelivery")
            {
                text = text.Replace("Customer_Name!", cart.Customer.Name);
                text = text.Replace("Paid_At", cart.PaidAt.ToString());
                text = text.Replace("Cart_Id", cart.Id.ToString());
                text = text.Replace("Amount_Paid", ((totalCart - discountAmount) * currecnyMutliplier).ToString() + currecnySymbol);
                text = text.Replace("Discount_Amount", discountAmount.ToString());
                text = text.Replace("Reciept_Table", recieptTable);

                SendEmail("Sp0derDev@protonmail.com", text, "VeeStore Reciept");
            }
            else if (deliveryMethod == "phoneDelivery")
            {
                var whatsappMessage = @"Hello, *" + cart.Customer.Name + @"* 👋

Thank you for using VeeStore.
This is a receipt for your recent purchase. 💵 


*Order #:* " + cart.Id.ToString() + @"
*Credit Card:* " + cardInfo + @"
*Paid At:* " + cart.PaidAt.ToString() + @"
*Discount:* -" + discountAmount + currecnySymbol + @"
*Amount Paid:* " + (((totalCart - discountAmount) * currecnyMutliplier).ToString() + currecnySymbol) + @"


"+whatsappCodeTable;
                await SendWhatsapp(whatsappMessage, cart.Customer.PhoneNumber);
            }


            return RedirectToAction("Receipt", new { id = id });


        }
        public ActionResult Apply(string code)
        {
            IEnumerable<CouponCode> couponCodes = db.CouponCodes.Where(c => c.Code == code);

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



            return RedirectToAction("Details", "Carts", new { id = cart.Id });

        }


        private void SendEmail(String email, String message, String subject)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("veestore.info@gmail.com", "Vvvvvv1!"),
                EnableSsl = true,
                Timeout = 10000
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress("veestore.info@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);

        }

        private async Task SendWhatsapp(string message, string number)
        {

            HttpClient client = new HttpClient();
            var values = new Dictionary<string, string>{
                      { "text", message},
                      { "phoneNumber", "974"+number.Replace("974","").Replace(" ","").Replace("+","") }
                  };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://api.sp0der.me/WhatsAppAPI/sendText", content);

            //var responseString = await response.Content.ReadAsStringAsync();

        }


    }
}