using EcWebApp.BLL;
using EcWebApp.DAL;
using EcWebApp.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcWebApp.Areas.Reports.Controllers
{
    public class RelDesempenhoController : Controller
    {
        private EspacoContext db = new EspacoContext();

        // GET: Reports/RelDesempenho
        public ActionResult Index()
        {
            ViewBag.IdVendedor = new SelectList(new Usuario().ListarUsuarioCombo(), "IdUsuario", "NomeUsuario");
            return View();
        }

        public JsonResult ComboPeriodo(string formato)
        {
            List<string> periodos = new List<string>();
            if (formato == "D")
            {
                DateTime periodoAtual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime periodoLimite = new DateTime(2016, 05, 01);
                while (periodoAtual >= periodoLimite)
                {
                    periodos.Add(string.Format("{0}/{1}", periodoAtual.Month.ToString("00"), periodoAtual.Year.ToString()));
                    periodoAtual = periodoAtual.AddMonths(-1);
                }
            }
            else
            {
                for (int ano = DateTime.Now.Year; ano >= 2016; ano--)
                    periodos.Add(ano.ToString());
            }

            return Json(periodos, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Exibir(string pFormato, string pPeriodo, string pIdVendedor)
        {
            var relatorio = this.Gerar(pFormato, pPeriodo, pIdVendedor, true);
            return PartialView("pvRelDesempenho", relatorio);
        }

        public FileResult Exportar(FormCollection filtros)
        {
            string Vendedor = "TODOS";
            if (string.IsNullOrEmpty(filtros["IdVendedor"]) == false)
            {
                Guid idVendedor = Guid.Parse(filtros["IdVendedor"]);
                Vendedor = db.Usuarios.Where(s => s.IdUsuario == idVendedor).First().NomeUsuario;
            }

            string path = HttpContext.Server.MapPath("~/Content/modelos/RelDesempenho.xlsx");
            System.IO.FileInfo modeloXLS = new System.IO.FileInfo(path);

            using (ExcelPackage xls = new ExcelPackage(modeloXLS))
            {
                var relatorio = this.Gerar(filtros["Formato"], filtros["Periodo"], filtros["IdVendedor"]);

                int linha = 8;
                ExcelWorksheet ws = xls.Workbook.Worksheets["Relatorio"];
                //ExcelWorksheet ws = xls.Workbook.Worksheets.Add("Relatorio");

                ws.Cells[3, 3].Value = Vendedor;
                ws.Cells[4, 3].Value = DateTime.Today.ToShortDateString();

                foreach (var item in relatorio)
                {
                    ws.Cells[linha, 02].Value = item.Data.ToShortDateString();
                    if (item.DiaUtil)
                    {
                        ws.Cells[linha, 03].Value = item.Atendimentos;
                        ws.Cells[linha, 04].Value = item.QtdProcompra;
                        ws.Cells[linha, 05].Value = item.QtdPorta;
                        ws.Cells[linha, 06].Value = item.QtdOutros;
                        ws.Cells[linha, 07].Value = item.QtdApresentacoes;
                        ws.Cells[linha, 08].Value = item.ValorApresentacoes;
                        ws.Cells[linha, 09].Value = item.QtdPedidos;
                        ws.Cells[linha, 10].Value = item.ValorPedidos;
                        ws.Cells[linha, 11].Value = item.QtdPerdidos;
                        ws.Cells[linha, 12].Value = item.ValorPerdidos;
                    }
                    else
                    {
                        using (ExcelRange rng = ws.Cells[linha, 2, linha, 12])
                        {
                            rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                        }
                    }

                    linha++;
                }

                using (ExcelRange rng = ws.Cells[linha, 2, linha, 12])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                }

                ws.Cells[linha, 02].Value = "TOTAL";
                ws.Cells[linha, 03].Formula = string.Format("SUM(C8:C{0})", (linha - 1));
                ws.Cells[linha, 04].Formula = string.Format("SUM(D8:D{0})", (linha - 1));
                ws.Cells[linha, 05].Formula = string.Format("SUM(E8:E{0})", (linha - 1));
                ws.Cells[linha, 06].Formula = string.Format("SUM(F8:F{0})", (linha - 1));
                ws.Cells[linha, 07].Formula = string.Format("SUM(G8:G{0})", (linha - 1));
                ws.Cells[linha, 08].Formula = string.Format("SUM(H8:H{0})", (linha - 1));
                ws.Cells[linha, 09].Formula = string.Format("SUM(I8:I{0})", (linha - 1));
                ws.Cells[linha, 10].Formula = string.Format("SUM(J8:J{0})", (linha - 1));
                ws.Cells[linha, 11].Formula = string.Format("SUM(K8:K{0})", (linha - 1));
                ws.Cells[linha, 12].Formula = string.Format("SUM(L8:L{0})", (linha - 1));

                return File(xls.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Indic-Desempenho.xlsx");
            }
        }

        private IList<RelatorioDesempenhoInfo> Gerar(string formato, string periodo, string pIdVendedor, bool total = false)
        {
            Guid? idVendedor = null;
            DateTime periodoDe = new DateTime(); DateTime periodoAte = new DateTime();
            IList<Models.FeriadoInfo> feriados;

            if (!string.IsNullOrEmpty(pIdVendedor)) { idVendedor = Guid.Parse(pIdVendedor); }
            string Vendedor = idVendedor.HasValue ? db.Usuarios.Find(idVendedor).NomeUsuario : "TODOS";

            if (formato == "D")
            {
                int[] data = Array.ConvertAll(periodo.Split('/'), int.Parse);
                periodoDe = new DateTime(data[1], data[0], 01);
                periodoAte = periodoDe.AddMonths(1);
            }
            else
            {
                int anoPeriodo = Convert.ToInt32(periodo);
                int mesInicial = (anoPeriodo == 2016) ? 5 : 1;
                int mesAtual = DateTime.Today.Month;
                int ultimoDia = DateTime.DaysInMonth(anoPeriodo, mesAtual);

                periodoDe = new DateTime(anoPeriodo, mesInicial, 01);
                periodoAte = new DateTime(anoPeriodo, mesAtual, ultimoDia, 23, 59, 59);
            }

            //-- Buscando os clientes e pedidos do período..
            var clientes = db.Clientes.Include(c => c.StatusAtendimento).Where(s => s.DataCadastro >= periodoDe && s.DataCadastro < periodoAte).ToList();
            var pedidos = db.Pedidos.Where(s => s.DataPedido >= periodoDe && s.DataPedido < periodoAte).ToList();

            if (idVendedor.HasValue)
            {
                clientes = clientes.Where(s => s.IdVendedor == idVendedor.Value).ToList();
                pedidos = pedidos.Where(s => s.IdVendedor == idVendedor.Value).ToList();
            }

            IList<RelatorioDesempenhoInfo> relatorio = new List<RelatorioDesempenhoInfo>();
            var itemTotal = new RelatorioDesempenhoInfo()
            {
                Formato = formato,
                DiaUtil = false,
                LinhaTotal = true,
                QtdProcompra = 0,
                QtdPorta = 0,
                QtdOutros = 0,
                QtdApresentacoes = 0,
                ValorApresentacoes = 0,
                QtdPedidos = 0,
                ValorPedidos = 0,
                QtdPerdidos = 0,
                ValorPerdidos = 0
            };

            if (formato == "D")
            {
                int ultimoDia = DateTime.DaysInMonth(periodoDe.Year, periodoDe.Month);
                feriados = db.Feriados.Where(s => s.DataFeriado >= periodoDe && s.DataFeriado < periodoAte).ToList();

                for (int dia = 1; dia <= ultimoDia; dia++)
                {
                    var item = new RelatorioDesempenhoInfo() { Formato = "D" };
                    item.Data = new DateTime(periodoDe.Year, periodoDe.Month, dia);
                    item.DiaUtil = true; item.LinhaTotal = false;

                    var isFeriado = feriados.Where(s => s.DataFeriado == item.Data).Any();
                    if (item.Data.DayOfWeek == DayOfWeek.Sunday || isFeriado) { item.DiaUtil = false; }
                    else
                    {
                        var clientesDia = clientes.Where(s => s.DataCadastro.ToShortDateString() == item.Data.ToShortDateString()).ToList();
                        if (clientesDia != null)
                        {
                            item.QtdProcompra = clientesDia.Where(s => s.Procedencia == Models.EnumProcedencia.ProCompra).Count();
                            item.QtdPorta = clientesDia.Where(s => s.Procedencia == Models.EnumProcedencia.Porta).Count();
                            item.QtdOutros = clientesDia.Count() - (item.QtdProcompra.GetValueOrDefault(0) + item.QtdPorta.GetValueOrDefault(0));

                            item.QtdPerdidos = clientesDia.Where(s => s.Interesse == Models.EnumInteresse.SemInteresse).Count();
                            item.ValorPerdidos = clientesDia.Where(s => s.Interesse == Models.EnumInteresse.SemInteresse).Sum(v => v.ValorEstimadoProjeto);

                            item.QtdApresentacoes = clientesDia.Where(s => s.StatusAtendimento.Apresentacao.GetValueOrDefault(false)).Count();
                            item.ValorApresentacoes = clientesDia.Where(s => s.StatusAtendimento.Apresentacao.GetValueOrDefault(false)).Sum(v => v.ValorEstimadoProjeto);
                        }

                        var pedidosDia = pedidos.Where(s => s.DataPedido.ToShortDateString() == item.Data.ToShortDateString()).ToList();
                        if (pedidosDia != null)
                        {
                            item.QtdPedidos = pedidosDia.Where(s => s.StatusPedido != Models.EnumStatusPedido.Cancelado).Count();
                            item.ValorPedidos = pedidosDia.Where(s => s.StatusPedido != Models.EnumStatusPedido.Cancelado).Sum(v => v.ValorOrcamento);

                            //relatorio.QtdPerdidos = pedidosDia.Where(s => s.StatusPedido == Models.EnumStatusPedido.Cancelado).Count();
                            //relatorio.ValorPerdidos = pedidosDia.Where(s => s.StatusPedido == Models.EnumStatusPedido.Cancelado).Sum(v => v.ValorOrcamento);
                        }

                        if (total)
                        {
                            itemTotal.QtdProcompra += item.QtdProcompra;
                            itemTotal.QtdPorta += item.QtdPorta;
                            itemTotal.QtdOutros += item.QtdOutros;
                            itemTotal.QtdApresentacoes += item.QtdApresentacoes;
                            itemTotal.ValorApresentacoes += item.ValorApresentacoes;
                            itemTotal.QtdPedidos += item.QtdPedidos;
                            itemTotal.ValorPedidos += item.ValorPedidos;
                            itemTotal.QtdPerdidos += item.QtdPerdidos;
                            itemTotal.ValorPerdidos += item.ValorPerdidos;
                        }
                    }
                    relatorio.Add(item);
                }
            }
            else    //-- Fomato "M"
            {
                for (int mes = periodoDe.Month; mes <= periodoAte.Month; mes++)
                {
                    var item = new RelatorioDesempenhoInfo() { Formato = "M" };
                    item.Data = new DateTime(periodoDe.Year, mes, 1);
                    item.DiaUtil = true; item.LinhaTotal = false;
                    var dataAte = item.Data.AddMonths(1);

                    var clientesMes = clientes.Where(s => s.DataCadastro >= item.Data && s.DataCadastro < dataAte).ToList();
                    if (clientesMes != null)
                    {
                        item.QtdProcompra = clientesMes.Where(s => s.Procedencia == Models.EnumProcedencia.ProCompra).Count();
                        item.QtdPorta = clientesMes.Where(s => s.Procedencia == Models.EnumProcedencia.Porta).Count();
                        item.QtdOutros = clientesMes.Count() - (item.QtdProcompra.GetValueOrDefault(0) + item.QtdPorta.GetValueOrDefault(0));

                        item.QtdPerdidos = clientesMes.Where(s => s.Interesse == Models.EnumInteresse.SemInteresse).Count();
                        item.ValorPerdidos = clientesMes.Where(s => s.Interesse == Models.EnumInteresse.SemInteresse).Sum(v => v.ValorEstimadoProjeto);

                        item.QtdApresentacoes = clientesMes.Where(s => s.StatusAtendimento.Apresentacao.GetValueOrDefault(false)).Count();
                        item.ValorApresentacoes = clientesMes.Where(s => s.StatusAtendimento.Apresentacao.GetValueOrDefault(false)).Sum(v => v.ValorEstimadoProjeto);
                    }

                    var pedidosMes = pedidos.Where(s => s.DataPedido >= item.Data && s.DataPedido < dataAte).ToList();
                    if (pedidosMes != null)
                    {
                        item.QtdPedidos = pedidosMes.Where(s => s.StatusPedido != Models.EnumStatusPedido.Cancelado).Count();
                        item.ValorPedidos = pedidosMes.Where(s => s.StatusPedido != Models.EnumStatusPedido.Cancelado).Sum(v => v.ValorOrcamento);

                        //relatorio.QtdPerdidos = pedidosDia.Where(s => s.StatusPedido == Models.EnumStatusPedido.Cancelado).Count();
                        //relatorio.ValorPerdidos = pedidosDia.Where(s => s.StatusPedido == Models.EnumStatusPedido.Cancelado).Sum(v => v.ValorOrcamento);
                    }

                    if (total)
                    {
                        itemTotal.QtdProcompra += item.QtdProcompra;
                        itemTotal.QtdPorta += item.QtdPorta;
                        itemTotal.QtdOutros += item.QtdOutros;
                        itemTotal.QtdApresentacoes += item.QtdApresentacoes;
                        itemTotal.ValorApresentacoes += item.ValorApresentacoes;
                        itemTotal.QtdPedidos += item.QtdPedidos;
                        itemTotal.ValorPedidos += item.ValorPedidos;
                        itemTotal.QtdPerdidos += item.QtdPerdidos;
                        itemTotal.ValorPerdidos += item.ValorPerdidos;
                    }
                    relatorio.Add(item);
                }
            }

            if (total) { relatorio.Add(itemTotal); }
            return relatorio;
        }
    }
}