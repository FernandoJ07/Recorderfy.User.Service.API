using Microsoft.AspNetCore.Mvc;
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.DTOs;

namespace Recorderfy.User.Service.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicoController : ControllerBase
{
    private readonly IMedicoService _medicoService;

    public MedicoController(IMedicoService medicoService)
    {
        _medicoService = medicoService;
    }

    /// <summary>
    /// Crear un nuevo médico
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateMedico([FromBody] CreateMedicoDto dto)
    {
        try
        {
            var medico = await _medicoService.CreateMedicoAsync(dto);
            return CreatedAtAction(nameof(GetMedico), new { id = medico.IdUsuario }, medico);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un médico por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedico(Guid id)
    {
        var medico = await _medicoService.GetMedicoByIdAsync(id);
        if (medico == null) return NotFound();
        return Ok(medico);
    }

    /// <summary>
    /// Obtener todos los médicos
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllMedicos()
    {
        var medicos = await _medicoService.GetAllMedicosAsync();
        return Ok(medicos);
    }

    /// <summary>
    /// Actualizar un médico
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMedico(Guid id, [FromBody] CreateMedicoDto dto)
    {
        try
        {
            var medico = await _medicoService.UpdateMedicoAsync(id, dto);
            if (medico == null) return NotFound();
            return Ok(medico);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Eliminar un médico
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedico(Guid id)
    {
        var result = await _medicoService.DeleteMedicoAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
