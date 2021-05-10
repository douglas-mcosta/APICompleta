using System.Linq;
using DevIO.Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DevIO.Business.Notificacoes;
using DevIO.Business.Interfaces;
using System;

namespace DevIO.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificador _notificador;
        protected readonly IUser AppUser;
        protected Guid UsuarioId {get;}
        protected bool UsuarioAutenticado {get;}
        public MainController(INotificador notificador, IUser appUser)
        {
            _notificador = notificador;
            AppUser = appUser;

            if (appUser.IsAuthenticated())
            {
                UsuarioId = appUser.GetUserId();
                UsuarioAutenticado = true;
            }
        }
        //Retorno Personalizado 
        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(new
                {
                    success = true,
                    data = result,
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    erros = _notificador.ObterNotificacoes().Select(m => m.Mensagem)
                });
            }
        }

        //Validar ModelState
        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!ModelState.IsValid) NotificarErroModelInvalida(modelState);
            return CustomResponse();
        }

        //Notifica os erros da ModelState
        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {

            var erros = modelState.Values.SelectMany(erros => erros.Errors);

            foreach (var erro in erros)
            {
                var erroMensagem = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(erroMensagem);
            }
        }

        //Notificador
        protected void NotificarErro(string erroMensagem)
        {
            _notificador.Handle(new Notificacao(erroMensagem));
        }

        //Verificar se tem alguma notificação de alguma outra camada
        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

    }
}
