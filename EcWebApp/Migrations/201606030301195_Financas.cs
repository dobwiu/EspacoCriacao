namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Financas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategoriasLancamento",
                c => new
                    {
                        IdCategoria = c.Int(nullable: false, identity: true),
                        Descricao = c.String(nullable: false, maxLength: 50),
                        TipoLancamento = c.Int(nullable: false),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdCategoria);
            
            AddColumn("fin.Lancamentos", "IdCategoria", c => c.Int(nullable: false));
            AlterColumn("fin.Lancamentos", "TipoLancamento", c => c.Int(nullable: false));
            CreateIndex("fin.Lancamentos", "IdCategoria");
            AddForeignKey("fin.Lancamentos", "IdCategoria", "dbo.CategoriasLancamento", "IdCategoria");
        }
        
        public override void Down()
        {
            DropForeignKey("fin.Lancamentos", "IdCategoria", "dbo.CategoriasLancamento");
            DropIndex("fin.Lancamentos", new[] { "IdCategoria" });
            AlterColumn("fin.Lancamentos", "TipoLancamento", c => c.String(nullable: false, maxLength: 1));
            DropColumn("fin.Lancamentos", "IdCategoria");
            DropTable("dbo.CategoriasLancamento");
        }
    }
}
