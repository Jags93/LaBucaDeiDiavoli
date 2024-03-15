using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LaBucaDeiDiavoli.Models;
using System.Threading.Tasks;
using Antlr.Runtime.Misc;

namespace LaBucaDeiDiavoli.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdiniController : Controller
    {
        private ModelDbContext db = new ModelDbContext();
        [Authorize(Roles="Admin")]
        public ActionResult Create()
        {
            ViewBag.Articoli = new SelectList(db.Articoli, "IdArticolo", "Nome");
            ViewBag.IdUtente = new SelectList(db.Utenti, "IdUtente", "Username");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles="Admin")]

        public ActionResult Create(Ordini ordini, int IDArticolo, int Qauntita)
        {
            if(ModelState.IsValid)
            {
                db.Ordini.Add(ordini);
                db.SaveChanges();

                ordini.DataOrdine = DateTime.Now;
                ordini.IsEvaso = false;


                var articolo = db.Articoli.Find(IDArticolo);
                if(articolo== null)
                {
                    return HttpNotFound();
                }   

                var totPrezzo = articolo.Prezzo * Qauntita;


                var dettagliOrdini = new DettagliOrdini
                {
                    IdArticolo = IDArticolo,
                    IdOrdine = ordini.IdOrdine,
                    Quantita = Qauntita,
                    PrezzoTotale = totPrezzo
                };

                db.DettagliOrdini.Add(dettagliOrdini);
                db.SaveChanges();
                return RedirectToAction("Index");

               
            }

            ViewBag.IdUtente = new SelectList(db.Utenti, "IdUtente", "Username", ordini.IdUtente);
            return View(ordini);
        }

        [Authorize(Roles="User")]
        public ActionResult Checkout()
        {
            var user = db.Utenti.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Utente non trovato");
            }

            var order = db.Ordini.Include(o => o.DettagliOrdini.Select(d => d.Articoli)).SingleOrDefault(o => o.IdUtente == user.IdUtente && !o.IsEvaso);
            if (order == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ordine non trovato");
            }

            return View(order);
        }

        [Authorize(Roles="User")]
        public ActionResult CheckoutConfirmed()
        {
            var user = db.Utenti.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Utente non trovato");
            }

            var order = db.Ordini.Include(o => o.DettagliOrdini.Select(d => d.Articoli)).SingleOrDefault(o => o.IdUtente == user.IdUtente && !o.IsEvaso);
            if (order == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ordine non trovato");
            }

            order.IsEvaso = true;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Articoli");
        }

        [HttpPost]
        [Authorize(Roles="User")]
       public ActionResult RemoveFromCart(int id)
        {
            var user = db.Utenti.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Utente non trovato");
            }

            var order = db.Ordini.Include(o => o.DettagliOrdini).SingleOrDefault(o => o.IdUtente == user.IdUtente && !o.IsEvaso);
            if (order == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ordine non trovato");
            }

            var orderDetail = order.DettagliOrdini.FirstOrDefault(d => d.IdDettagliOrdine == id);
            if (orderDetail == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Dettaglio ordine non trovato");
            }

            db.DettagliOrdini.Remove(orderDetail);
            db.SaveChanges();

            return RedirectToAction("Checkout");
        }


        [Authorize(Roles="User")]

        public ActionResult UserDetalis(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
        }

        [HttpPost]
        [Authorize(Roles="Admin")]

        public ActionResult EvasioneOrdine(int id)
        {
            var ordine = db.Ordini.Find(id);
            if (ordine == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ordine non trovato");
            }

            ordine.IsEvaso = true;
            db.Entry(ordine).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // GET: Ordini
        public ActionResult Index()
        {
            var ordini = db.Ordini.Include(o => o.Utenti);
            return View(ordini.ToList());
        }

        [Authorize(Roles="User")]
      
        

        public async Task<ActionResult> GetNumeroOrdini()
        {
            int totale = await db.Ordini.Where(o => o.IsEvaso == true).CountAsync();
            return Json(totale, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> IncassatoPerGiorno(DateTime data)
        {
            decimal incasso = await db.Ordini
                .Where(o => o.DataOrdine.Year == data.Year && o.DataOrdine.Month == data.Month && o.DataOrdine.Day == data.Day)
                .SelectMany(o => o.DettagliOrdini)
                .SumAsync(a => a.PrezzoTotale);
            return Json(incasso, JsonRequestBehavior.AllowGet);
        }

       
    }
}

        

  

