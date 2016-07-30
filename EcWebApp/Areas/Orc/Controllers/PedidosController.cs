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
using EcWebApp.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace EcWebApp.Areas.Orc.Controllers
{
    [Authorize]
    public class PedidosController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        // GET: Orc/Pedidos
        public ActionResult Index()
        {
            var pedidoInfoes = db.Pedidos.Include(p => p.Cliente)
                                 .Include(p => p.FormaPagamento).Include(p => p.Vendedor)
                                 .Where(s => s.Cliente.Interesse != EnumInteresse.SemInteresse)
                                 .OrderByDescending(o => o.NumeroPedido);

            ViewBag.IdVendedor = new SelectList(new BLL.Usuario().ListarUsuarioCombo(), "IdUsuario", "NomeUsuario");
            return View(pedidoInfoes.ToList());
        }

        #region CRUD..
        // GET: Orc/Pedidos/Create
        public ActionResult Create(Guid id)
        {
            var cliente = db.Clientes.Include(i => i.Vendedor).Where(s => s.IdCliente == id).FirstOrDefault();
            if (cliente == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            var pedido = db.Pedidos.Include(p => p.Cliente).Include(p => p.Vendedor)
                           .Where(s => s.IdCliente == id && s.StatusPedido != EnumStatusPedido.Cancelado)
                           .FirstOrDefault();

            //-- Se não houver pedido para o cliente, cria um novo..
            if (pedido == null)
            {
                pedido = new PedidoInfo()
                {
                    IdPedido = Guid.NewGuid(),
                    NumeroPedido = this.RetornaNumeroPedido(),
                    IdCliente = id,
                    Cliente = cliente,
                    IdVendedor = cliente.IdVendedor,
                    Vendedor = cliente.Vendedor,
                    DataPedido = DateTime.Today,
                    StatusPedido = EnumStatusPedido.Aberto,
                    ValorOrcamento = 0
                };

                db.Pedidos.Add(pedido);
                db.SaveChanges();
            }

            ViewBag.Origem = "Cliente";
            ViewBag.IdFormaPagamento = new SelectList(db.FormasPagamento, "IdFormaPagto", "Descricao", pedido.IdFormaPagamento);
            return View("Details", pedido);
        }

        // POST: Orc/Pedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PedidoInfo pedidoInfo)
        {
            /* Remove as entidades relacionadas da entidade */
            PedidoInfo entity = this.RetornaEntidadePedido(pedidoInfo);

            try
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                string messages = string.Join("; ", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                ViewBag.MsgErro = strMsgErro + messages;
            }

            ViewBag.Origem = "Cliente";
            ViewBag.IdFormaPagamento = new SelectList(db.FormasPagamento, "IdFormaPagto", "Descricao", pedidoInfo.IdFormaPagamento);
            return View("Details", pedidoInfo);
        }

        // GET: Orc/Pedidos/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PedidoInfo pedidoInfo = db.Pedidos.Include(p => p.Cliente).Include(p => p.Vendedor)
                                      .Where(s => s.IdPedido == id && s.StatusPedido != EnumStatusPedido.Cancelado)
                                      .FirstOrDefault();

            if (pedidoInfo == null)
            {
                return HttpNotFound();
            }

            ViewBag.Origem = "Pedido";
            ViewBag.IdFormaPagamento = new SelectList(db.FormasPagamento, "IdFormaPagto", "Descricao", pedidoInfo.IdFormaPagamento);
            return View("Details", pedidoInfo);
        }

        // POST: Orc/Pedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PedidoInfo pedidoInfo)
        {
            /* Remove as entidades relacionadas da entidade */
            PedidoInfo entity = this.RetornaEntidadePedido(pedidoInfo);

            try
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch //(Exception ex)
            {
                string messages = string.Join("; ", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                ViewBag.MsgErro = strMsgErro + messages;
            }

            ViewBag.Origem = "Pedido";
            ViewBag.IdFormaPagamento = new SelectList(db.FormasPagamento, "IdFormaPagto", "Descricao", pedidoInfo.IdFormaPagamento);
            return View("Details", pedidoInfo);
        }

        public JsonResult EncerraPedido(Guid id)
        {
            return this.AlteraStatusPedido(id, EnumStatusPedido.Fechado);
        }

        public JsonResult CancelaPedido(Guid id)
        {
            return this.AlteraStatusPedido(id, EnumStatusPedido.Cancelado);
        }
        #endregion

        #region Impressão..
        /// <summary>
        /// Exibe o Pedido para Impressão
        /// </summary>
        public ActionResult ImprimirPedido(Guid id)
        {
            var printPedido = new PrintPedidoInfo()
            {
                Pedido = db.Pedidos.Include(c => c.Cliente).Include(v => v.Vendedor).Include(f => f.FormaPagamento).Where(s => s.IdPedido == id).First(),
                Ambientes = db.Ambientes.Where(s => s.IdPedido == id).OrderBy(o => o.Item).ToList(),
                Parcelas = db.Parcelas.Where(p => p.IdPedido == id).ToList()
            };

            //-- Endereço de Entrega -->
            printPedido.Entrega = db.Enderecos.Where(s => s.IdCliente == printPedido.Pedido.IdCliente && s.TipoEndereco == EnumTipoEndereco.Entrega).FirstOrDefault();

            return View("PrintPedido", printPedido);
        }

        /// <summary>
        /// Mostra o Contrato do Cliente
        /// </summary>
        public ActionResult ImprimirContrato(Guid id)
        {
            var pedidoInfo = db.Pedidos.Include(x => x.Cliente).Where(s => s.IdPedido == id).FirstOrDefault();
            var printContrato = new PrintContratoInfo()
            {
                NumeroContrato = pedidoInfo.NumeroPedido,
                NomeCliente = pedidoInfo.Cliente.NomeCliente,
                DataContrato = pedidoInfo.DataPedido
            };

            return View("PrintContrato", printContrato);
        }
        #endregion

        #region Exportação..
        public FileResult Exportar(ExportacaoInfo filtro)
        {
            string path = HttpContext.Server.MapPath("~/Content/modelos/ExportaPedidos.xlsx");
            System.IO.FileInfo modeloXLS = new System.IO.FileInfo(path);

            using (ExcelPackage xls = new ExcelPackage(modeloXLS))
            {
                var xlsPedidos = db.Pedidos.Include(c => c.Cliente).Include(v => v.Vendedor).Include(f => f.FormaPagamento);
                #region Filtros..
                if (filtro.DataPedidoDe.HasValue)
                    xlsPedidos = xlsPedidos.Where(s => s.DataPedido >= filtro.DataPedidoDe.Value);

                if (filtro.DataPedidoAte.HasValue)
                {
                    DateTime dtFinal = filtro.DataPedidoAte.Value.AddDays(1);
                    xlsPedidos = xlsPedidos.Where(s => s.DataPedido < dtFinal);
                }

                if (filtro.IdVendedor.HasValue)
                    xlsPedidos = xlsPedidos.Where(s => s.IdVendedor == filtro.IdVendedor.Value);

                if (filtro.StatusPedido.HasValue)
                    xlsPedidos = xlsPedidos.Where(s => s.StatusPedido == filtro.StatusPedido.Value);
                #endregion
                var xlsListaPedidos = xlsPedidos.OrderBy(o => o.NumeroPedido).ToList();

                int linha = 6;
                ExcelWorksheet ws = xls.Workbook.Worksheets["Pedidos"];
                foreach (var item in xlsListaPedidos)
                {
                    ws.Cells[linha, 02].Value = item.NumeroPedido;
                    ws.Cells[linha, 03].Value = item.Cliente.NomeCliente;
                    ws.Cells[linha, 04].Value = item.DataPedido.ToShortDateString();
                    ws.Cells[linha, 05].Value = item.Vendedor.NomeUsuario;
                    ws.Cells[linha, 06].Value = item.DataParaEntrega;
                    ws.Cells[linha, 07].Value = item.StatusPedido.ToString();
                    ws.Cells[linha, 08].Value = item.ValorOrcamento;
                    ws.Cells[linha, 09].Value = item.FormaPagamento.Descricao;
                    ws.Cells[linha, 10].Value = item.CondicaoPagamento;
                    ws.Cells[linha, 11].Value = item.ValorEntrada;
                    ws.Cells[linha, 12].Value = item.NumeroParcelas;
                    ws.Cells[linha, 13].Value = item.ValorPrazo;
                    ws.Cells[linha, 14].Value = item.Observacoes;

                    //ws.Cells[linha, 08].Style.Numberformat.Format = "#.##0,00";
                    ws.Cells[linha, 08].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //ws.Cells[linha, 11].Style.Numberformat.Format = "#.##0,00";
                    ws.Cells[linha, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //ws.Cells[linha, 13].Style.Numberformat.Format = "#.##0,00";
                    ws.Cells[linha, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[linha, 14].Style.WrapText = true;
                    linha++;
                }

                return File(xls.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Pedidos.xlsx");
            }
        }
        #endregion

        #region Private Methods..
        private string RetornaNumeroPedido()
        {
            int qtdPedidos = db.Pedidos.Count() + 1;
            return qtdPedidos.ToString("00000");
            //return string.Format("{0}/{1}", qtdPedidos.ToString("00000"), DateTime.Now.Year);
        }

        /// <summary>
        /// Retorna uma nova Entidade de Pedido, sem as entidades relacionadas (EF)
        /// </summary>
        private PedidoInfo RetornaEntidadePedido(PedidoInfo pedidoInfo)
        {
            return new PedidoInfo()
            {
                IdPedido = pedidoInfo.IdPedido,
                NumeroPedido = pedidoInfo.NumeroPedido,
                TipoContrato = pedidoInfo.TipoContrato,
                IdCliente = pedidoInfo.IdCliente,
                IdVendedor = pedidoInfo.IdVendedor,
                DataPedido = pedidoInfo.DataPedido,
                VersaoProMob = pedidoInfo.VersaoProMob,
                PrazoEntrega = pedidoInfo.PrazoEntrega,
                StatusPedido = pedidoInfo.StatusPedido,
                FasePedido = pedidoInfo.FasePedido,
                ValorOrcamento = pedidoInfo.ValorOrcamento,
                ValorEntrada = pedidoInfo.ValorEntrada,
                ValorPrazo = pedidoInfo.ValorPrazo,
                IdFormaPagamento = pedidoInfo.IdFormaPagamento,
                CondicaoPagamento = pedidoInfo.CondicaoPagamento,
                NumeroParcelas = pedidoInfo.NumeroParcelas,
                Observacoes = pedidoInfo.Observacoes
            };
        }

        private JsonResult AlteraStatusPedido(Guid pIdPedido, EnumStatusPedido pEnumStatus)
        {
            var pedidoInfo = db.Pedidos.Find(pIdPedido);
            pedidoInfo.StatusPedido = pEnumStatus;

            db.Entry(pedidoInfo).State = EntityState.Modified;
            db.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AngularJS..
        [HttpGet]
        public JsonResult GetAmbientes(Guid id)
        {
            return Json(lstAmbientes(id), JsonRequestBehavior.AllowGet);
        }

        private IList<AmbienteInfo> lstAmbientes(Guid idPedido)
        {
            return db.Ambientes.Where(s => s.IdPedido == idPedido).OrderBy(o => o.Item).ToList();
        }

        public JsonResult IncluirAmbiente(AmbienteInfo novoAmbiente)
        {
            novoAmbiente.IdAmbiente = Guid.NewGuid();
            novoAmbiente.Item = db.Ambientes.Where(s => s.IdPedido == novoAmbiente.IdPedido).Count() + 1;
            db.Ambientes.Add(novoAmbiente);
            db.SaveChanges();

            return Json(lstAmbientes(novoAmbiente.IdPedido), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExcluirAmbiente(AmbienteInfo delAmbiente)
        {
            Guid pIdPedido = delAmbiente.IdPedido;

            /* exclusao */
            var item = db.Ambientes.Find(delAmbiente.IdAmbiente);
            db.Ambientes.Remove(item);
            db.SaveChanges();

            return Json(lstAmbientes(pIdPedido), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetParcelas(Guid id)
        {
            return Json(lstParcelas(id), JsonRequestBehavior.AllowGet);
        }

        private IList<ParcelaInfo> lstParcelas(Guid idPedido)
        {
            return db.Parcelas.Where(s => s.IdPedido == idPedido).OrderBy(o => o.NumeroParcela).ToList();
        }

        public JsonResult IncluirParcela(ParcelaInfo novaParcela)
        {
            novaParcela.Contabilizado = false;
            //novaParcela.NumeroParcela = db.Parcelas.Where(s => s.IdPedido == novaParcela.IdPedido).Count() + 1;
            db.Parcelas.Add(novaParcela);
            db.SaveChanges();

            return Json(lstParcelas(novaParcela.IdPedido), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExcluirParcela(ParcelaInfo delParcela)
        {
            Guid pIdPedido = delParcela.IdPedido;

            /* exclusao */
            var item = db.Parcelas.Find(delParcela.IdParcela);
            db.Parcelas.Remove(item);
            db.SaveChanges();

            return Json(lstParcelas(pIdPedido), JsonRequestBehavior.AllowGet);
        }
        #endregion

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
