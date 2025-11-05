using Microsoft.AspNetCore.Mvc;
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.DTOs;

namespace Recorderfy.User.Service.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuidadorController : ControllerBase
{
    private readonly ICuidadorService _cuidadorService;

    public CuidadorController(ICuidadorService cuidadorService)
    {
        _cuidadorService = cuidadorService;
    }

    /// <summary>
    /// Crear un nuevo cuidador
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCuidador([FromBody] CreateCuidadorDto dto)
    {
        try
        {
            var cuidador = await _cuidadorService.CreateCuidadorAsync(dto);
            return CreatedAtAction(nameof(GetCuidador), new { id = cuidador.IdUsuario }, cuidador);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un cuidador por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCuidador(Guid id)
    {
        var cuidador = await _cuidadorService.GetCuidadorByIdAsync(id);
        if (cuidador == null) return NotFound();
        return Ok(cuidador);
    }

    /// <summary>
    /// Obtener todos los cuidadores
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllCuidadores()
    {
        var cuidadores = await _cuidadorService.GetAllCuidadoresAsync();
        return Ok(cuidadores);
    }

    /// <summary>
    /// Actualizar un cuidador
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCuidador(Guid id, [FromBody] CreateCuidadorDto dto)
    {
        try
        {
            var cuidador = await _cuidadorService.UpdateCuidadorAsync(id, dto);
            if (cuidador == null) return NotFound();
            return Ok(cuidador);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Eliminar un cuidador
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCuidador(Guid id)
    {
        var result = await _cuidadorService.DeleteCuidadorAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
