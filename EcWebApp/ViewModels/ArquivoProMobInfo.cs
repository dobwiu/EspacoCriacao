using EcWebApp.Models;
using System;
using System.Collections.Generic;

/// <summary>
/// Entidade para a carga do arquivo do ProMob
/// </summary>
public class ArquivoProMobInfo
{
    public Guid IdPedido { get; set; }

    public string Cliente { get; set; }

    public string Versao { get; set; }

    public Decimal? Total { get; set; }

    public ICollection<ItemProMobInfo> Itens { get; set; }
}