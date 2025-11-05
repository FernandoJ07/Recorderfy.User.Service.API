using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Recorderfy.User.Service.Model.DTOs;

namespace Recorderfy.User.Service.BLL.Interfaces;

public interface IMedicoService
{
    Task<MedicoDto> CreateMedicoAsync(CreateMedicoDto dto);
    Task<MedicoDto?> GetMedicoByIdAsync(Guid id);
    Task<IEnumerable<MedicoDto>> GetAllMedicosAsync();
    Task<MedicoDto?> UpdateMedicoAsync(Guid id, CreateMedicoDto dto);
    Task<bool> DeleteMedicoAsync(Guid id);
}
