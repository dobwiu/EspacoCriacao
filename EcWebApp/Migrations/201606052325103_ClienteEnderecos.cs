namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClienteEnderecos : DbMigration
    {
        public override void Up()
        {
            AddColumn("orc.Clientes", "EnderecosIguais", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("orc.Clientes", "EnderecosIguais");
        }
    }
}
