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

namespace EcWebApp.Areas.Orc.Controllers
{
    [Authorize]
    public class PedidosController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        // GET: Orc/Pedidos
        public ActionResult Index()
        {
            var pedidoInfoes = db.Pedidos.Include(p => p.Cliente).Include(p => p.FormaPagamento).Include(p => p.Vendedor).OrderBy(o => o.NumeroPedido);
            return View(pedidoInfoes.ToList());
        }

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

        /// <summary>
        /// Exibe o Pedido para Impressão
        /// </summary>
        public ActionResult ImprimirPedido(Guid id)
        {
            var printPedido = new PrintPedidoInfo()
            {
                Pedido = db.Pedidos.Include(c => c.Cliente).Include(v => v.Vendedor).Include(f => f.FormaPagamento).Where(s => s.IdPedido == id).First(),
                Ambientes = db.Ambientes.Where(s => s.IdPedido == id).OrderBy(o => o.Item).ToList()
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
                IdCliente = pedidoInfo.IdCliente,
                IdVendedor = pedidoInfo.IdVendedor,
                DataPedido = pedidoInfo.DataPedido,
                VersaoProMob = pedidoInfo.VersaoProMob,
                PrazoEntrega = pedidoInfo.PrazoEntrega,
                StatusPedido = pedidoInfo.StatusPedido,
                FasePedido = pedidoInfo.FasePedido,
                ValorOrcamento = pedidoInfo.ValorOrcamento,
                ValorPrazo = pedidoInfo.ValorPrazo,
                IdFormaPagamento = pedidoInfo.IdFormaPagamento,
                CondicaoPagamento = pedidoInfo.CondicaoPagamento,
                NumeroParcelas = pedidoInfo.NumeroParcelas
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
