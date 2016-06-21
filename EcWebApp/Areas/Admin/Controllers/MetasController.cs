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
    public class MetasController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        // GET: Admin/Metas
        public ActionResult Index()
        {
            return View(db.Metas.OrderByDescending(o=> o.Ano).ThenByDescending(t=> t.Mes).ToList());
        }

        // GET: Admin/Metas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MetaInfo metaInfo = db.Metas.Find(id);
            if (metaInfo == null)
            {
                return HttpNotFound();
            }
            return View(metaInfo);
        }

        // GET: Admin/Metas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Metas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdMeta,Mes,Ano,ValorMeta,ValorDesafio")] MetaInfo metaInfo)
        {
            if (ModelState.IsValid)
            {
                db.Metas.Add(metaInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(metaInfo);
        }

        // GET: Admin/Metas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MetaInfo metaInfo = db.Metas.Find(id);
            if (metaInfo == null)
            {
                return HttpNotFound();
            }
            return View(metaInfo);
        }

        // POST: Admin/Metas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdMeta,Mes,Ano,ValorMeta,ValorDesafio")] MetaInfo metaInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(metaInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(metaInfo);
        }

        // GET: Admin/Metas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MetaInfo metaInfo = db.Metas.Find(id);
            if (metaInfo == null)
            {
                return HttpNotFound();
            }
            return View(metaInfo);
        }

        // POST: Admin/Metas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MetaInfo metaInfo = db.Metas.Find(id);
            db.Metas.Remove(metaInfo);
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
