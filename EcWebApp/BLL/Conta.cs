using EcWebApp.DAL;
using EcWebApp.Models;
using EcWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EcWebApp.BLL
{
    public class Conta
    {
        public IList<JobContasInfo> ProcessaContas()
        {
            EspacoContext db = new EspacoContext();
            IList<JobContasInfo> jobs = new List<JobContasInfo>();
            jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = "Iniciando.." });

            var contas = db.Contas.Where(s => s.Ativo).ToList();
            jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("Processando {0} contas", contas.Count()) });
            foreach (var conta in contas)
            {
                var dtProcessa = DateTime.Today.AddDays(-1); // 1 dia antes, devido ao fuso horário do Azure.
                var lances = db.Lancamentos.Where(s => s.IdConta == conta.IdConta && s.DataProcessamento <= dtProcessa && s.Processado == false).ToList();
                jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("Conta: {0} / {1} lançamentos", conta.Descricao, lances.Count()) });
                jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("Saldo Inicial:{0}", conta.SaldoAtual) });
                foreach (var item in lances)
                {
                    conta.SaldoAtual += item.Valor;
                    item.Processado = true;
                    db.Entry(item).State = EntityState.Modified;
                }
                jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("Saldo Final:{0}", conta.SaldoAtual) });

                var ultimoDia = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                //if (DateTime.Today.Day == ultimoDia)
                if (dtProcessa.Day >= ultimoDia)
                {
                    var fechamento = new FechamentoInfo()
                    {
                        IdConta = conta.IdConta.Value,
                        DataFechamento = dtProcessa,
                        ValorFechamento = conta.SaldoAtual.Value
                    };
                    jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("Inserindo Fechamento: {0}", DateTime.Today) });
                    db.Fechamentos.Add(fechamento);
                    conta.SaldoAnterior = conta.SaldoAtual;
                }

                conta.DataUltimaAtualizacao = dtProcessa; // DateTime.Today
                db.Entry(conta).State = EntityState.Modified;
                db.SaveChanges();
                jobs.Add(new JobContasInfo() { DataHora = DateTime.Now, Mensagem = string.Format("{0} atualizada.", conta.Descricao) });
            }

            return jobs;
        }
    }
}