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
using EcWebApp.ViewModels;
using EcWebApp.Controllers;

namespace EcWebApp.Areas.Financas.Controllers
{
    [Authorize]
    public class LancamentosController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        // GET: Financas/Lancamentos
        public ActionResult Index()
        {
            var contas = db.Contas.Where(s => s.Ativo).OrderBy(o => o.Descricao);
            if (contas.Count() == 1)
                return RedirectToAction("Details", new { id = contas.First().IdConta.Value });

            /* Se houver mais que 1 conta, retorna lista para usuário selecionar.. */
            return View(contas.ToList());
        }

        // GET: Financas/Lancamentos/Details/5
        public ActionResult Details(Guid id)
        {
            var conta = db.Contas.Find(id);
            ViewBag.IdConta = conta.IdConta;
            ViewBag.Title = conta.Descricao;
            ViewBag.SubTitle = DateTime.Today.ToString("MM/yyyy");

            return View();
        }

        private ExtratoInfo GerarExtrato(Guid idConta)
        {
            DateTime inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
            DateTime finalMes = inicioMes.AddMonths(1);
            DateTime finalPeriodo = finalMes.AddMonths(2).AddDays(15);

            var conta = db.Contas.Find(idConta);
            var lancamentos = db.Lancamentos.Include(i=> i.Categoria)
                                .Where(s => s.IdConta == idConta && s.DataProcessamento >= inicioMes && s.DataProcessamento < finalPeriodo)
                                .OrderBy(o => o.DataProcessamento).ToList();


            var extrato = new ExtratoInfo()
            {
                IdConta = idConta,
                PeriodoDe = inicioMes,
                PeriodoAte = finalMes,
                SaldoAnterior = conta.SaldoAnterior.Value,
                SaldoAtual = conta.SaldoAtual.Value
            };

            extrato.Lancamentos = lancamentos.Where(s => s.DataProcessamento <= DateTime.Now && s.Processado).OrderBy(o => o.DataProcessamento).ToList();
            extrato.Recebimentos = lancamentos.Where(s => s.TipoLancamento == EnumTipoLancamento.Credito && s.Processado).Sum(v => v.Valor);
            extrato.Pagamentos = lancamentos.Where(s => s.TipoLancamento == EnumTipoLancamento.Debito && s.Processado).Sum(v => v.Valor);
            extrato.LancFuturos = lancamentos.Where(s => s.DataProcessamento >= DateTime.Now && !s.Processado).ToList();

            return extrato;
        }

        public JsonResult GetLancamentos(Guid id)
        {
            return Json(this.GerarExtrato(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ComboCategorias(EnumTipoLancamento tipo)
        {
            var itens = db.Categorias.Where(s => s.TipoLancamento == tipo).OrderBy(o => o.Descricao).ToList();
            return Json(itens, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddLancamento(LancamentoInfo addLancamento)
        {
            Guid idConta = addLancamento.IdConta;
            string descricao = addLancamento.Descricao;

            addLancamento.Processado = (bool)(addLancamento.DataProcessamento <= DateTime.Now);
            addLancamento.DataLancamento = DateTime.Now;
            if (addLancamento.TipoLancamento == EnumTipoLancamento.Debito)
            {
                addLancamento.Valor *= -1;
            }

            if (addLancamento.Repeticao == EnumTipoRepeticao.Unico)
                db.Lancamentos.Add(addLancamento);
            else
            {
                //-- Primeira Parcela -->
                addLancamento.Descricao = string.Format("{0} - 1/{1}", descricao, addLancamento.Vezes);
                db.Lancamentos.Add(addLancamento);

                DateTime proxData = addLancamento.DataProcessamento;
                for (int i = 2; i <= addLancamento.Vezes; i++)
                {
                    switch (addLancamento.Repeticao)
                    {
                        case EnumTipoRepeticao.Semanal:
                            proxData = proxData.AddDays(7);
                            break;
                        case EnumTipoRepeticao.Quinzenal:
                            proxData = proxData.AddDays(15);
                            break;
                        case EnumTipoRepeticao.Mensal:
                            proxData = proxData.AddMonths(1);
                            break;
                        case EnumTipoRepeticao.Anual:
                            proxData = proxData.AddYears(1);
                            break;
                    }

                    //-- Demais Parcelas -->
                    var novoItem = new LancamentoInfo()
                    {
                        IdConta = idConta,
                        TipoLancamento = addLancamento.TipoLancamento,
                        IdCategoria = addLancamento.IdCategoria,
                        Descricao = string.Format("{0} - {1}/{2}", descricao, i, addLancamento.Vezes),
                        Valor = addLancamento.Valor,
                        DataProcessamento = proxData,
                        Processado = false,
                        Comentario = addLancamento.Comentario,
                        DataLancamento = DateTime.Now,
                        IdUsuario = base.IdUsuario
                    };

                    db.Lancamentos.Add(novoItem);
                }
            }

            if (addLancamento.Processado)
            {
                var conta = db.Contas.Find(addLancamento.IdConta);
                conta.SaldoAtual += addLancamento.Valor;
                db.Entry(conta).State = EntityState.Modified;
            }

            db.SaveChanges();
            return Json(this.GerarExtrato(idConta), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DelLancamento(LancamentoInfo delLancamento)
        {
            Guid idConta = delLancamento.IdConta;

            if (delLancamento.Processado)
            {
                delLancamento.Valor *= -1;
                var conta = db.Contas.Find(idConta);
                conta.SaldoAtual += delLancamento.Valor;
                db.Entry(conta).State = EntityState.Modified;
            }

            var lancamento = db.Lancamentos.Find(delLancamento.IdLancamento);
            db.Lancamentos.Remove(lancamento);
            db.SaveChanges();

            return Json(this.GerarExtrato(idConta), JsonRequestBehavior.AllowGet);
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
