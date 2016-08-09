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
using OfficeOpenXml;

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
            DateTime ontem = DateTime.Today.AddDays(-1);

            var conta = db.Contas.Find(idConta);
            var lancamentos = db.Lancamentos.Include(i => i.Categoria)
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
            extrato.LancFuturos = lancamentos.Where(s => s.DataProcessamento >= ontem && !s.Processado).ToList();

            return extrato;
        }

        public JsonResult GetLancamentos(Guid id)
        {
            return Json(this.GerarExtrato(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ComboCategorias(EnumTipoLancamento tipo)
        {
            var itens = db.Categorias.Where(s => s.TipoLancamento == tipo && s.Ativo).OrderBy(o => o.Descricao).ToList();
            return Json(itens, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddLancamento(LancamentoInfo addLancamento)
        {
            Guid idConta = addLancamento.IdConta;
            string descricao = addLancamento.Descricao;

            addLancamento.Processado = (bool)(addLancamento.DataProcessamento <= DateTime.Now);
            addLancamento.DataLancamento = DateTime.Now; addLancamento.IdUsuario = base.IdUsuario;

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

        public FileResult Exportar(FormCollection exporta)
        {
            Guid idConta = Guid.Parse(exporta["IdConta"]);
            DateTime dataIni = Convert.ToDateTime(exporta["periodoDe"].Trim());
            DateTime dataFim = Convert.ToDateTime(exporta["periodoAte"].Trim());

            #region Query..
            var conta = db.Contas.Find(idConta);
            var lancamentos = db.Lancamentos.Include(i => i.Categoria)
                                .Where(s => s.IdConta == idConta && s.DataProcessamento >= dataIni && s.DataProcessamento <= dataFim)
                                .OrderBy(o => o.DataProcessamento).ToList();

            var extrato = new ExtratoInfo()
            {
                IdConta = idConta,
                PeriodoDe = dataIni,
                PeriodoAte = dataFim,
                SaldoAnterior = conta.SaldoAnterior.Value,
                SaldoAtual = conta.SaldoAtual.Value
            };

            extrato.Lancamentos = lancamentos.Where(s => s.DataProcessamento <= DateTime.Now && s.Processado).OrderBy(o => o.DataProcessamento).ToList();
            extrato.Recebimentos = lancamentos.Where(s => s.TipoLancamento == EnumTipoLancamento.Credito && s.Processado).Sum(v => v.Valor);
            extrato.Pagamentos = lancamentos.Where(s => s.TipoLancamento == EnumTipoLancamento.Debito && s.Processado).Sum(v => v.Valor);
            extrato.LancFuturos = lancamentos.Where(s => s.DataProcessamento >= DateTime.Now && !s.Processado).ToList();
            #endregion

            #region Exportação..
            string path = HttpContext.Server.MapPath("~/Content/modelos/RelFinanceiro.xlsx");
            System.IO.FileInfo modeloXLS = new System.IO.FileInfo(path);

            using (ExcelPackage xls = new ExcelPackage(modeloXLS))
            {
                ExcelWorksheet ws = xls.Workbook.Worksheets["Relatorio"];
                ws.Cells[03, 02].Value = string.Format("Período: {0} - {1}", extrato.PeriodoDe.ToShortDateString(), extrato.PeriodoAte.ToShortDateString());
                ws.Cells[03, 05].Value = string.Concat("Data: ", DateTime.Now.ToString());

                int linha = 6;
                foreach (var item in extrato.Lancamentos)
                {
                    ws.Cells[linha, 02].Value = item.DataProcessamento.ToShortDateString();
                    ws.Cells[linha, 03].Value = item.Categoria.Descricao;
                    ws.Cells[linha, 04].Value = item.Descricao;
                    ws.Cells[linha, 07].Value = item.Comentario;

                    if (item.TipoLancamento == EnumTipoLancamento.Credito)
                        ws.Cells[linha, 05].Value = item.Valor;
                    else
                        ws.Cells[linha, 06].Value = item.Valor;

                    linha++;
                }

                ws.Cells[linha, 02].Value = "TOTAL";
                if (extrato.Recebimentos != 0) { ws.Cells[linha, 05].Value = extrato.Recebimentos; }
                ws.Cells[linha, 06].Value = extrato.Pagamentos;
                ws.Cells[linha, 02, linha, 07].Style.Font.Bold = true;
                ws.Cells[linha, 02, linha, 07].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[linha, 02, linha, 07].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                linha = linha + 2;

                ws.Cells[linha, 02].Value = "LANÇAMENTOS FUTUROS";
                ws.Cells[linha, 02, linha, 07].Merge = true;
                ws.Cells[linha, 02].Style.Font.Bold = true; linha++;
                ws.Cells[05, 02, 05, 07].Copy(ws.Cells[linha, 02, linha, 07]); linha++;

                decimal vCredito = 0; decimal vDebito = 0;
                foreach (var item in extrato.LancFuturos)
                {
                    ws.Cells[linha, 02].Value = item.DataProcessamento.ToShortDateString();
                    ws.Cells[linha, 03].Value = item.Categoria.Descricao;
                    ws.Cells[linha, 04].Value = item.Descricao;
                    ws.Cells[linha, 07].Value = item.Comentario;

                    if (item.TipoLancamento == EnumTipoLancamento.Credito)
                    {
                        ws.Cells[linha, 05].Value = item.Valor; vCredito += item.Valor;
                    }
                    else
                    {
                        ws.Cells[linha, 06].Value = item.Valor; vDebito += item.Valor;
                    }

                    linha++;
                }

                ws.Cells[linha, 02].Value = "TOTAL";
                if (vCredito != 0) { ws.Cells[linha, 05].Value = vCredito; }
                ws.Cells[linha, 06].Value = vDebito;
                ws.Cells[linha, 02, linha, 07].Style.Font.Bold = true;
                ws.Cells[linha, 02, linha, 07].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[linha, 02, linha, 07].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                return File(xls.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Financeiro.xlsx");
            }
            #endregion
        }


        /// <summary>
        /// Processo de Fechamento do Saldo do Dia.
        /// RODAR as 23hs !!!
        /// </summary>
        public ActionResult JobContas()
        {
            IList<JobContasInfo> jobs = new List<JobContasInfo>();
            jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = "Iniciando.." });

            var contas = db.Contas.Where(s => s.Ativo).ToList();
            jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("Processando {0} contas", contas.Count()) });
            foreach (var conta in contas)
            {
                var lances = db.Lancamentos.Where(s => s.IdConta == conta.IdConta && s.DataProcessamento <= DateTime.Today && s.Processado == false).ToList();
                jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("Conta: {0} / {1} lançamentos", conta.Descricao, lances.Count()) });
                jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("Saldo Inicial:{0}", conta.SaldoAtual) });
                foreach (var item in lances)
                {
                    conta.SaldoAtual += item.Valor;
                    item.Processado = true;
                    db.Entry(item).State = EntityState.Modified;
                }
                jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("Saldo Final:{0}", conta.SaldoAtual) });

                var ultimoDia = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                if (DateTime.Today.Day == ultimoDia)
                {
                    var fechamento = new FechamentoInfo()
                    {
                        IdConta = conta.IdConta.Value,
                        DataFechamento = DateTime.Today,
                        ValorFechamento = conta.SaldoAtual.Value
                    };
                    jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("Inserindo Fechamento: {0}", DateTime.Today) });
                    db.Fechamentos.Add(fechamento);
                    conta.SaldoAnterior = conta.SaldoAtual;
                }

                conta.DataUltimaAtualizacao = DateTime.Today;
                db.Entry(conta).State = EntityState.Modified;
                db.SaveChanges();
                jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("{0} atualizada.", conta.Descricao) });
            }

            return View(jobs);
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
