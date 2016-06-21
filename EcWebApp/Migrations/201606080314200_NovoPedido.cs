namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NovoPedido : DbMigration
    {
        public override void Up()
        {
            DropIndex("orc.Pedidos", new[] { "IdFormaPagamento" });
            AlterColumn("orc.Pedidos", "IdFormaPagamento", c => c.Int());
            AlterColumn("orc.Pedidos", "CondicaoPagamento", c => c.Int());
            CreateIndex("orc.Pedidos", "IdFormaPagamento");
        }
        
        public override void Down()
        {
            DropIndex("orc.Pedidos", new[] { "IdFormaPagamento" });
            AlterColumn("orc.Pedidos", "CondicaoPagamento", c => c.Int(nullable: false));
            AlterColumn("orc.Pedidos", "IdFormaPagamento", c => c.Int(nullable: false));
            CreateIndex("orc.Pedidos", "IdFormaPagamento");
        }
    }
}
