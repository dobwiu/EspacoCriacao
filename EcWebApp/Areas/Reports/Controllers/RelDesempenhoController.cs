using EcWebApp.BLL;
using EcWebApp.DAL;
using EcWebApp.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
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
            string periodoAtual = string.Format("{0}/{1}", DateTime.Today.Month.ToString("00"), DateTime.Today.Year);
            List<string> periodos = new List<string>();
            for (int ano = 2016; ano <= DateTime.Now.Year; ano++)
            {
                int mes = (ano == 2016 ? 5 : 1);
                for (int m = mes; m <= 12; m++)
                {
                    periodos.Add(string.Format("{0}/{1}", m.ToString("00"), ano));
                }
            }

            ViewBag.Periodo = new SelectList(periodos, periodoAtual);
            ViewBag.IdVendedor = new SelectList(new Usuario().ListarUsuarioCombo(), "IdUsuario", "NomeUsuario");
            return View();
        }

        public PartialViewResult Exibir(string pPeriodo, string pIdVendedor)
        {
            var relatorio = this.Gerar(pPeriodo, pIdVendedor, true);

            int[] data = Array.ConvertAll(pPeriodo.Split('/'), int.Parse);
            int mes = data[0]; int ano = data[1];
            var meta = db.Metas.Where(s => s.Mes == mes && s.Ano == ano).FirstOrDefault();
            if (meta != null)
            {
                ViewBag.Meta = meta.ValorMeta;
                ViewBag.Desafio = meta.ValorDesafio;
            }
            else
            {
                ViewBag.Meta = "---";
                ViewBag.Desafio = "---";
            }

            return PartialView("pvRelDesempenho", relatorio);
        }

        public FileResult Exportar(FormCollection filtros)
        {
            int[] data = Array.ConvertAll(filtros["Periodo"].Split('/'), int.Parse);
            int mes = data[0]; int ano = data[1];
            var meta = db.Metas.Where(s => s.Mes == mes && s.Ano == ano).FirstOrDefault();

            string Vendedor = "TODOS";
            if (string.IsNullOrEmpty(filtros["IdVendedor"]) == false)
            {
                Guid idVendedor = Guid.Parse(filtros["IdVendedor"]);
                Vendedor = db.Usuarios.Where(s => s.IdUsuario == idVendedor).First().NomeUsuario;
            }

            //string path = HttpContext.Server.MapPath("~/Content/modelos/RelDesempenho.xlsx");
            //System.IO.FileInfo modeloXLS = new System.IO.FileInfo(path);

            using (ExcelPackage xls = new ExcelPackage())
            {
                var relatorio = this.Gerar(filtros["Periodo"], filtros["IdVendedor"]);

                int linha = 6;
                //ExcelWorksheet ws = xls.Workbook.Worksheets["Indicativos"];
                ExcelWorksheet ws = xls.Workbook.Worksheets.Add("Indicativos");
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
                ws.Cells[linha, 03].Formula = string.Format("SUM(C6:C{0})", (linha - 1));
                ws.Cells[linha, 04].Formula = string.Format("SUM(D6:D{0})", (linha - 1));
                ws.Cells[linha, 05].Formula = string.Format("SUM(E6:E{0})", (linha - 1));
                ws.Cells[linha, 06].Formula = string.Format("SUM(F6:F{0})", (linha - 1));
                ws.Cells[linha, 07].Formula = string.Format("SUM(G6:G{0})", (linha - 1));
                ws.Cells[linha, 08].Formula = string.Format("SUM(H6:H{0})", (linha - 1));
                ws.Cells[linha, 09].Formula = string.Format("SUM(I6:I{0})", (linha - 1));
                ws.Cells[linha, 10].Formula = string.Format("SUM(J6:J{0})", (linha - 1));
                ws.Cells[linha, 11].Formula = string.Format("SUM(K6:K{0})", (linha - 1));
                ws.Cells[linha, 12].Formula = string.Format("SUM(L6:L{0})", (linha - 1));

                return File(xls.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RelDesempenho.xlsx");
            }
        }

        private IList<RelatorioDesempenhoInfo> Gerar(string pedido, string pIdVendedor, bool total = false)
        {
            Guid? idVendedor = null;
            if (!string.IsNullOrEmpty(pIdVendedor)) { idVendedor = Guid.Parse(pIdVendedor); }
            string Vendedor = idVendedor.HasValue ? db.Usuarios.Find(idVendedor).NomeUsuario : "TODOS";

            int[] data = Array.ConvertAll(pedido.Split('/'), int.Parse);
            DateTime periodoDe = new DateTime(data[1], data[0], 01);
            DateTime periodoAte = periodoDe.AddMonths(1);
            int ultimoDia = DateTime.DaysInMonth(data[1], data[0]);
            var feriados = db.Feriados.Where(s => s.DataFeriado >= periodoDe && s.DataFeriado < periodoAte).ToList();

            var clientes = db.Clientes.Where(s => s.DataCadastro >= periodoDe && s.DataCadastro < periodoAte).ToList();
            var pedidos = db.Pedidos.Where(s => s.DataPedido >= periodoDe && s.DataPedido < periodoAte).ToList();

            if (idVendedor.HasValue)
            {
                clientes = clientes.Where(s => s.IdVendedor == idVendedor.Value).ToList();
                pedidos = pedidos.Where(s => s.IdVendedor == idVendedor.Value).ToList();
            }

            IList<RelatorioDesempenhoInfo> relatorio = new List<RelatorioDesempenhoInfo>();
            var itemTotal = new RelatorioDesempenhoInfo()
            {
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

            for (int i = 1; i <= ultimoDia; i++)
            {
                var item = new RelatorioDesempenhoInfo();
                item.Data = new DateTime(data[1], data[0], i);
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

                        item.QtdApresentacoes = clientesDia.Where(s => s.Interesse == Models.EnumInteresse.Negociacao).Count();
                        item.ValorApresentacoes = clientesDia.Where(s => s.Interesse == Models.EnumInteresse.Negociacao).Sum(v => v.ValorEstimadoProjeto);
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

            if (total) { relatorio.Add(itemTotal); }

            return relatorio;
        }
    }
}