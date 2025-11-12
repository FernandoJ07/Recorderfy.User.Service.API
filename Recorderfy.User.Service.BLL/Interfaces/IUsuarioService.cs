using Recorderfy.User.Service.Model.Entities;
using Recorderfy.User.Service.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorderfy.User.Service.BLL.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(Guid id);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<Usuario?> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(Guid id);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<UsuarioDto?> GetByDocumentAndRoleAsync(string nroDocumento, int idRol);
    }
}
