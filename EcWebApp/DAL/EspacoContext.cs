using EcWebApp.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EcWebApp.DAL
{
    public class EspacoContext : DbContext
    {
        #region Constructor..
        public EspacoContext() : base("EspacoContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            //Database.SetInitializer(new NullDatabaseInitializer<EspacoContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
        #endregion

        #region DbSets..

        public DbSet<DicionarioInfo> Dicionario { get; set; }
        public DbSet<MetaInfo> Metas { get; set; }
        public DbSet<FeriadoInfo> Feriados { get; set; }
        public DbSet<FormaPagamentoInfo> FormasPagamento { get; set; }
        public DbSet<StatusAtendimentoInfo> StatusAtendimento { get; set; }

        public DbSet<UsuarioInfo> Usuarios { get; set; }
        public DbSet<PermissaoInfo> Permissoes { get; set; }

        public DbSet<ClienteInfo> Clientes { get; set; }
        public DbSet<EnderecoInfo> Enderecos { get; set; }

        public DbSet<AtendimentoInfo> Atendimentos { get; set; }
        //public DbSet<AnexoInfo> Anexos { get; set; }

        public DbSet<PedidoInfo> Pedidos { get; set; }
        public DbSet<AmbienteInfo> Ambientes { get; set; }
        //public DbSet<PedidoObservacaoInfo> PedidoObservacoes { get; set; }
        public DbSet<ItemProMobInfo> ItensProMob { get; set; }
        public DbSet<ParcelaInfo> Parcelas { get; set; }

        public DbSet<PlanoContaInfo> Contas { get; set; }
        public DbSet<LancamentoInfo> Lancamentos { get; set; }
        public DbSet<CategoriaLancamentoInfo> Categorias { get; set; }
        public DbSet<FechamentoInfo> Fechamentos { get; set; }

        #endregion
    }
}