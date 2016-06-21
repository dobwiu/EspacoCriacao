using EcWebApp.Controllers;
using EcWebApp.DAL;
using EcWebApp.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace EcWebApp.Areas.Admin.Controllers
{
    [Authorize]
    public class UsuariosController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        // GET: Admin/Usuarios
        public ActionResult Index()
        {
            var idAdmin = Guid.Empty;
            var usuarios = db.Usuarios.Where(s => s.IdUsuario != idAdmin).OrderBy(o => o.NomeUsuario);
            return View(usuarios.ToList());
        }

        // GET: Admin/Usuarios/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioInfo usuarioInfo = db.Usuarios.Find(id);
            if (usuarioInfo == null)
            {
                return HttpNotFound();
            }
            return View(usuarioInfo);
        }

        // GET: Admin/Usuarios/Create
        public ActionResult Create()
        {
            UsuarioInfo usuarioInfo = new UsuarioInfo() { Ativo = true, Permissoes = new PermissaoInfo() };
            return View(usuarioInfo);
        }

        // POST: Admin/Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsuarioInfo usuarioInfo)
        {
            var bllUsuario = new BLL.Usuario();
            string senhaMD5 = bllUsuario.CalculateMD5Hash(usuarioInfo.SenhaUsuario);

            if (ModelState.IsValid)
            {
                usuarioInfo.IdUsuario = Guid.NewGuid();
                usuarioInfo.SenhaUsuario = senhaMD5;
                usuarioInfo.SenhaConfirmada = senhaMD5;

                db.Usuarios.Add(usuarioInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                ViewBag.MsgErro = strMsgErro + messages;
                return View(usuarioInfo);
            }
        }

        // GET: Admin/Usuarios/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UsuarioInfo usuarioInfo = db.Usuarios.Include("Permissoes").Where(s => s.IdUsuario == id.Value).SingleOrDefault();
            if (usuarioInfo == null)
            {
                return HttpNotFound();
            }

            //ViewBag.IdUsuario = new SelectList(db.Permissoes, "IdUsuario", "IdUsuario", usuarioInfo.IdUsuario);
            return View(usuarioInfo);
        }

        // POST: Admin/Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsuarioInfo usuarioInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuarioInfo.Permissoes).State = EntityState.Modified;
                db.Entry(usuarioInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                ViewBag.MsgErro = strMsgErro + messages;
                return View(usuarioInfo);
            }
        }

        // GET: Admin/Usuarios/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioInfo usuarioInfo = db.Usuarios.Include("Permissoes").Where(s => s.IdUsuario == id.Value).SingleOrDefault();
            if (usuarioInfo == null)
            {
                return HttpNotFound();
            }

            ViewBag.PossuiClientes = db.Clientes.Where(s => s.IdVendedor == id.Value).Any();
            return View(usuarioInfo);
        }

        // POST: Admin/Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            UsuarioInfo usuarioInfo = db.Usuarios.Include("Permissoes").Where(s => s.IdUsuario == id).SingleOrDefault();
            db.Permissoes.Remove(usuarioInfo.Permissoes);
            db.Usuarios.Remove(usuarioInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DesactiveConfirmed(Guid id)
        {
            UsuarioInfo usuarioInfo = db.Usuarios.Find(id);
            usuarioInfo.SenhaConfirmada = usuarioInfo.SenhaUsuario;
            usuarioInfo.Ativo = false;
            db.Entry(usuarioInfo).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult NewPassword(Guid id, string pword)
        {
            var bllUsuario = new BLL.Usuario();
            string newPass = bllUsuario.RedefinirSenha(id, pword);
            return Json(newPass, JsonRequestBehavior.AllowGet);
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
