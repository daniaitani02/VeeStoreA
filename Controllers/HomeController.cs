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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult AdminPanel()
        {

            if(User.Identity.Name != "admin@admin.com") return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            ViewBag.totalCustomers = db.Customers.Count();
            ViewBag.totalProducts = db.Products.Count();
            ViewBag.totalEarnings = db.Carts.Where(c => c.Status == "Paid").Sum(c => c.CartItems.Sum(item => item.Quantity * item.Product.Price));
            ViewBag.totalPaidCarts = db.Carts.Where(c => c.Status == "Paid").Count();


            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}