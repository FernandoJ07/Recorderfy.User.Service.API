//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Recorderfy.User.Service.BLL.Interfaces;
//using Recorderfy.User.Service.Model.Entities;

//namespace Recorderfy.User.Service.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UsuarioController : ControllerBase
//    {
//        private readonly IUsuarioService _usuarioService;

//        public UsuarioController(IUsuarioService usuarioService)
//        {
//            _usuarioService = usuarioService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll() =>
//            Ok(await _usuarioService.GetAllAsync());

//        [HttpGet("{id:guid}")]
//        public async Task<IActionResult> GetById(Guid id)
//        {
//            var user = await _usuarioService.GetByIdAsync(id);
//            return user is null ? NotFound() : Ok(user);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] Usuario usuario)
//        {
//            var created = await _usuarioService.CreateAsync(usuario);
//            return CreatedAtAction(nameof(GetById), new { id = created.IdUsuario }, created);
//        }

//        [HttpPut("{id:guid}")]
//        public async Task<IActionResult> Update(Guid id, [FromBody] Usuario usuario)
//        {
//            if (id != usuario.IdUsuario) return BadRequest();

//            var updated = await _usuarioService.UpdateAsync(usuario);
//            return updated is null ? NotFound() : Ok(updated);
//        }

//        [HttpDelete("{id:guid}")]
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            var deleted = await _usuarioService.DeleteAsync(id);
//            return deleted ? NoContent() : NotFound();
//        }
//    }
//}
