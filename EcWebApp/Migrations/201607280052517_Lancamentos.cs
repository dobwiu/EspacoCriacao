namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Lancamentos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "fin.Fechamentos",
                c => new
                    {
                        IdFechamento = c.Int(nullable: false, identity: true),
                        IdConta = c.Guid(nullable: false),
                        DataFechamento = c.DateTime(nullable: false),
                        ValorFechamento = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IdFechamento)
                .ForeignKey("fin.PlanoContas", t => t.IdConta)
                .Index(t => t.IdConta);
            
            AddColumn("fin.PlanoContas", "SaldoAnterior", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("fin.Fechamentos", "IdConta", "fin.PlanoContas");
            DropIndex("fin.Fechamentos", new[] { "IdConta" });
            DropColumn("fin.PlanoContas", "SaldoAnterior");
            DropTable("fin.Fechamentos");
        }
    }
}
