using EcWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcWebApp.ViewModels
{
    public class ExtratoInfo
    {
        public ExtratoInfo()
        {
            Lancamentos = new List<LancamentoInfo>();
            LancFuturos = new List<LancamentoInfo>();
        }

        public Guid IdConta { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime PeriodoDe { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime PeriodoAte { get; set; }

        [DataType(DataType.Currency)]
        public decimal SaldoAnterior { get; set; }

        [DataType(DataType.Currency)]
        public decimal Recebimentos { get; set; }

        [DataType(DataType.Currency)]
        public decimal Pagamentos { get; set; }

        [DataType(DataType.Currency)]
        public decimal SaldoAtual { get; set; }

        public IList<LancamentoInfo> Lancamentos { get; set; }

        public IList<LancamentoInfo> LancFuturos { get; set; }

    }
}