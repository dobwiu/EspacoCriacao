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
    public class LancamentosController : Controller
    {
        private EspacoContext db = new EspacoContext();

        // GET: Financas/Lancamentos
        public ActionResult Index()
        {
            var lancamentos = db.Lancamentos.Include(l => l.Conta).Include(l => l.Usuario);
            return View(lancamentos.ToList());
        }

        // GET: Financas/Lancamentos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LancamentoInfo lancamentoInfo = db.Lancamentos.Find(id);
            if (lancamentoInfo == null)
            {
                return HttpNotFound();
            }
            return View(lancamentoInfo);
        }

        // GET: Financas/Lancamentos/Create
        public ActionResult Create()
        {
            ViewBag.IdConta = new SelectList(db.Contas, "IdConta", "Descricao");
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "NomeUsuario");
            return View();
        }

        // POST: Financas/Lancamentos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdLancamento,IdConta,TipoLancamento,Descricao,Valor,DataProcessamento,Processado,IdUsuario,DataLancamento")] LancamentoInfo lancamentoInfo)
        {
            if (ModelState.IsValid)
            {
                db.Lancamentos.Add(lancamentoInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdConta = new SelectList(db.Contas, "IdConta", "Descricao", lancamentoInfo.IdConta);
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "NomeUsuario", lancamentoInfo.IdUsuario);
            return View(lancamentoInfo);
        }

        // GET: Financas/Lancamentos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LancamentoInfo lancamentoInfo = db.Lancamentos.Find(id);
            if (lancamentoInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdConta = new SelectList(db.Contas, "IdConta", "Descricao", lancamentoInfo.IdConta);
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "NomeUsuario", lancamentoInfo.IdUsuario);
            return View(lancamentoInfo);
        }

        // POST: Financas/Lancamentos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdLancamento,IdConta,TipoLancamento,Descricao,Valor,DataProcessamento,Processado,IdUsuario,DataLancamento")] LancamentoInfo lancamentoInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lancamentoInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdConta = new SelectList(db.Contas, "IdConta", "Descricao", lancamentoInfo.IdConta);
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "IdUsuario", "NomeUsuario", lancamentoInfo.IdUsuario);
            return View(lancamentoInfo);
        }

        // GET: Financas/Lancamentos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LancamentoInfo lancamentoInfo = db.Lancamentos.Find(id);
            if (lancamentoInfo == null)
            {
                return HttpNotFound();
            }
            return View(lancamentoInfo);
        }

        // POST: Financas/Lancamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LancamentoInfo lancamentoInfo = db.Lancamentos.Find(id);
            db.Lancamentos.Remove(lancamentoInfo);
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
