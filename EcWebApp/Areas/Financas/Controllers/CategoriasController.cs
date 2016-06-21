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

namespace EcWebApp.Areas.Financas.Controllers
{
    [Authorize]
    public class CategoriasController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        // GET: Financas/Categorias
        public ActionResult Index()
        {
            return View(db.Categorias.ToList());
        }

        // GET: Financas/Categorias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaLancamentoInfo categoriaLancamentoInfo = db.Categorias.Find(id);
            if (categoriaLancamentoInfo == null)
            {
                return HttpNotFound();
            }
            return View(categoriaLancamentoInfo);
        }

        // GET: Financas/Categorias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Financas/Categorias/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdCategoria,Descricao,TipoLancamento,Ativo")] CategoriaLancamentoInfo categoriaLancamentoInfo)
        {
            if (ModelState.IsValid)
            {
                db.Categorias.Add(categoriaLancamentoInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categoriaLancamentoInfo);
        }

        // GET: Financas/Categorias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaLancamentoInfo categoriaLancamentoInfo = db.Categorias.Find(id);
            if (categoriaLancamentoInfo == null)
            {
                return HttpNotFound();
            }
            return View(categoriaLancamentoInfo);
        }

        // POST: Financas/Categorias/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdCategoria,Descricao,TipoLancamento,Ativo")] CategoriaLancamentoInfo categoriaLancamentoInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoriaLancamentoInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categoriaLancamentoInfo);
        }

        // GET: Financas/Categorias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaLancamentoInfo categoriaLancamentoInfo = db.Categorias.Find(id);
            if (categoriaLancamentoInfo == null)
            {
                return HttpNotFound();
            }
            return View(categoriaLancamentoInfo);
        }

        // POST: Financas/Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoriaLancamentoInfo categoriaLancamentoInfo = db.Categorias.Find(id);
            db.Categorias.Remove(categoriaLancamentoInfo);
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
