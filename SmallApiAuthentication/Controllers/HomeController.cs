using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmallApiAuthentication.Controllers
{
    /// <summary>
    /// Teste de autorização e autenticação
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Método anonimo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("anonymous")]
        [AllowAnonymous]
        public string Anonymous() => "Anônimo";

        /// <summary>
        /// Método autorizado apenas para usuários com as roles de user e manager
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("user")]
        [Authorize(Roles = "user,manager")]
        public string User() => "Usuário";

        /// <summary>
        /// Método autorizado apenas para usuários com a role de manager
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("manager")]
        [Authorize(Roles = "manager")]
        public string Manager() => "Gerente";
    }
}