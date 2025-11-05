using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Recorderfy.User.Service.Model.DTOs;

namespace Recorderfy.User.Service.BLL.Interfaces;

public interface ICuidadorService
{
    Task<CuidadorDto> CreateCuidadorAsync(CreateCuidadorDto dto);
    Task<CuidadorDto?> GetCuidadorByIdAsync(Guid id);
    Task<IEnumerable<CuidadorDto>> GetAllCuidadoresAsync();
    Task<CuidadorDto?> UpdateCuidadorAsync(Guid id, CreateCuidadorDto dto);
    Task<bool> DeleteCuidadorAsync(Guid id);
}
