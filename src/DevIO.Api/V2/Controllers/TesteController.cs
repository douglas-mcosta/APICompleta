using DevIO.Api.Controllers;
using DevIO.Business.Interfaces;
using DevIO.Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DevIO.Api.V2.Controllers
{
    [ApiVersion("2.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TesteController : MainController
    {
        private readonly ILogger _logger;
        public TesteController(INotificador notificador, IUser appUser, ILogger<TesteController> logger) : base(notificador, appUser)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Versao()
        {
            _logger.LogCritical("Teste erro Critico");
            _logger.LogDebug("Teste erro Debug");
            _logger.LogWarning("Teste Warning");
            _logger.LogInformation("Teste log info");
            _logger.LogError("Teste error");
            return "Sou a V2";
        }
    }
}