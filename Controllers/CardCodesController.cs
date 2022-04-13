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
    [Authorize(Users = "admin@admin.com")]
    public class CardCodesController : Controller
    {
        private VeeStoreDbEntities db = new VeeStoreDbEntities();

        // GET: CardCodes
        public ActionResult Index()
        {
            var cardCodes = db.CardCodes.Include(c => c.Product);
            return View(cardCodes.ToList());
        }

        // GET: CardCodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardCode cardCode = db.CardCodes.Find(id);
            if (cardCode == null)
            {
                return HttpNotFound();
            }
            return View(cardCode);
        }

        // GET: CardCodes/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            return View();
        }

        // POST: CardCodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductId,Code")] CardCode cardCode)
        {
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", cardCode.ProductId);

            if (ModelState.IsValid)
            {
                if (db.CardCodes.Where(x => x.Code == cardCode.Code).Count() != 0)
                {
                    TempData["errorAdminCardC"] = "You cannot create a card code that is already present";
                    return View();
                }
                DateTime now = DateTime.Now;
                cardCode.CreatedAt = now;
                cardCode.Status = "New";
                db.CardCodes.Add(cardCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View(cardCode);
        }

        // GET: CardCodes/Edit/5
        public ActionResult Edit(int? id)
        {

            ViewBag.canEdit = true;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardCode cardCode = db.CardCodes.Find(id);
            if (cardCode == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", cardCode.ProductId);

            if (cardCode.Status == "Used")
            {
                TempData["errorAdminCardC"] = "You cannot edit or delete this card because it has been used";
                ViewBag.canEdit = false;
                return RedirectToAction("Index");
            }
            return View(cardCode);
        }

        // POST: CardCodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductId,Code,Status,CreatedAt,UsedAt")] CardCode cardCode)
        {
            ViewBag.canEdit = true;

            if (ModelState.IsValid)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", cardCode.ProductId);

                if (db.CardCodes.Where(x => x.Code == cardCode.Code && cardCode.Id != x.Id ).Count() != 0)
                {
                    TempData["errorAdminCardC"] = "You cannot edit a card code that is already present in another record";
                    return View();
                }

                db.Entry(cardCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(cardCode);
        }

        // GET: CardCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardCode cardCode = db.CardCodes.Find(id);
            if (cardCode == null || cardCode.Status == "Used")
            {
                TempData["errorAdminCardC"] = "You cannot edit or delete this card because it has been used";
                return RedirectToAction("Index");
            }
            db.CardCodes.Remove(cardCode);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: CardCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CardCode cardCode = db.CardCodes.Find(id);
            db.CardCodes.Remove(cardCode);
            db.SaveChanges();
            return RedirectToAction("Index");
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
