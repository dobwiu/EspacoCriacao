using EcWebApp.Models;
using System;


namespace EcWebApp.ViewModels
{
    public class ExportacaoInfo
    {
        public DateTime? DataCadastroDe { get; set; }
        public DateTime? DataCadastroAte { get; set; }

        public DateTime? DataPedidoDe { get; set; }
        public DateTime? DataPedidoAte { get; set; }

        public Guid? IdVendedor { get; set; }

        public EnumInteresse? Interesse { get; set; }

        public EnumStatusPedido? StatusPedido { get; set; }

        public int? IdStatusAtendimento { get; set; }

        public EnumStatusPlanta? StatusPlanta { get; set; }

        public int? IdProcedencia { get; set; }
    }
}