namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegistradoPor : DbMigration
    {
        public override void Up()
        {
            AddColumn("orc.Atendimentos", "RegistradoPor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("orc.Atendimentos", "RegistradoPor");
        }
    }
}
