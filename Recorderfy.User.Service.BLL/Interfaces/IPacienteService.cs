using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Recorderfy.User.Service.Model.DTOs;

namespace Recorderfy.User.Service.BLL.Interfaces;

public interface IPacienteService
{
    Task<PacienteDto> CreatePacienteAsync(CreatePacienteDto dto);
    Task<PacienteDto?> GetPacienteByIdAsync(Guid id);
    Task<IEnumerable<PacienteDto>> GetAllPacientesAsync();
    Task<PacienteDto?> UpdatePacienteAsync(Guid id, CreatePacienteDto dto);
    Task<bool> DeletePacienteAsync(Guid id);
    Task<IEnumerable<PacienteDto>> GetPacientesByMedicoIdAsync(Guid medicoId);
    Task<IEnumerable<PacienteDto>> GetPacientesByCuidadorIdAsync(Guid cuidadorId);
}
