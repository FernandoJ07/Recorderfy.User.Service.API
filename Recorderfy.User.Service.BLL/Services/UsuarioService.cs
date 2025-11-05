using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.Entities;
using Recorderfy.User.Service.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorderfy.User.Service.BLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repo;

        public UsuarioService(IGenericRepository<Usuario> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Usuario?> GetByIdAsync(Guid id) => await _repo.GetByIdAsync(id);

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            await _repo.AddAsync(usuario);
            await _repo.SaveAsync();
            return usuario;
        }

        public async Task<Usuario?> UpdateAsync(Usuario usuario)
        {
            var existing = await _repo.GetByIdAsync(usuario.IdUsuario);
            if (existing == null) return null;

            _repo.Update(usuario);
            await _repo.SaveAsync();
            return usuario;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            _repo.Delete(entity);
            await _repo.SaveAsync();
            return true;
        }
    }
}
