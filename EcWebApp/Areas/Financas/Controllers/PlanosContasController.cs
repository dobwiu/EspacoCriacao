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

namespace EcWebApp.Areas.Financas.Controllers
{
    public class PlanosContasController : Controller
    {
        private EspacoContext db = new EspacoContext();

        // GET: Financas/PlanosContas
        public ActionResult Index()
        {
            var contas = db.Contas.Include(p => p.Usuario);
            return View(contas.ToList());
        }

        // GET: Financas/PlanosContas/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanoContaInfo planoContaInfo = db.Contas.Find(id);
            if (planoContaInfo == null)
            {
                return HttpNotFound();
            }
            return View(planoContaInfo);
        }

        // GET: Financas/PlanosContas/Create
        public ActionResult Create()
        {
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "NomeUsuario");
            return View();
        }

        // POST: Financas/PlanosContas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdConta,Descricao,DataAberturaConta,SaldoInicial,SaldoAtual,DataUltimaAtualizacao,Ativo,IdUsuario")] PlanoContaInfo planoContaInfo)
        {
            if (ModelState.IsValid)
            {
                planoContaInfo.IdConta = Guid.NewGuid();
                db.Contas.Add(planoContaInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "NomeUsuario", planoContaInfo.IdUsuario);
            return View(planoContaInfo);
        }

        // GET: Financas/PlanosContas/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanoContaInfo planoContaInfo = db.Contas.Find(id);
            if (planoContaInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "NomeUsuario", planoContaInfo.IdUsuario);
            return View(planoContaInfo);
        }

        // POST: Financas/PlanosContas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdConta,Descricao,DataAberturaConta,SaldoInicial,SaldoAtual,DataUltimaAtualizacao,Ativo,IdUsuario")] PlanoContaInfo planoContaInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(planoContaInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "NomeUsuario", planoContaInfo.IdUsuario);
            return View(planoContaInfo);
        }

        // GET: Financas/PlanosContas/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanoContaInfo planoContaInfo = db.Contas.Find(id);
            if (planoContaInfo == null)
            {
                return HttpNotFound();
            }
            return View(planoContaInfo);
        }

        // POST: Financas/PlanosContas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PlanoContaInfo planoContaInfo = db.Contas.Find(id);
            db.Contas.Remove(planoContaInfo);
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
