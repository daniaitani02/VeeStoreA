using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VeeStoreA.Models;

namespace VeeStoreA.Controllers
{
    public class FaqsController : Controller
    {
        private VeeStoreDbEntities db = new VeeStoreDbEntities();

        // GET: Faqs
        public ActionResult Index(string faqSearchString)
        {
           

            var faqs = from f in db.Faqs
                       select f;
            faqs = faqs.Where(f => f.Status == "Approved");

            if (!String.IsNullOrEmpty(faqSearchString))
            {
                faqs = faqs.Where(f => f.Question.Contains(faqSearchString));
            }
            IEnumerable<Faq> faqItems = faqs;
            ViewBag.faqItems = faqItems;
            ViewBag.faqsearch = faqSearchString;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id,Question,Answer,Email,Name")] Faq faq)
        {
            if (ModelState.IsValid)
            {
                faq.Status = "Awaiting Approval";
                db.Faqs.Add(faq);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var faqs = from f in db.Faqs
                       select f;
            IEnumerable<Faq> faqItems = faqs;
            ViewBag.faqItems = faqItems;
     
            return View(faq);
        }
        // GET: Faqs/Details/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faq faq = db.Faqs.Find(id);
            if (faq == null)
            {
                return HttpNotFound();
            }
            return View(faq);
        }

        // GET: Faqs/Create
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Faqs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Create([Bind(Include = "Id,Question,Answer,Email,Name")] Faq faq)
        {
            if (ModelState.IsValid)
            {
                db.Faqs.Add(faq);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(faq);
        }

        // GET: Faqs/Edit/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faq faq = db.Faqs.Find(id);
            if (faq == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(new List<string> { "Approved", "Awaiting Approval" },faq.Status);
            return View(faq);
        }

        // POST: Faqs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Edit([Bind(Include = "Id,Question,Answer,Status,Email,Name")] Faq faq)
        {
            if (ModelState.IsValid)
            {
                db.Entry(faq).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Faq","Admin");
            }
            ViewBag.Status = new SelectList(new List<string> { "Approved", "Awaiting Approval" }, faq.Status);

            return View(faq);
        }

        // GET: Faqs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faq faq = db.Faqs.Find(id);
            if (faq == null)
            {
                return HttpNotFound();
            }
    
            db.Faqs.Remove(faq);
            db.SaveChanges();
            return RedirectToAction("Faq", "Admin");

        }

        // POST: Faqs/Delete/5
       


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
