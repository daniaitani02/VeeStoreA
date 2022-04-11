using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VeeStoreA.Models;

namespace VeeStoreA.Controllers
{
    public class HomeController : Controller
    {
        private VeeStoreDbEntities db = new VeeStoreDbEntities();
        public ActionResult Index()
        {
       
            return View();
            
        }

        public ActionResult FAQ(string faqSearchString)
        {
            ViewBag.Message = "Your application description page.";
                        
            var faqs = from f in db.Faqs
                       select f;
            if (!String.IsNullOrEmpty(faqSearchString))
            {
                faqs = faqs.Where(f => f.Question.Contains(faqSearchString));

            }
            ViewBag.faqsearch = faqSearchString;
            return View(faqs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FaqCreate([Bind(Include = "Email,Name,Question")] Faq faq)
        {
            if (ModelState.IsValid)
            {
                faq.Status = "Awaiting Aproval";
                db.Faqs.Add(faq);
                db.SaveChanges();
                return RedirectToAction("FAQ");
            }

            //ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", cart.CustomerName);
            return View(faq);
        }
        public ActionResult AdminPanel()
        {

            if(User.Identity.Name != "admin@admin.com") return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            ViewBag.totalCustomers = db.Customers.Count();
            ViewBag.totalProducts = db.Products.Count();
            try {
                ViewBag.totalEarnings = db.Carts.Where(c => c.Status == "Paid").Sum(c => c.CartItems.Sum(item => item.Quantity * item.Product.Price));
                ViewBag.totalPaidCarts = db.Carts.Where(c => c.Status == "Paid").Count();
            } catch
            {
                ViewBag.totalEarnings = 0;
                ViewBag.totalPaidCarts = 0;
            }
            


            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}