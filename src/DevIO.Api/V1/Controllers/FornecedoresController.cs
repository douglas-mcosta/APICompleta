using AutoMapper;
using DevIO.Api.Controllers;
using DevIO.Api.DTO;
using DevIO.Api.Extensions;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using DevIO.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FornecedoresController : MainController
    {
        //Repositorios
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        //Services
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;
        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                        IMapper mapper,
                                        IFornecedorService fornecedorService,
                                        INotificador notificador,
                                        IEnderecoRepository enderecoRepository,
                                        IUser appUser) : base(notificador,appUser)
        {
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
            _fornecedorService = fornecedorService;
            _enderecoRepository = enderecoRepository;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<FornecedorDTO>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<FornecedorDTO>>(await _fornecedorRepository.ObterTodos());
        }
        [HttpGet("{id:guid}")]
        [ClaimsAuthorize("Fornecedor","r")]
        public async Task<ActionResult<FornecedorDTO>> ObterPorId(Guid id)
        {
            var fornecedor = await ObterFornecedorProdutosEndereco(id);
            if (fornecedor == null) return NotFound();
            return Ok(fornecedor);
        }

        [HttpPost]
        [ClaimsAuthorize("Fornecedor","c")]
        public async Task<ActionResult<FornecedorDTO>> Adicionar(FornecedorDTO fornecedorDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorDTO));
            return CustomResponse(fornecedorDTO);
        }
        [HttpPut("{id:guid}")]
        [ClaimsAuthorize("Fornecedor","u")]
        public async Task<ActionResult<FornecedorDTO>> Atualizar(Guid id, FornecedorDTO fornecedor)
        {
            if (id != fornecedor.Id)
            {
                NotificarErro("O Id do fornecedor não é o mesmo do objeto para atualização");
                return CustomResponse(fornecedor);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);
            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedor));

            return CustomResponse(fornecedor);
        }
        [HttpDelete("{id:guid}")]
        [ClaimsAuthorize("Fornecedor","d")]
        public async Task<ActionResult<FornecedorDTO>> Excluir(Guid id)
        {
            var fornecedorDTO = await ObterFornecedorEndereco(id);
            if (fornecedorDTO == null) return NotFound();
            await _fornecedorService.Remover(id);
            return CustomResponse(fornecedorDTO);
        }

        [HttpPut("atualizar-endereco/{id:guid}")]
        [ClaimsAuthorize("Fornecedor","u")]
        public async Task<ActionResult<EnderecoDTO>> AtualizarEndereco(Guid id, EnderecoDTO enderecoDTO)
        {
            if (id != enderecoDTO.Id)
            {
                NotificarErro("O Id informado não confere com o Id do objeto que sera atualizado");
                return CustomResponse(enderecoDTO);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);
            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(enderecoDTO));

            return CustomResponse(enderecoDTO);
        }

        [HttpGet("obter-endereco/{id:guid}")]
        [ClaimsAuthorize("Fornecedor","r")]
        public async Task<EnderecoDTO> ObterEnderecoPorId(Guid id)
        {
            return _mapper.Map<EnderecoDTO>(await _enderecoRepository.ObterEnderecoPorFornecedor(id));
        }
        //Auxiliares
        private async Task<FornecedorDTO> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorDTO>(await _enderecoRepository.ObterEnderecoPorFornecedor(id));
        }
        private async Task<FornecedorDTO> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorDTO>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }
    }
}
