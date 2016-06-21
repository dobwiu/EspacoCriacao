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

namespace EcWebApp.Areas.Orc.Controllers
{
    [Authorize]
    public class ClientesController : _BaseController
    {
        private EspacoContext db = new EspacoContext();
        private BLL.Usuario bllUsuario = new BLL.Usuario();

        // GET: Orc/Clientes
        public ActionResult Index()
        {
            var clientes = db.Clientes.Include(c => c.Vendedor).Include(c => c.StatusAtendimento);
            return View(clientes.ToList());
        }

        // GET: Orc/Clientes/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClienteInfo clienteInfo = db.Clientes.Find(id);
            if (clienteInfo == null)
            {
                return HttpNotFound();
            }
            return View(clienteInfo);
        }

        // GET: Orc/Clientes/Create
        public ActionResult Create()
        {
            var clienteInfo = new ClienteInfo() { Enderecos = new List<EnderecoInfo>(), EnderecosIguais = true };
            clienteInfo.Enderecos.Add(new EnderecoInfo() { TipoEndereco = EnumTipoEndereco.Atual, Estado = "SP" });
            clienteInfo.Enderecos.Add(new EnderecoInfo() { TipoEndereco = EnumTipoEndereco.Entrega, Estado = "SP" });

            ViewBag.IdVendedor = new SelectList(bllUsuario.ListarUsuarioCombo(), "IdUsuario", "NomeUsuario");
            ViewBag.IdStatus = new SelectList(db.StatusAtendimento, "IdStatus", "Descricao");

            clienteInfo.DataCadastro = DateTime.Now;
            return View(clienteInfo);
        }

        // POST: Orc/Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClienteInfo clienteInfo)
        {
            if (ModelState.IsValid)
            {
                clienteInfo.IdCliente = Guid.NewGuid();
                IList<EnderecoInfo> enderecos = this.VerificaEnderecos(clienteInfo);

                //clienteInfo.DataCadastro = DateTime.Now;
                clienteInfo.DataProximoContato = DateTime.Today;

                clienteInfo.Enderecos = null;
                db.Clientes.Add(clienteInfo);
                db.SaveChanges();

                if (enderecos != null)
                {
                    foreach (var item in enderecos)
                    {
                        item.IdEndereco = Guid.NewGuid();
                        item.IdCliente = clienteInfo.IdCliente.Value;
                        db.Enderecos.Add(item);
                    }
                    db.SaveChanges();
                }

                this.RegistraAtendimento(clienteInfo, "Cadastramento");
                return RedirectToAction("Index");
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));

                ViewBag.MsgErro = strMsgErro + messages;
                ViewBag.IdVendedor = new SelectList(bllUsuario.ListarUsuarioCombo(), "IdUsuario", "NomeUsuario", clienteInfo.IdVendedor);
                ViewBag.IdStatus = new SelectList(db.StatusAtendimento, "IdStatus", "Descricao", clienteInfo.IdStatus);

                return View(clienteInfo);
            }
        }

        private void RegistraAtendimento(ClienteInfo clienteInfo, string comentario)
        {
            var atendimento = new AtendimentoInfo()
            {
                IdAtendimento = Guid.NewGuid(),
                IdCliente = clienteInfo.IdCliente.Value,
                Interesse = clienteInfo.Interesse,
                IdStatus = clienteInfo.IdStatus,
                StatusPlanta = clienteInfo.StatusPlanta,
                DataApresentacao = clienteInfo.DataApresentacao,
                DataMedicao = clienteInfo.DataMedicao,
                IdVendedor = clienteInfo.IdVendedor,
                DataAtendimento = DateTime.Now,
                Comentario = comentario,
                RegistradoPor = this.NomeUsuario
            };

            db.Atendimentos.Add(atendimento);
            db.SaveChanges();
        }

        private IList<EnderecoInfo> VerificaEnderecos(ClienteInfo clienteInfo)
        {
            var lstEndereco = new List<EnderecoInfo>();
            var endAtual = clienteInfo.Enderecos.Where(s => s.TipoEndereco == EnumTipoEndereco.Atual).FirstOrDefault();
            var endEntrega = clienteInfo.Enderecos.Where(s => s.TipoEndereco == EnumTipoEndereco.Entrega).FirstOrDefault();

            if (clienteInfo.EnderecosIguais)
                endEntrega = new EnderecoInfo()
                {
                    TipoEndereco = EnumTipoEndereco.Entrega,
                    Endereco = endAtual.Endereco,
                    Complemento = endAtual.Complemento,
                    Bairro = endAtual.Bairro,
                    Cidade = endAtual.Cidade,
                    Estado = endAtual.Estado,
                    CEP = endAtual.CEP,
                    Observacao = endAtual.Observacao
                };

            lstEndereco.Add(endAtual);
            lstEndereco.Add(endEntrega);
            return lstEndereco;
        }

        // GET: Orc/Clientes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClienteInfo clienteInfo = db.Clientes.Include(c => c.Vendedor).Include(c => c.StatusAtendimento).Where(s => s.IdCliente == id.Value).FirstOrDefault();
            if (clienteInfo == null)
            {
                return HttpNotFound();
            }

            /* Buscando os endereços do cliente.. */
            clienteInfo.Enderecos = db.Enderecos.Where(s => s.IdCliente == clienteInfo.IdCliente).OrderBy(o => o.TipoEndereco).ToList();

            ViewBag.IdVendedor = new SelectList(bllUsuario.ListarUsuarioCombo(), "IdUsuario", "NomeUsuario", clienteInfo.IdVendedor);
            ViewBag.IdStatus = new SelectList(db.StatusAtendimento, "IdStatus", "Descricao", clienteInfo.IdStatus);

            return View(clienteInfo);
        }

        // POST: Orc/Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClienteInfo clienteInfo)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in clienteInfo.Enderecos)
                    db.Entry(item).State = EntityState.Modified;

                db.Entry(clienteInfo).State = EntityState.Modified;
                db.SaveChanges();

                this.RegistraAtendimento(clienteInfo, "Atualização dos dados");
                return RedirectToAction("Index");
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));

                ViewBag.MsgErro = strMsgErro + messages;
                ViewBag.IdVendedor = new SelectList(bllUsuario.ListarUsuarioCombo(), "IdUsuario", "NomeUsuario", clienteInfo.IdVendedor);
                ViewBag.IdStatus = new SelectList(db.StatusAtendimento, "IdStatus", "Descricao", clienteInfo.IdStatus);

                return View(clienteInfo);
            }
        }

        // GET: Orc/Clientes/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var clienteInfo = db.Clientes.Include(c => c.Vendedor).Include(c => c.StatusAtendimento)
                             .Where(s => s.IdCliente == id.Value).FirstOrDefault();

            if (clienteInfo == null)
            {
                return HttpNotFound();
            }
            return View(clienteInfo);
        }

        // POST: Orc/Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ClienteInfo clienteInfo = db.Clientes.Include("Enderecos").Where(s => s.IdCliente == id).First();
            db.Enderecos.RemoveRange(clienteInfo.Enderecos);
            db.Clientes.Remove(clienteInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public PartialViewResult ListaHistorico(Guid idCliente)
        {
            var historico = db.Atendimentos.Include(a => a.Status).Include(a => a.Vendedor)
                              .Where(s => s.IdCliente == idCliente).OrderByDescending(o => o.DataAtendimento);

            return PartialView("pvHistorico", historico);
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
