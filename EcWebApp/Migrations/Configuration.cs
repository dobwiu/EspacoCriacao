namespace EcWebApp.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EcWebApp.DAL.EspacoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EcWebApp.DAL.EspacoContext context)
        {
            //  This method will be called after migrating to the latest version.

            //-- Usuario ADMIN [Dj102030!] -->
            context.Usuarios.AddOrUpdate(
              p => p.LoginUsuario,
              new UsuarioInfo
              {
                  IdUsuario = Guid.Empty,
                  NomeUsuario = "Administrador",
                  LoginUsuario = "dobsprt",
                  SenhaUsuario = "1D27D056812F88F7EF77BD1E8209B6EC",
                  SenhaConfirmada = "1D27D056812F88F7EF77BD1E8209B6EC",
                  Ativo = true
              }
            );
            context.SaveChanges();

            //-- Permissao do usuario ADMIN -->
            context.Permissoes.AddOrUpdate(
              p => p.IdUsuario,
              new PermissaoInfo
              {
                  IdUsuario = Guid.Empty,
                  Vendas = EnumNivelAcesso.SomenteConsulta,
                  Financeiro = EnumNivelAcesso.SemAcesso,
                  Relatorios = EnumNivelAcesso.SomenteConsulta,
                  Administrativo = EnumNivelAcesso.Administrador
              }
            );
            context.SaveChanges();
        }
    }
}
