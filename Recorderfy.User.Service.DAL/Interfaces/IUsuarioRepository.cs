using Recorderfy.User.Service.Model.Entities;
using System.Threading.Tasks;

namespace Recorderfy.User.Service.DAL.Interfaces
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<Usuario?> GetByEmailWithRelationsAsync(string email);
    }
}
