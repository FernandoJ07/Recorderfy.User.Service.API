using Microsoft.EntityFrameworkCore;
using Recorderfy.User.Service.DAL.Data;
using Recorderfy.User.Service.DAL.Interfaces;
using Recorderfy.User.Service.Model.Entities;
using System.Threading.Tasks;

namespace Recorderfy.User.Service.DAL.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetByEmailWithRelationsAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Include(u => u.IdTipoDocumentoNavigation)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
