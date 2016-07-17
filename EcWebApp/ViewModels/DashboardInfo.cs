using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcWebApp.ViewModels
{
    public class DashboardInfo
    {
        public int NovosClientes { get; set; }

        public int Agendamentos { get; set; }

        public Decimal? ValorOrcamentos { get; set; }

        public Decimal? MetaVendas { get; set; }

        public bool Atrasos { get; set; }
    }
}