using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmallApiAuthentication.Domain.Contracts;
using SmallApiAuthentication.Domain.Manager;
using SmallApiAuthentication.Dto;
using SmallApiAuthentication.Settings;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SmallApiAuthentication.Controllers
{
    /// <summary>
    /// Operações de usuário 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserManager _userManager;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="userManager">Manager Ods</param>
        /// <param name="logger">logger with NLOG</param>
        /// <param name="appSettings"></param>
        public UserController(IUserManager userManager, ILogger<UserController> logger, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Autenticação de usuário no LDAP
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public ActionResult<dynamic> Authenticate([FromBody]UserDto userDto)
        {
            var result = _userManager.Validate(userDto.UserName, Encoding.UTF8.GetString(Convert.FromBase64String(userDto.Password)));

            if (result.HasException)
            {
                _logger.LogError(string.Format("Error: {0}", result.ExceptionMessage));
                return BadRequest(result.ExceptionMessage);
            }

            if (result.Content == null)
                return NotFound(new { message = "User or passaword invalid" });

            var token = TokenManager.GenerateToken(result.Content, _appSettings.Secret);

            return new
            {
                user = result.Content,
                token = token
            };
        }
    }
}