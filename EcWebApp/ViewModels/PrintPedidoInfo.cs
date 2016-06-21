using EcWebApp.Models;
using System;
using System.Collections.Generic;

namespace EcWebApp.ViewModels
{
    public class PrintPedidoInfo
    {
        public PedidoInfo Pedido { get; set; }

        public EnderecoInfo Entrega {get; set;}
        
        public ICollection<AmbienteInfo> Ambientes { get; set; }
    }

    public class PrintContratoInfo
    {
        public string NumeroContrato {get; set;}
        
        public DateTime DataContrato {get; set;}

        public string NomeCliente {get; set;}
    }
}