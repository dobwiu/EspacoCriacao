namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Agenda : DbMigration
    {
        public override void Up()
        {
            AlterColumn("orc.Clientes", "DataUltimoContato", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("orc.Clientes", "DataUltimoContato", c => c.DateTime(nullable: false));
        }
    }
}
