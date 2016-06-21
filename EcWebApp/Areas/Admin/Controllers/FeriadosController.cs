using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EcWebApp.DAL;
using EcWebApp.Models;
using EcWebApp.Controllers;

namespace EcWebApp.Areas.Admin.Controllers
{
    [Authorize]
    public class FeriadosController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        // GET: Admin/Feriados
        public ActionResult Index()
        {
            return View(db.Feriados.ToList());
        }

        // GET: Admin/Feriados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeriadoInfo feriadoInfo = db.Feriados.Find(id);
            if (feriadoInfo == null)
            {
                return HttpNotFound();
            }
            return View(feriadoInfo);
        }

        // GET: Admin/Feriados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Feriados/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdFeriado,DataFeriado,Descricao")] FeriadoInfo feriadoInfo)
        {
            if (ModelState.IsValid)
            {
                db.Feriados.Add(feriadoInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(feriadoInfo);
        }

        // GET: Admin/Feriados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeriadoInfo feriadoInfo = db.Feriados.Find(id);
            if (feriadoInfo == null)
            {
                return HttpNotFound();
            }
            return View(feriadoInfo);
        }

        // POST: Admin/Feriados/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdFeriado,DataFeriado,Descricao")] FeriadoInfo feriadoInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feriadoInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(feriadoInfo);
        }

        // GET: Admin/Feriados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeriadoInfo feriadoInfo = db.Feriados.Find(id);
            if (feriadoInfo == null)
            {
                return HttpNotFound();
            }
            return View(feriadoInfo);
        }

        // POST: Admin/Feriados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FeriadoInfo feriadoInfo = db.Feriados.Find(id);
            db.Feriados.Remove(feriadoInfo);
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
