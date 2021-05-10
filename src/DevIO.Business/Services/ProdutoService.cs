using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using DevIO.Business.Models.Validations;
using System;
using System.Threading.Tasks;

namespace DevIO.Business.Services
{
    public class ProdutoService : BaseService, IProdutoService, IDisposable
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly INotificador _notificador;
        private readonly IUser _user;

        public ProdutoService(IProdutoRepository produtoRepository, INotificador notificador, IUser user) : base(notificador)
        {
            _produtoRepository = produtoRepository;
            _notificador = notificador;
            _user = user;
        }

        public async Task Adicionar(Produto produto)
        {
            if (!ExecutarValidacao(new ProdutoValidation(), produto)) return;

                var userId = _user.GetUserId();

            await _produtoRepository.Adicionar(produto);
        }

        public async Task Atualizar(Produto produto)
        {
            if (!ExecutarValidacao(new ProdutoValidation(), produto)) return;
            await _produtoRepository.Atualizar(produto);
        }

        public async Task Remover(Guid id)
        {
            await _produtoRepository.Remover(id);
        }
        public void Dispose()
        {
           _produtoRepository?.Dispose();
        }
    }
}
