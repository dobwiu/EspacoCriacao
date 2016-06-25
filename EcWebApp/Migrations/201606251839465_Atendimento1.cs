namespace EcWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Atendimento1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("orc.Atendimentos", "Automatico", c => c.Boolean());
            AddColumn("fin.Lancamentos", "Comentario", c => c.String(maxLength: 100));
            AlterColumn("fin.Lancamentos", "Descricao", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("fin.Lancamentos", "Descricao", c => c.String(nullable: false, maxLength: 50));
            DropColumn("fin.Lancamentos", "Comentario");
            DropColumn("orc.Atendimentos", "Automatico");
        }
    }
}
