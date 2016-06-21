namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemPedidos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "orc.Cheques",
                c => new
                    {
                        IdCheque = c.Int(nullable: false, identity: true),
                        IdPedido = c.Guid(nullable: false),
                        IdCliente = c.Guid(nullable: false),
                        Parcela = c.Int(nullable: false),
                        Numero = c.String(maxLength: 25),
                        Data = c.DateTime(nullable: false),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Compensado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdCheque);
            
            CreateTable(
                "orc.PedidoObservacoes",
                c => new
                    {
                        IdObservacao = c.Guid(nullable: false),
                        Item = c.Int(nullable: false),
                        Observacao = c.String(maxLength: 500),
                        IdPedido = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.IdObservacao)
                .ForeignKey("orc.Pedidos", t => t.IdPedido)
                .Index(t => t.IdPedido);
            
            AlterColumn("orc.PedidoAmbientes", "Fornecedor", c => c.String(maxLength: 100));
            AlterColumn("orc.PedidoAmbientes", "Linha", c => c.String(maxLength: 100));
            AlterColumn("orc.PedidoAmbientes", "Corpo", c => c.String(maxLength: 100));
            AlterColumn("orc.PedidoAmbientes", "Porta", c => c.String(maxLength: 100));
            AlterColumn("orc.PedidoAmbientes", "Puxador", c => c.String(maxLength: 100));
            AlterColumn("orc.PedidoAmbientes", "Complemento", c => c.String(maxLength: 100));
            AlterColumn("orc.PedidoAmbientes", "Modelo", c => c.String(maxLength: 100));
            DropColumn("orc.PedidoAmbientes", "Observacao");
        }
        
        public override void Down()
        {
            AddColumn("orc.PedidoAmbientes", "Observacao", c => c.String());
            DropForeignKey("orc.PedidoObservacoes", "IdPedido", "orc.Pedidos");
            DropIndex("orc.PedidoObservacoes", new[] { "IdPedido" });
            AlterColumn("orc.PedidoAmbientes", "Modelo", c => c.String());
            AlterColumn("orc.PedidoAmbientes", "Complemento", c => c.String());
            AlterColumn("orc.PedidoAmbientes", "Puxador", c => c.String());
            AlterColumn("orc.PedidoAmbientes", "Porta", c => c.String());
            AlterColumn("orc.PedidoAmbientes", "Corpo", c => c.String());
            AlterColumn("orc.PedidoAmbientes", "Linha", c => c.String());
            AlterColumn("orc.PedidoAmbientes", "Fornecedor", c => c.String());
            DropTable("orc.PedidoObservacoes");
            DropTable("orc.Cheques");
        }
    }
}
