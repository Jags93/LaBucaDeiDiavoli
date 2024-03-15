using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LaBucaDeiDiavoli.Models;

namespace LaBucaDeiDiavoli.Controllers
{
    [Authorize]
    public class ArticoliController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        

        // GET: Articoli
        public ActionResult Index()
        {
            var isUserAdmin = User.IsInRole("Admin");
            ViewBag.IsUserAdmin = isUserAdmin;
            return View(db.Articoli.ToList());
        }

        // GET: Articoli/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        //[Authorize(Roles="Admin")]

        // GET: Articoli/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Articoli/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult Create(Articoli articoli, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".png" && extension != ".gif" && extension != ".jpeg")
                    {
                        ModelState.AddModelError("", "Il file caricato non è un'immagine valida. Sono accettate solo immagini .jpg, .png, .gif, .jpeg");
                        return View(articoli);
                    }

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/image"), fileName);
                    file.SaveAs(path);
                    articoli.Immagine = fileName;
                }
                db.Articoli.Add(articoli);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(articoli);
        }


       // [Authorize(Roles="Admin")]

        // GET: Articoli/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        // POST: Articoli/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdArticolo,Nome,Immagine,Prezzo,TempoConsegna")] Articoli articoli)
        {
            if (ModelState.IsValid)
            {
                db.Entry(articoli).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(articoli);
        }


        [Authorize(Roles="Admin")]
        // GET: Articoli/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        // POST: Articoli/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Articoli articoli = db.Articoli.Find(id);
            db.Articoli.Remove(articoli);
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

        
        

        //[Authorize(Roles="User")]
        [HttpPost]
        
        public ActionResult AddToCart(int productId, int quantity)
        {
           
            var user = db.Utenti.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Utente non trovato");
            }

            
            var order = db.Ordini.SingleOrDefault(o => o.IdUtente == user.IdUtente && !o.IsEvaso);

          
            if (order == null)
            {
                order = new Ordini { IdUtente = user.IdUtente, IsEvaso = false, DettagliOrdini = new List<DettagliOrdini>(), DataOrdine = DateTime.Now };
                db.Ordini.Add(order);
            }

           
            var orderDetail = order.DettagliOrdini.FirstOrDefault(d => d.Articoli.IdArticolo == productId);

            if (orderDetail != null)
            {
                orderDetail.Quantita += quantity;
                orderDetail.PrezzoTotale = orderDetail.Articoli.Prezzo * orderDetail.Quantita;

            }
            else
            {
               
                var product = db.Articoli.Find(productId);
                if (product == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Prodotto non trovato");
                }
                order.DettagliOrdini.Add(new DettagliOrdini { Articoli = product, Quantita = quantity, PrezzoTotale = product.Prezzo * quantity });
            }

            try
            {
                db.SaveChanges();
                TempData["SuccessMessage"] = "Prodotto aggiunto al carrello con successo!";
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                TempData["ErrorMessage"] = "Errore durante il salvataggio dei dati";
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles="User")]
        public ActionResult Cart()
        {
            // Recupera il carrello dalla sessione
            var cart = Session["cart"] as List<Articoli> ?? new List<Articoli>();

            return View(cart);
        }





    }
}
