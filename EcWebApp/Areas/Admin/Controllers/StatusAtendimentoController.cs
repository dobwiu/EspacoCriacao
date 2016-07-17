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
    public class StatusAtendimentoController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        // GET: Admin/StatusAtendimento
        public ActionResult Index()
        {
            return View(db.StatusAtendimento.OrderBy(o=> o.Descricao).ToList());
        }

        // GET: Admin/StatusAtendimento/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatusAtendimentoInfo statusAtendimentoInfo = db.StatusAtendimento.Find(id);
            if (statusAtendimentoInfo == null)
            {
                return HttpNotFound();
            }
            return View(statusAtendimentoInfo);
        }

        // GET: Admin/StatusAtendimento/Create
        public ActionResult Create()
        {
            StatusAtendimentoInfo statusAtendimentoInfo = new StatusAtendimentoInfo() { Ativo = true, Apresentacao = false };
            return View(statusAtendimentoInfo);
        }

        // POST: Admin/StatusAtendimento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StatusAtendimentoInfo statusAtendimentoInfo)
        {
            if (ModelState.IsValid)
            {
                db.StatusAtendimento.Add(statusAtendimentoInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statusAtendimentoInfo);
        }

        // GET: Admin/StatusAtendimento/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatusAtendimentoInfo statusAtendimentoInfo = db.StatusAtendimento.Find(id);
            if (statusAtendimentoInfo == null)
            {
                return HttpNotFound();
            }

            ViewBag.IsAdmin = (bool)(IdUsuario == Guid.Empty);
            return View(statusAtendimentoInfo);
        }

        // POST: Admin/StatusAtendimento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StatusAtendimentoInfo statusAtendimentoInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statusAtendimentoInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statusAtendimentoInfo);
        }

        // GET: Admin/StatusAtendimento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatusAtendimentoInfo statusAtendimentoInfo = db.StatusAtendimento.Find(id);
            if (statusAtendimentoInfo == null)
            {
                return HttpNotFound();
            }

            ViewBag.ExisteAtendimento = db.Atendimentos.Where(s => s.IdStatus == id.Value).Any();
            return View(statusAtendimentoInfo);
        }

        // POST: Admin/StatusAtendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StatusAtendimentoInfo statusAtendimentoInfo = db.StatusAtendimento.Find(id);
            db.StatusAtendimento.Remove(statusAtendimentoInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DesactiveConfirmed(int id)
        {
            StatusAtendimentoInfo statusAtendimentoInfo = db.StatusAtendimento.Find(id);
            statusAtendimentoInfo.Ativo = false;
            db.Entry(statusAtendimentoInfo).State = EntityState.Modified;
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
