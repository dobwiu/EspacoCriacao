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
    public class FormasPagamentoController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        // GET: Admin/FormasPagamento
        public ActionResult Index()
        {
            return View(db.FormasPagamento.OrderBy(o=> o.Descricao).ToList());
        }

        // GET: Admin/FormasPagamento/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormaPagamentoInfo formaPagamentoInfo = db.FormasPagamento.Find(id);
            if (formaPagamentoInfo == null)
            {
                return HttpNotFound();
            }
            return View(formaPagamentoInfo);
        }

        // GET: Admin/FormasPagamento/Create
        public ActionResult Create()
        {
            FormaPagamentoInfo formaPagamentoInfo = new FormaPagamentoInfo() { Ativo = true };
            return View(formaPagamentoInfo);
        }

        // POST: Admin/FormasPagamento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdFormaPagto,Descricao,Ativo")] FormaPagamentoInfo formaPagamentoInfo)
        {
            if (ModelState.IsValid)
            {
                db.FormasPagamento.Add(formaPagamentoInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(formaPagamentoInfo);
        }

        // GET: Admin/FormasPagamento/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormaPagamentoInfo formaPagamentoInfo = db.FormasPagamento.Find(id);
            if (formaPagamentoInfo == null)
            {
                return HttpNotFound();
            }
            return View(formaPagamentoInfo);
        }

        // POST: Admin/FormasPagamento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdFormaPagto,Descricao,Ativo")] FormaPagamentoInfo formaPagamentoInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(formaPagamentoInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(formaPagamentoInfo);
        }

        // GET: Admin/FormasPagamento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormaPagamentoInfo formaPagamentoInfo = db.FormasPagamento.Find(id);
            if (formaPagamentoInfo == null)
            {
                return HttpNotFound();
            }

            ViewBag.ExistePedido = db.Pedidos.Where(s => s.IdFormaPagamento == id.Value).Any();
            return View(formaPagamentoInfo);
        }

        // POST: Admin/FormasPagamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FormaPagamentoInfo formaPagamentoInfo = db.FormasPagamento.Find(id);
            db.FormasPagamento.Remove(formaPagamentoInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DesactiveConfirmed(int id)
        {
            FormaPagamentoInfo formaPagamentoInfo = db.FormasPagamento.Find(id);
            formaPagamentoInfo.Ativo = false;
            db.Entry(formaPagamentoInfo).State = EntityState.Modified;
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
