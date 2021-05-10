using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using DevIO.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Data.Repository
{
    public class FornecedorRepository : Repository<Fornecedor>, IFornecedorRepository
    {
        public FornecedorRepository(MeuDbContext context) : base(context)
        {

        }
        public async Task<Fornecedor> ObterFornecedorEndereco(Guid id)
        {
            return await DbSet.AsNoTracking().Include(e => e.Endereco).FirstOrDefaultAsync(x => x.Id==id);
        }

        public async Task<Fornecedor> ObterFornecedorProdutosEndereco(Guid id)
        {
            return await DbSet.AsNoTracking()
                .Include(p => p.Produtos)
                .Include(e => e.Endereco)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
        /// <summary>
        /// Verificar se existe fornecedor passando o documento
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        public bool ExisteFornecedor(string documento)
        {
            return Buscar(x => x.Documento == documento).Result.Any();
        }
    }
}
