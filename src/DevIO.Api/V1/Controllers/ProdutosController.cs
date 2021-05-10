using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using DevIO.Api.Controllers;
using DevIO.Api.DTO;
using DevIO.Api.Extensions;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using DevIO.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;
        public ProdutosController(INotificador notificador,
                                     IProdutoRepository produtoRepository,
                                     IProdutoService produtoService,
                                     IMapper mapper,
                                     IUser appUser) : base(notificador,appUser)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpGet]
        [ClaimsAuthorize("Produto", "r")]
        public async Task<IEnumerable<ProdutoDTO>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<ProdutoDTO>>(await _produtoRepository.ObterTodos());
        }
        [ClaimsAuthorize("Produto", "r")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoDTO>> ObterPorId(Guid id)
        {
            var produto = await ObterProduto(id);
            if (produto == null) return NotFound();
            return produto;
        }
        [HttpPost]
        [ClaimsAuthorize("Produto", "c")]
        public async Task<ActionResult> Adcionar(ProdutoDTO produtoDTO)
        {

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imagemNome = Guid.NewGuid() + "_" + produtoDTO.Imagem;

            if (!UploadImage(produtoDTO.ImagemUpload, imagemNome))
            {
                return CustomResponse(produtoDTO);
            }

            produtoDTO.Imagem = imagemNome;
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoDTO));
            return CustomResponse(produtoDTO);
        }

        [HttpPut("{id:guid}")]
        [ClaimsAuthorize("Produto","u")]
        public async Task<ActionResult<ProdutoDTO>> Atualizar(Guid id, ProdutoDTO produtoDTO)
        {

            if (id != produtoDTO.Id)
            {
                NotificarErro("O id informado não confere com o id do objeto passado para atualização.");
                CustomResponse();
            }
            var produtoAtualizacao = await ObterProduto(id);
            if (produtoAtualizacao == null) return NotFound();

            produtoDTO.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (produtoDTO.ImagemUpload != null)
            {
                var imgNome = Guid.NewGuid() + "_" + produtoDTO.Imagem;
                if (!UploadImage(produtoDTO.ImagemUpload, imgNome))
                {
                    return CustomResponse(ModelState);
                }
                produtoAtualizacao.Imagem = imgNome;
            }

            produtoAtualizacao.Nome = produtoDTO.Nome;
            produtoAtualizacao.Descricao = produtoDTO.Descricao;
            produtoAtualizacao.Valor = produtoDTO.Valor;
            produtoAtualizacao.Ativo = produtoDTO.Ativo;
            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));
            return CustomResponse(produtoDTO);
        }
        [HttpDelete("{id:guid}")]
        [ClaimsAuthorize("Produto","d")]
        public async Task<ActionResult<ProdutoDTO>> Excluir(Guid id)
        {
            var produto = ObterProduto(id);
            if (produto == null) return NotFound();
            await _produtoService.Remover(id);
            return CustomResponse(produto);
        }
       
        private bool UploadImage(string arquivo, string imagemNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para o produto.");
                return false;
            }
            var imageDataByteArray = Convert.FromBase64String(arquivo);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens/", imagemNome);
            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com esse nome.");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);
            return true;

        }
        private async Task<ProdutoDTO> ObterProduto(Guid id)
        {
            return _mapper.Map<ProdutoDTO>(await _produtoRepository.ObterPorId(id));
        }

    }
}