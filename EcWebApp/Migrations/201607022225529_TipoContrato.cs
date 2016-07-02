namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TipoContrato : DbMigration
    {
        public override void Up()
        {
            AddColumn("orc.Pedidos", "TipoContrato", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("orc.Pedidos", "TipoContrato");
        }
    }
}
