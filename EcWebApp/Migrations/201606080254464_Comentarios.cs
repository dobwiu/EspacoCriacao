namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Comentarios : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "orc.Parcelas",
                c => new
                    {
                        IdParcela = c.Int(nullable: false, identity: true),
                        IdPedido = c.Guid(nullable: false),
                        IdCliente = c.Guid(nullable: false),
                        NumeroParcela = c.Int(nullable: false),
                        NumeroCheque = c.String(maxLength: 25),
                        DataPagamento = c.DateTime(),
                        ValorParcela = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Contabilizado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdParcela)
                .ForeignKey("orc.Pedidos", t => t.IdPedido)
                .Index(t => t.IdPedido);
            
            DropTable("orc.Cheques");
        }
        
        public override void Down()
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
            
            DropForeignKey("orc.Parcelas", "IdPedido", "orc.Pedidos");
            DropIndex("orc.Parcelas", new[] { "IdPedido" });
            DropTable("orc.Parcelas");
        }
    }
}
