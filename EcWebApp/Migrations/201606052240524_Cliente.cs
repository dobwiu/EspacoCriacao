namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cliente : DbMigration
    {
        public override void Up()
        {
            AddColumn("orc.Clientes", "Empreendimento", c => c.String());
            AddColumn("orc.Clientes", "PrazoEntrega", c => c.DateTime());
            CreateIndex("orc.Clientes", "IdStatus");
            AddForeignKey("orc.Clientes", "IdStatus", "dbo.StatusAtendimento", "IdStatus");
        }
        
        public override void Down()
        {
            DropForeignKey("orc.Clientes", "IdStatus", "dbo.StatusAtendimento");
            DropIndex("orc.Clientes", new[] { "IdStatus" });
            DropColumn("orc.Clientes", "PrazoEntrega");
            DropColumn("orc.Clientes", "Empreendimento");
        }
    }
}
