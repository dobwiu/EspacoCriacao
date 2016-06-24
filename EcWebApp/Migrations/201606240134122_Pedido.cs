namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pedido : DbMigration
    {
        public override void Up()
        {
            AddColumn("orc.Pedidos", "Observacoes", c => c.String(maxLength: 1000));
            AddColumn("orc.Pedidos", "ValorEntrada", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("orc.PedidoAmbientes", "Observacao");
        }
        
        public override void Down()
        {
            AddColumn("orc.PedidoAmbientes", "Observacao", c => c.String(maxLength: 500));
            DropColumn("orc.Pedidos", "ValorEntrada");
            DropColumn("orc.Pedidos", "Observacoes");
        }
    }
}
