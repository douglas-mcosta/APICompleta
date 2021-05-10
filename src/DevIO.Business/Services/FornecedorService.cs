using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using DevIO.Business.Models.Validations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Business.Services
{
    public class FornecedorService : BaseService, IFornecedorService, IDisposable
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly INotificador _notificador;

        public FornecedorService(
                                IFornecedorRepository fornecedorRepository,
                                IEnderecoRepository enderecoRepository,
                                INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _enderecoRepository = enderecoRepository;
            _notificador = notificador;
        }

        public async Task Adicionar(Fornecedor fornecedor)
        {
            if (!ExecutarValidacao(new FornecedorValidation(), fornecedor) || !ExecutarValidacao(new EnderecoValidation(), fornecedor.Endereco)) return;

            if (_fornecedorRepository.ExisteFornecedor(fornecedor.Documento))
            {
                Notificar("Já existe um fornecedor com o documento: " + fornecedor.Documento + " cadastrado.");
                return;
            }

            await _fornecedorRepository.Adicionar(fornecedor);
            return;
        }

        public async Task Atualizar(Fornecedor fornecedor)
        {
            if (!ExecutarValidacao(new FornecedorValidation(), fornecedor)) return;
            if (_fornecedorRepository.Buscar(f => f.Documento == fornecedor.Documento && f.Id != fornecedor.Id).Result.Any())
            {
                Notificar("Já existe um fornecedor com o documento: " + fornecedor.Documento + " cadastrado.");
                return;
            }
            await _fornecedorRepository.Atualizar(fornecedor);
            return;
        }

        public async Task AtualizarEndereco(Endereco endereco)
        {
            if (!ExecutarValidacao(new EnderecoValidation(), endereco)) return;

            await _enderecoRepository.Atualizar(endereco);
        }

        public async Task Remover(Guid id)
        {
            if (_fornecedorRepository.ObterFornecedorProdutosEndereco(id).Result.Produtos.Any())
            {
                Notificar("Não é possivel remover fornecedores com produtos cadastrados");
                return;
            }
            var endereco = await _enderecoRepository.ObterEnderecoPorFornecedor(id);
            if (endereco != null)
            {
               await _enderecoRepository.Remover(endereco.Id);
            }
            await _fornecedorRepository.Remover(id);
            return;
        }
        public void Dispose()
        {
            _enderecoRepository?.Dispose();
            _fornecedorRepository?.Dispose();
        }
    }
}
