using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.DAL.Data;
using Recorderfy.User.Service.Model.DTOs;
using Recorderfy.User.Service.Model.Entities;

namespace Recorderfy.User.Service.BLL.Services
{
    public class CuidadorService : ICuidadorService
    {
        private readonly ApplicationDbContext _context;
        private const int ROL_CUIDADOR = 4; // Asume que ID 4 es el rol Cuidador

        public CuidadorService(ApplicationDbContext context)
        {
            _context = context;
        }

            public async Task<CuidadorDto> CreateCuidadorAsync(CreateCuidadorDto dto)
        {
            try
            {
                var exists = await _context.Usuarios
                    .AnyAsync(u => u.Email == dto.Email || u.NroDocumento == dto.NroDocumento);

                if (exists)
                    throw new InvalidOperationException("Ya existe un usuario con ese email o documento.");

                var cuidador = new Cuidador
                {
                    IdUsuario = Guid.NewGuid(),
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    IdTipoDocumento = dto.IdTipoDocumento,
                    NroDocumento = dto.NroDocumento,
                    Email = dto.Email,
                    Telefono = dto.Telefono,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    IdRol = dto.IdRol,
                    Genero = dto.Genero,
                    FechaNacimiento = dto.FechaNacimiento,
                    FechaRegistro = DateTime.Now,
                    Estado = "activo",
                    FotoPerfil = dto.FotoPerfil,
                    // Campos específicos de Cuidador
                    RelacionConPaciente = dto.RelacionConPaciente,
                    Direccion = dto.Direccion,
                    NotificacionesActivadas = dto.NotificacionesActivadas,
                    PacientesAsociados = "[]"
                };

                _context.Cuidadores.Add(cuidador);
                await _context.SaveChangesAsync();

                var savedCuidador = await GetCuidadorByIdAsync(cuidador.IdUsuario);

                if (savedCuidador == null)
                    throw new Exception("No se pudo recuperar el médico después de guardarlo.");

                return savedCuidador;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[ERROR VALIDACIÓN] {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"[ERROR BD] No se pudo guardar el cuidador: {ex.InnerException?.Message ?? ex.Message}");
                throw new Exception("Error al guardar el cuidador en la base de datos.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR GENERAL] {ex.Message}");
                throw new Exception("Ocurrió un error inesperado al crear el cuidador.", ex);
            }
        }

        public async Task<CuidadorDto?> GetCuidadorByIdAsync(Guid id)
        {
            try
            {
                var cuidador = await _context.Cuidadores
                    .Include(c => c.IdRolNavigation)
                    .Include(c => c.IdTipoDocumentoNavigation)
                    .FirstOrDefaultAsync(c => c.IdUsuario == id);

                return cuidador == null ? null : MapToDto(cuidador);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[NO ENCONTRADO] {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR GENERAL] {ex.Message}");
                throw new Exception("Error al obtener el cuidador por ID.", ex);
            }
        }

        public async Task<IEnumerable<CuidadorDto>> GetAllCuidadoresAsync()
        {
            try
            {
                var cuidadores = await _context.Cuidadores
                    .Include(c => c.IdRolNavigation)
                    .Include(c => c.IdTipoDocumentoNavigation)
                    .ToListAsync();

                return cuidadores.Select(MapToDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR GENERAL] {ex.Message}");
                throw new Exception("Error al obtener la lista de cuidadores.", ex);
            }
        }

        public async Task<CuidadorDto?> UpdateCuidadorAsync(Guid id, CreateCuidadorDto dto)
        {
            try
            {
                var cuidador = await _context.Cuidadores.FindAsync(id);
                if (cuidador == null)
                    throw new KeyNotFoundException($"No se encontró el cuidador con ID {id}.");

                // Actualizar campos de Usuario
                cuidador.Nombre = dto.Nombre;
                cuidador.Apellido = dto.Apellido;
                cuidador.Email = dto.Email;
                cuidador.Telefono = dto.Telefono;
                cuidador.Genero = dto.Genero;
                cuidador.FechaNacimiento = dto.FechaNacimiento;
                cuidador.FotoPerfil = dto.FotoPerfil;

                // Actualizar campos de Cuidador
                cuidador.RelacionConPaciente = dto.RelacionConPaciente;
                cuidador.Direccion = dto.Direccion;
                cuidador.NotificacionesActivadas = dto.NotificacionesActivadas;

                await _context.SaveChangesAsync();
                return MapToDto(cuidador);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[NO ENCONTRADO] {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"[ERROR BD] No se pudo actualizar el cuidador: {ex.InnerException?.Message ?? ex.Message}");
                throw new Exception("Error al actualizar el cuidador en la base de datos.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR GENERAL] {ex.Message}");
                throw new Exception("Ocurrió un error inesperado al actualizar el cuidador.", ex);
            }
        }

        public async Task<bool> DeleteCuidadorAsync(Guid id)
        {
            try
            {
                var cuidador = await _context.Cuidadores.FindAsync(id);
                if (cuidador == null)
                    throw new KeyNotFoundException($"No se encontró el cuidador con ID {id}.");

                _context.Cuidadores.Remove(cuidador);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[NO ENCONTRADO] {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"[ERROR BD] No se pudo eliminar el cuidador: {ex.InnerException?.Message ?? ex.Message}");
                throw new Exception("Error al eliminar el cuidador de la base de datos.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR GENERAL] {ex.Message}");
                throw new Exception("Ocurrió un error inesperado al eliminar el cuidador.", ex);
            }
        }

        private static CuidadorDto MapToDto(Cuidador cuidador)
        {
            return new CuidadorDto
            {
                IdUsuario = cuidador.IdUsuario,
                Nombre = cuidador.Nombre,
                Apellido = cuidador.Apellido,
                NroDocumento = cuidador.NroDocumento,
                Email = cuidador.Email,
                Telefono = cuidador.Telefono,
                Genero = cuidador.Genero,
                FechaNacimiento = cuidador.FechaNacimiento,
                FechaRegistro = cuidador.FechaRegistro,
                UltimoAcceso = cuidador.UltimoAcceso,
                Estado = cuidador.Estado,
                FotoPerfil = cuidador.FotoPerfil,
                TipoDocumento = cuidador.IdTipoDocumentoNavigation == null ? null : new TipoDocumentoDto
                {
                    IdTipoDocumento = cuidador.IdTipoDocumentoNavigation.IdTipoDocumento,
                    NombreDocumento = cuidador.IdTipoDocumentoNavigation.NombreDocumento,
                    Descripcion = cuidador.IdTipoDocumentoNavigation.Descripcion
                },
                Rol = cuidador.IdRolNavigation == null ? null : new RolDto
                {
                    IdRol = cuidador.IdRolNavigation.IdRol,
                    NombreRol = cuidador.IdRolNavigation.NombreRol
                },
                RelacionConPaciente = cuidador.RelacionConPaciente,
                Direccion = cuidador.Direccion,
                NotificacionesActivadas = cuidador.NotificacionesActivadas
            };
        }
    }
}
