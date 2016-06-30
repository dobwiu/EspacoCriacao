namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CondicaoPagamento : DbMigration
    {
        public override void Up()
        {
            AlterColumn("orc.Pedidos", "CondicaoPagamento", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("orc.Pedidos", "CondicaoPagamento", c => c.Int());
        }
    }
}
