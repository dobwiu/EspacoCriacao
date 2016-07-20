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
    public class RelPerdidosController : Controller
    {
        private EspacoContext db = new EspacoContext();

        // GET: Reports/RelDesempenho
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Exibir(string pPeriodo)
        {
            string[] datas = pPeriodo.Split('-');
            DateTime periodoDe = Convert.ToDateTime(datas[0].Trim());
            DateTime periodoAte = Convert.ToDateTime(datas[1].Trim()).AddDays(1);

            var relatorio = this.Gerar(periodoDe, periodoAte);
            return PartialView("pvRelPerdidos", relatorio);
        }

        private IList<RelatorioPerdidosInfo> Gerar(DateTime periodoDe, DateTime periodoAte)
        {
            var motivos = from c in db.Clientes
                          join s in db.StatusAtendimento on c.IdStatus equals s.IdStatus
                          where c.Interesse == Models.EnumInteresse.SemInteresse
                          && (c.DataCadastro >= periodoDe && c.DataCadastro <= periodoAte)
                          group s by s.Descricao into X
                          select new RelatorioPerdidosInfo() { Motivo = X.Key, Qtde = X.Count() };

            //var motivos = db.Clientes.Include(i => i.StatusAtendimento)
            //    .Where(c => c.Interesse == Models.EnumInteresse.SemInteresse)
            //    .GroupBy(g => g.StatusAtendimento.Descricao)
            //    .Select(s => new RelatorioPerdidosInfo() { Motivo = s.Key, Qtde = s.Count() })
            //    .ToList();

            return motivos.ToList();
        }

        public FileResult Exportar(FormCollection filtros)
        {
            string[] datas = filtros[0].Split('-');
            DateTime periodoDe = Convert.ToDateTime(datas[0].Trim());
            DateTime periodoAte = Convert.ToDateTime(datas[1].Trim()).AddDays(1);

            string path = HttpContext.Server.MapPath("~/Content/modelos/RelPerdidos.xlsx");
            System.IO.FileInfo modeloXLS = new System.IO.FileInfo(path);

            using (ExcelPackage xls = new ExcelPackage(modeloXLS))
            {
                var relatorio = this.Gerar(periodoDe, periodoAte);

                int linha = 6; int total = 0;
                ExcelWorksheet ws = xls.Workbook.Worksheets["Relatorio"];
                ws.Cells[4, 2].Value = string.Format("Período: {0}", filtros[0]);

                foreach (var item in relatorio)
                {
                    ws.Cells[linha, 02].Value = item.Motivo;
                    ws.Cells[linha, 03].Value = item.Qtde;

                    total += item.Qtde;
                    linha++;
                }

                using (ExcelRange rng = ws.Cells[linha, 2, linha, 03])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                }

                ws.Cells[linha, 02].Value = "TOTAL";
                ws.Cells[linha, 03].Value = total;

                return File(xls.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Indic-ClientesPerdidos.xlsx");
            }
        }
    }
}
