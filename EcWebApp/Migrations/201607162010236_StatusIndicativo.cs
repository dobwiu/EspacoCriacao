namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StatusIndicativo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StatusAtendimento", "Apresentacao", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StatusAtendimento", "Apresentacao");
        }
    }
}
