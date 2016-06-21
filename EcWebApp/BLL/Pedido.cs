using EcWebApp.DAL;
using EcWebApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EcWebApp.BLL
{
    public class Pedido
    {
        private EspacoContext db = new EspacoContext();

        #region Importação ProMob

        private CultureInfo cultProMob = new CultureInfo("en-US");

        private ArquivoProMobInfo ProcessaArquivo(Guid pIdPedido, string[] lines)
        {
            try
            {
                ArquivoProMobInfo arquivo = new ArquivoProMobInfo() { IdPedido = pIdPedido, Itens = new List<ItemProMobInfo>() };

                int nroLinha = 0;
                foreach (string line in lines)
                {
                    nroLinha++;
                    if (nroLinha <= 10)
                    {
                        /* Linhas do cabeçalho */
                        switch (nroLinha)
                        {
                            case 3:
                                arquivo.Cliente = line.Replace("Cliente: ", "").Trim();
                                break;
                            case 10:
                                arquivo.Versao = line.Replace("Versao: ", "").Trim();
                                break;
                        }
                    }
                    else if (nroLinha > 10 && nroLinha < 15)
                    {
                        /* header da tabela de itens */
                        continue;
                    }
                    else
                    {
                        /* itens do pedido */
                        if (string.IsNullOrEmpty(line) == false)
                        {
                            if (line.Substring(0, 6) == "Total:")
                            {
                                arquivo.Total = Convert.ToDecimal(line.Substring(7).ToString(), cultProMob);
                            }
                            else
                            {
                                ItemProMobInfo item = this.RetornaItem(pIdPedido, line);
                                if (item != null) { arquivo.Itens.Add(item); }
                            }
                        }
                    }
                }

                return arquivo;
            }
            catch
            {
                return null;
            }
        }

        private ItemProMobInfo RetornaItem(Guid pIdPedido, string line)
        {
            try
            {
                return new ItemProMobInfo()
                {
                    Item = Convert.ToInt32(line.Substring(0, 5).Trim()),
                    Qtde = Convert.ToDecimal(line.Substring(5, 5).Trim(), cultProMob),
                    Referencia = line.Substring(10, 16).Trim(),
                    Descricao = line.Substring(26, 45).Trim(),
                    Dimensoes = line.Substring(71, 15).Trim(),
                    PrecoCT = Convert.ToDecimal(line.Substring(86).Trim(), cultProMob),
                    IdPedido = pIdPedido
                };
            }
            catch
            {
                return null;    /* Falhou em alguma conversão */
            }
        }
        #endregion
    }
}