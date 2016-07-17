using EcWebApp.DAL;
using EcWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcWebApp.Controllers
{
    [Authorize]
    public class HomeController : _BaseController
    {
        private EspacoContext db = new EspacoContext();

        public ActionResult Index()
        {
            DashboardInfo dash = new DashboardInfo();
            var lstClientes = db.Clientes.Where(s => s.DataCadastro.Month == DateTime.Today.Month && s.DataCadastro.Year == DateTime.Today.Year).ToList();
            dash.NovosClientes = lstClientes.Count();
            dash.Agendamentos = db.Clientes.Where(s => s.Interesse == Models.EnumInteresse.Orcamento).Count();
            dash.Atrasos = db.Clientes.Where(s => s.DataProximoContato <= DateTime.Now).Any();

            try
            {
                dash.ValorOrcamentos = lstClientes.Where(w => w.ValorEstimadoProjeto.HasValue).Sum(s => s.ValorEstimadoProjeto.Value);
            }
            catch
            {
                dash.ValorOrcamentos = null;
            }

            try
            {
                var metaMes = db.Metas.Where(s => s.Mes == DateTime.Today.Month && s.Ano == DateTime.Today.Year).FirstOrDefault();
                if (metaMes != null)
                {
                    if (metaMes.ValorMeta.HasValue && metaMes.ValorMeta.GetValueOrDefault(0) > 0)
                    {
                        var valorPedidos = db.Pedidos.Where(s => s.DataPedido.Month == DateTime.Today.Month && s.DataPedido.Year == DateTime.Today.Year).Sum(x => x.ValorOrcamento);
                        dash.MetaVendas = Math.Round((valorPedidos * 100 / metaMes.ValorMeta.Value), 2);

                    }
                }
            }
            catch
            {
                dash.MetaVendas = null;
            }

            return View(dash);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}