using EcWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcWebApp.DAL
{
    public class UnitOfWork : IDisposable
    {
        #region Constructor..
        private EspacoContext context = new EspacoContext();
        private bool disposed = false;

        public void Save()
        {
            context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region REPOSITÓRIOS..
        private GenericRepository<AtendimentoInfo> atendimentoRepository;
        public GenericRepository<AtendimentoInfo> AtendimentoRepository
        {
            get
            {
                return this.atendimentoRepository ?? (this.atendimentoRepository = new GenericRepository<AtendimentoInfo>(context));
            }
        }

        #region Dicionario..

        private GenericRepository<MetaInfo> metaRepository;
        public GenericRepository<MetaInfo> MetaRepository
        {
            get
            {
                return this.metaRepository ?? (this.metaRepository = new GenericRepository<MetaInfo>(context));
            }
        }

        private GenericRepository<FeriadoInfo> feriadoRepository;
        public GenericRepository<FeriadoInfo> FeriadoRepository
        {
            get
            {
                return this.feriadoRepository ?? (this.feriadoRepository = new GenericRepository<FeriadoInfo>(context));
            }
        }

        private GenericRepository<FormaPagamentoInfo> formaPagamentoRepository;
        public GenericRepository<FormaPagamentoInfo> FormaPagamentoRepository
        {
            get
            {
                return this.formaPagamentoRepository ?? (this.formaPagamentoRepository = new GenericRepository<FormaPagamentoInfo>(context));
            }
        }

        private GenericRepository<StatusAtendimentoInfo> statusAtendimentoRepository;
        public GenericRepository<StatusAtendimentoInfo> StatusAtendimentoRepository
        {
            get
            {
                return this.statusAtendimentoRepository ?? (this.statusAtendimentoRepository = new GenericRepository<StatusAtendimentoInfo>(context));
            }
        }
        #endregion

        #region Usuario..

        private GenericRepository<UsuarioInfo> usuarioRepository;
        public GenericRepository<UsuarioInfo> UsuarioRepository
        {
            get
            {
                return this.usuarioRepository ?? (this.usuarioRepository = new GenericRepository<UsuarioInfo>(context));
            }
        }

        private GenericRepository<PermissaoInfo> permissaoRepository;
        public GenericRepository<PermissaoInfo> PermissaoRepository
        {
            get
            {
                return this.permissaoRepository ?? (this.permissaoRepository = new GenericRepository<PermissaoInfo>(context));
            }
        }
        #endregion

        #region Cliente

        private GenericRepository<ClienteInfo> clienteRepository;
        public GenericRepository<ClienteInfo> ClienteRepository
        {
            get
            {
                return this.clienteRepository ?? (this.clienteRepository = new GenericRepository<ClienteInfo>(context));
            }
        }

        private GenericRepository<EnderecoInfo> enderecoRepository;
        public GenericRepository<EnderecoInfo> EnderecoRepository
        {
            get
            {
                return this.enderecoRepository ?? (this.enderecoRepository = new GenericRepository<EnderecoInfo>(context));
            }
        }

        private GenericRepository<AnexoInfo> anexoRepository;
        public GenericRepository<AnexoInfo> AnexoRepository
        {
            get
            {
                return this.anexoRepository ?? (this.anexoRepository = new GenericRepository<AnexoInfo>(context));
            }
        }

        #endregion

        #region Pedidos..

        private GenericRepository<PedidoInfo> pedidoRepository;
        public GenericRepository<PedidoInfo> PedidoRepository
        {
            get
            {
                return this.pedidoRepository ?? (this.pedidoRepository = new GenericRepository<PedidoInfo>(context));
            }
        }

        private GenericRepository<ItemPedidoInfo> itemPedidoRepository;
        public GenericRepository<ItemPedidoInfo> ItemPedidoRepository
        {
            get
            {
                return this.itemPedidoRepository ?? (this.itemPedidoRepository = new GenericRepository<ItemPedidoInfo>(context));
            }
        }

        #endregion

        #region Financeiro

        private GenericRepository<PlanoContaInfo> planoContaRepository;
        public GenericRepository<PlanoContaInfo> PlanoContaRepository
        {
            get
            {
                return this.planoContaRepository ?? (this.planoContaRepository = new GenericRepository<PlanoContaInfo>(context));
            }
        }

        private GenericRepository<LancamentoInfo> lancamentoRepository;
        public GenericRepository<LancamentoInfo> LancamentoRepository
        {
            get
            {
                return this.lancamentoRepository ?? (this.lancamentoRepository = new GenericRepository<LancamentoInfo>(context));
            }
        }

        #endregion

        #endregion 
    }
}