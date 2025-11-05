using Microsoft.AspNetCore.Mvc;
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.DTOs;

namespace Recorderfy.User.Service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Endpoint para iniciar sesión con email y contraseña
        /// </summary>
        /// <param name="loginDto">Credenciales del usuario (Email y Password)</param>
        /// <returns>Objeto completo del usuario si el login es exitoso</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponseDto
                {
                    Success = false,
                    Message = "Datos de entrada inválidos",
                    Usuario = null
                });
            }

            var result = await _usuarioService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}
