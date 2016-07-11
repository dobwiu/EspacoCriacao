using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcWebApp.ViewModels
{
    public class RelatorioDesempenhoInfo
    {
        public DateTime Data { get; set; }

        public string Vendedor { get; set; }

        public int? QtdProcompra { get; set; }

        public int? QtdPorta { get; set; }

        public int? QtdOutros { get; set; }

        public int? QtdApresentacoes { get; set; }

        public Decimal? ValorApresentacoes { get; set; }

        public int? QtdPedidos { get; set; }

        public Decimal? ValorPedidos { get; set; }

        public int? QtdPerdidos { get; set; }

        public Decimal? ValorPerdidos { get; set; }

        public bool DiaUtil { get; set; }

        public bool LinhaTotal { get; set; }

        public int Atendimentos
        {
            get
            {
                return QtdProcompra.GetValueOrDefault(0)
                     + QtdPorta.GetValueOrDefault(0)
                     + QtdOutros.GetValueOrDefault(0);
            }
        }
        
    }
}