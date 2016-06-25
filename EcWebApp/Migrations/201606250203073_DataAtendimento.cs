namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataAtendimento : DbMigration
    {
        public override void Up()
        {
            AddColumn("orc.Clientes", "DataPrimeiroContato", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("orc.Clientes", "DataPrimeiroContato");
        }
    }
}
