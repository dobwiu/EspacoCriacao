namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ajustes : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "dbo.CategoriasLancamento", newSchema: "fin");
        }
        
        public override void Down()
        {
            MoveTable(name: "fin.CategoriasLancamento", newSchema: "dbo");
        }
    }
}
