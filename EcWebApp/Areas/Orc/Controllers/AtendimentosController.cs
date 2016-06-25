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
    public class AtendimentosController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        // GET: Orc/Atendimentos
        public ActionResult Index()
        {
            var agendamentos = db.Clientes.Include(c => c.Vendedor).Include(c => c.StatusAtendimento)
                                 .Where(s => s.Interesse == EnumInteresse.Orcamento)
                                 .OrderBy(o => o.DataProximoContato);
            return View(agendamentos.ToList());
        }

        // GET: Orc/Atendimentos/Create
        public ActionResult Create(Guid idCliente)
        {
            var cliente = db.Clientes.Include(c => c.Vendedor).Include(c => c.StatusAtendimento)
                            .Where(s => s.IdCliente == idCliente).FirstOrDefault();

            var atendimento = new AtendimentoInfo()
            {
                IdCliente = idCliente,
                Cliente = cliente,
                Interesse = cliente.Interesse,
                StatusPlanta = cliente.StatusPlanta,
                DataApresentacao = cliente.DataApresentacao,
                DataMedicao = cliente.DataMedicao
            };
                        
            ViewBag.IdStatus = new SelectList(db.StatusAtendimento, "IdStatus", "Descricao");
            return View(atendimento);
        }

        // POST: Orc/Atendimentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AtendimentoInfo atendimentoInfo)
        {
            if (ModelState.IsValid)
            {
                atendimentoInfo.IdAtendimento = Guid.NewGuid();
                atendimentoInfo.DataAtendimento = DateTime.Now;
                atendimentoInfo.RegistradoPor = this.NomeUsuario;

                db.Atendimentos.Add(atendimentoInfo);
                db.SaveChanges();

                this.AtualizaCliente(atendimentoInfo);
                return RedirectToAction("Index");
            }

            ViewBag.IdStatus = new SelectList(db.StatusAtendimento, "IdStatus", "Descricao", atendimentoInfo.IdStatus);
            return View(atendimentoInfo);
        }

        private void AtualizaCliente(AtendimentoInfo atendimentoInfo)
        {
            var clienteInfo = db.Clientes.Find(atendimentoInfo.IdCliente);

            clienteInfo.Interesse = atendimentoInfo.Interesse;
            clienteInfo.IdStatus = atendimentoInfo.IdStatus;
            clienteInfo.StatusPlanta = atendimentoInfo.StatusPlanta;

            clienteInfo.DataProximoContato = atendimentoInfo.DataProximoContato;
            clienteInfo.DataApresentacao = atendimentoInfo.DataApresentacao;
            clienteInfo.DataMedicao = atendimentoInfo.DataMedicao;

            clienteInfo.Comentarios = atendimentoInfo.Comentario;
            clienteInfo.DataUltimoContato = atendimentoInfo.DataAtendimento;

            //-- Caso seja o primeiro contato, atualiza a data também.
            if (!clienteInfo.DataPrimeiroContato.HasValue)
                clienteInfo.DataPrimeiroContato = atendimentoInfo.DataAtendimento;


            db.Entry(clienteInfo).State = EntityState.Modified;
            db.SaveChanges();
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
