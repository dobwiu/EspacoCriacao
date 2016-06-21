namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "orc.Clientes",
                c => new
                    {
                        IdCliente = c.Guid(nullable: false),
                        NomeCliente = c.String(nullable: false, maxLength: 255),
                        EmailCliente = c.String(maxLength: 255),
                        Telefone01 = c.String(nullable: false, maxLength: 20),
                        Telefone02 = c.String(maxLength: 20),
                        DataNascimento = c.DateTime(),
                        CPFCNPJ = c.String(),
                        Documento = c.String(),
                        Profissao = c.String(),
                        Interesse = c.Int(nullable: false),
                        IdStatus = c.Int(nullable: false),
                        StatusPlanta = c.Int(),
                        DataApresentacao = c.DateTime(),
                        DataMedicao = c.DateTime(),
                        DataUltimoContato = c.DateTime(nullable: false),
                        DataProximoContato = c.DateTime(),
                        Comentarios = c.String(maxLength: 500),
                        Procedencia = c.Int(nullable: false),
                        OutraProcedencia = c.String(),
                        ValorEstimadoProjeto = c.Decimal(precision: 18, scale: 2),
                        IdVendedor = c.Guid(nullable: false),
                        DataCadastro = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IdCliente)
                .ForeignKey("dbo.Usuarios", t => t.IdVendedor)
                .Index(t => t.IdVendedor);
            
            CreateTable(
                "orc.Enderecos",
                c => new
                    {
                        IdEndereco = c.Guid(nullable: false),
                        IdCliente = c.Guid(nullable: false),
                        TipoEndereco = c.Int(nullable: false),
                        Endereco = c.String(maxLength: 255),
                        Complemento = c.String(maxLength: 100),
                        Bairro = c.String(maxLength: 100),
                        Cidade = c.String(maxLength: 100),
                        Estado = c.String(maxLength: 2),
                        CEP = c.String(maxLength: 10),
                        Observacao = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.IdEndereco)
                .ForeignKey("orc.Clientes", t => t.IdCliente)
                .Index(t => t.IdCliente);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        IdUsuario = c.Guid(nullable: false),
                        NomeUsuario = c.String(nullable: false, maxLength: 100),
                        EmailUsuario = c.String(maxLength: 255),
                        LoginUsuario = c.String(nullable: false, maxLength: 50),
                        SenhaUsuario = c.String(nullable: false, maxLength: 50),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdUsuario)
                .Index(t => t.LoginUsuario, unique: true, name: "UK_LoginAcesso");
            
            CreateTable(
                "dbo.Permissoes",
                c => new
                    {
                        IdUsuario = c.Guid(nullable: false),
                        Vendas = c.Int(nullable: false),
                        Relatorios = c.Int(nullable: false),
                        Financeiro = c.Int(nullable: false),
                        Administrativo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdUsuario)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "fin.PlanoContas",
                c => new
                    {
                        IdConta = c.Guid(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 50),
                        DataAberturaConta = c.DateTime(nullable: false),
                        SaldoInicial = c.Decimal(precision: 18, scale: 2),
                        SaldoAtual = c.Decimal(precision: 18, scale: 2),
                        DataUltimaAtualizacao = c.DateTime(nullable: false),
                        Ativo = c.Boolean(nullable: false),
                        IdUsuario = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.IdConta)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Dicionario",
                c => new
                    {
                        Enumerador = c.String(nullable: false, maxLength: 128),
                        Valor = c.Int(nullable: false),
                        Texto = c.String(maxLength: 100),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.Enumerador, t.Valor });
            
            CreateTable(
                "dbo.Feriados",
                c => new
                    {
                        IdFeriado = c.Int(nullable: false, identity: true),
                        DataFeriado = c.DateTime(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.IdFeriado)
                .Index(t => t.DataFeriado, unique: true, name: "UK_DataFeriado");
            
            CreateTable(
                "dbo.FormasPagamento",
                c => new
                    {
                        IdFormaPagto = c.Int(nullable: false, identity: true),
                        Descricao = c.String(nullable: false, maxLength: 50),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdFormaPagto);
            
            CreateTable(
                "fin.Lancamentos",
                c => new
                    {
                        IdLancamento = c.Int(nullable: false, identity: true),
                        IdConta = c.Guid(nullable: false),
                        TipoLancamento = c.String(nullable: false, maxLength: 1),
                        Descricao = c.String(nullable: false, maxLength: 50),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DataProcessamento = c.DateTime(nullable: false),
                        Processado = c.Boolean(nullable: false),
                        IdUsuario = c.Guid(nullable: false),
                        DataLancamento = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IdLancamento)
                .ForeignKey("fin.PlanoContas", t => t.IdConta)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario)
                .Index(t => t.IdConta)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Metas",
                c => new
                    {
                        IdMeta = c.Int(nullable: false, identity: true),
                        Mes = c.Int(nullable: false),
                        Ano = c.Int(nullable: false),
                        ValorMeta = c.Decimal(precision: 18, scale: 2),
                        ValorDesafio = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IdMeta)
                .Index(t => new { t.Mes, t.Ano }, unique: true, name: "UK_PeriodoMeta");
            
            CreateTable(
                "orc.Pedidos",
                c => new
                    {
                        IdPedido = c.Guid(nullable: false),
                        NumeroPedido = c.String(nullable: false, maxLength: 20),
                        IdCliente = c.Guid(nullable: false),
                        IdVendedor = c.Guid(nullable: false),
                        DataPedido = c.DateTime(nullable: false),
                        VersaoProMob = c.String(),
                        PrazoEntrega = c.DateTime(),
                        StatusPedido = c.Int(nullable: false),
                        FasePedido = c.Int(),
                        ValorOrcamento = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ValorPrazo = c.Decimal(precision: 18, scale: 2),
                        IdFormaPagamento = c.Int(nullable: false),
                        CondicaoPagamento = c.Int(nullable: false),
                        NumeroParcelas = c.Int(),
                    })
                .PrimaryKey(t => t.IdPedido)
                .ForeignKey("orc.Clientes", t => t.IdCliente)
                .ForeignKey("dbo.FormasPagamento", t => t.IdFormaPagamento)
                .ForeignKey("dbo.Usuarios", t => t.IdVendedor)
                .Index(t => t.IdCliente)
                .Index(t => t.IdVendedor)
                .Index(t => t.IdFormaPagamento);
            
            CreateTable(
                "orc.PedidoAmbientes",
                c => new
                    {
                        IdAmbiente = c.Guid(nullable: false),
                        Item = c.Int(nullable: false),
                        Qtde = c.Int(nullable: false),
                        Descricao = c.String(),
                        Fornecedor = c.String(),
                        Linha = c.String(),
                        Prazo = c.Int(),
                        Valor = c.Decimal(precision: 18, scale: 2),
                        Corpo = c.String(),
                        Porta = c.String(),
                        Puxador = c.String(),
                        Complemento = c.String(),
                        Modelo = c.String(),
                        Observacao = c.String(),
                        IdPedido = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.IdAmbiente)
                .ForeignKey("orc.Pedidos", t => t.IdPedido)
                .Index(t => t.IdPedido);
            
            CreateTable(
                "orc.PedidoItensProMob",
                c => new
                    {
                        IdItem = c.Guid(nullable: false),
                        Item = c.Int(nullable: false),
                        Qtde = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Referencia = c.String(),
                        Descricao = c.String(),
                        Dimensoes = c.String(),
                        PrecoCT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdAmbiente = c.Guid(),
                        IdPedido = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.IdItem)
                .ForeignKey("orc.Pedidos", t => t.IdPedido)
                .Index(t => t.IdPedido);
            
            CreateTable(
                "dbo.StatusAtendimento",
                c => new
                    {
                        IdStatus = c.Int(nullable: false, identity: true),
                        Descricao = c.String(nullable: false, maxLength: 50),
                        TipoStatus = c.Int(nullable: false),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdStatus);
            
        }
        
        public override void Down()
        {
            DropForeignKey("orc.Pedidos", "IdVendedor", "dbo.Usuarios");
            DropForeignKey("orc.PedidoItensProMob", "IdPedido", "orc.Pedidos");
            DropForeignKey("orc.Pedidos", "IdFormaPagamento", "dbo.FormasPagamento");
            DropForeignKey("orc.Pedidos", "IdCliente", "orc.Clientes");
            DropForeignKey("orc.PedidoAmbientes", "IdPedido", "orc.Pedidos");
            DropForeignKey("fin.Lancamentos", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("fin.Lancamentos", "IdConta", "fin.PlanoContas");
            DropForeignKey("fin.PlanoContas", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("orc.Clientes", "IdVendedor", "dbo.Usuarios");
            DropForeignKey("dbo.Permissoes", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("orc.Enderecos", "IdCliente", "orc.Clientes");
            DropIndex("orc.PedidoItensProMob", new[] { "IdPedido" });
            DropIndex("orc.PedidoAmbientes", new[] { "IdPedido" });
            DropIndex("orc.Pedidos", new[] { "IdFormaPagamento" });
            DropIndex("orc.Pedidos", new[] { "IdVendedor" });
            DropIndex("orc.Pedidos", new[] { "IdCliente" });
            DropIndex("dbo.Metas", "UK_PeriodoMeta");
            DropIndex("fin.Lancamentos", new[] { "IdUsuario" });
            DropIndex("fin.Lancamentos", new[] { "IdConta" });
            DropIndex("dbo.Feriados", "UK_DataFeriado");
            DropIndex("fin.PlanoContas", new[] { "IdUsuario" });
            DropIndex("dbo.Permissoes", new[] { "IdUsuario" });
            DropIndex("dbo.Usuarios", "UK_LoginAcesso");
            DropIndex("orc.Enderecos", new[] { "IdCliente" });
            DropIndex("orc.Clientes", new[] { "IdVendedor" });
            DropTable("dbo.StatusAtendimento");
            DropTable("orc.PedidoItensProMob");
            DropTable("orc.PedidoAmbientes");
            DropTable("orc.Pedidos");
            DropTable("dbo.Metas");
            DropTable("fin.Lancamentos");
            DropTable("dbo.FormasPagamento");
            DropTable("dbo.Feriados");
            DropTable("dbo.Dicionario");
            DropTable("fin.PlanoContas");
            DropTable("dbo.Permissoes");
            DropTable("dbo.Usuarios");
            DropTable("orc.Enderecos");
            DropTable("orc.Clientes");
        }
    }
}
