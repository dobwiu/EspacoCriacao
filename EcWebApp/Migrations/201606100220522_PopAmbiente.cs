namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopAmbiente : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("orc.PedidoObservacoes", "IdPedido", "orc.Pedidos");
            DropIndex("orc.PedidoObservacoes", new[] { "IdPedido" });
            AddColumn("orc.PedidoAmbientes", "Portas", c => c.String(maxLength: 100));
            AddColumn("orc.PedidoAmbientes", "Espessura", c => c.String(maxLength: 50));
            AddColumn("orc.PedidoAmbientes", "Observacao", c => c.String(maxLength: 500));
            AlterColumn("orc.PedidoAmbientes", "Qtde", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("orc.PedidoAmbientes", "Descricao", c => c.String(maxLength: 100));
            DropColumn("orc.PedidoAmbientes", "Porta");
            DropTable("orc.PedidoObservacoes");
        }
        
        public override void Down()
        {
            CreateTable(
                "orc.PedidoObservacoes",
                c => new
                    {
                        IdObservacao = c.Guid(nullable: false),
                        Item = c.Int(nullable: false),
                        Observacao = c.String(maxLength: 500),
                        IdPedido = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.IdObservacao);
            
            AddColumn("orc.PedidoAmbientes", "Porta", c => c.String(maxLength: 100));
            AlterColumn("orc.PedidoAmbientes", "Descricao", c => c.String());
            AlterColumn("orc.PedidoAmbientes", "Qtde", c => c.Int(nullable: false));
            DropColumn("orc.PedidoAmbientes", "Observacao");
            DropColumn("orc.PedidoAmbientes", "Espessura");
            DropColumn("orc.PedidoAmbientes", "Portas");
            CreateIndex("orc.PedidoObservacoes", "IdPedido");
            AddForeignKey("orc.PedidoObservacoes", "IdPedido", "orc.Pedidos", "IdPedido");
        }
    }
}
