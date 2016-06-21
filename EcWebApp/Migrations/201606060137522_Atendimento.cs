namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Atendimento : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "orc.Atendimentos",
                c => new
                    {
                        IdAtendimento = c.Guid(nullable: false),
                        IdCliente = c.Guid(nullable: false),
                        Interesse = c.Int(nullable: false),
                        IdStatus = c.Int(nullable: false),
                        StatusPlanta = c.Int(),
                        DataApresentacao = c.DateTime(),
                        DataMedicao = c.DateTime(),
                        Comentario = c.String(maxLength: 500),
                        IdVendedor = c.Guid(nullable: false),
                        DataAtendimento = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IdAtendimento)
                .ForeignKey("orc.Clientes", t => t.IdCliente)
                .ForeignKey("dbo.StatusAtendimento", t => t.IdStatus)
                .ForeignKey("dbo.Usuarios", t => t.IdVendedor)
                .Index(t => t.IdCliente)
                .Index(t => t.IdStatus)
                .Index(t => t.IdVendedor);
            
        }
        
        public override void Down()
        {
            DropForeignKey("orc.Atendimentos", "IdVendedor", "dbo.Usuarios");
            DropForeignKey("orc.Atendimentos", "IdStatus", "dbo.StatusAtendimento");
            DropForeignKey("orc.Atendimentos", "IdCliente", "orc.Clientes");
            DropIndex("orc.Atendimentos", new[] { "IdVendedor" });
            DropIndex("orc.Atendimentos", new[] { "IdStatus" });
            DropIndex("orc.Atendimentos", new[] { "IdCliente" });
            DropTable("orc.Atendimentos");
        }
    }
}
