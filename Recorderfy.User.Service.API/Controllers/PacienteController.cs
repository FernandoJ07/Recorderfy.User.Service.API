using Microsoft.AspNetCore.Mvc;
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.DTOs;

namespace Recorderfy.User.Service.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacienteController : ControllerBase
{
    private readonly IPacienteService _pacienteService;

    public PacienteController(IPacienteService pacienteService)
    {
        _pacienteService = pacienteService;
    }

    /// <summary>
    /// Crear un nuevo paciente
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePaciente([FromBody] CreatePacienteDto dto)
    {
        try
        {
            var paciente = await _pacienteService.CreatePacienteAsync(dto);
            return CreatedAtAction(nameof(GetPaciente), new { id = paciente.IdUsuario }, paciente);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un paciente por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaciente(Guid id)
    {
        var paciente = await _pacienteService.GetPacienteByIdAsync(id);
        if (paciente == null) return NotFound();
        return Ok(paciente);
    }

    /// <summary>
    /// Obtener todos los pacientes
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPacientes()
    {
        var pacientes = await _pacienteService.GetAllPacientesAsync();
        return Ok(pacientes);
    }

    /// <summary>
    /// Actualizar un paciente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePaciente(Guid id, [FromBody] CreatePacienteDto dto)
    {
        try
        {
            var paciente = await _pacienteService.UpdatePacienteAsync(id, dto);
            if (paciente == null) return NotFound();
            return Ok(paciente);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Eliminar un paciente
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(Guid id)
    {
        var result = await _pacienteService.DeletePacienteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
