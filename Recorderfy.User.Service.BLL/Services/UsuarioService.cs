using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.Entities;
using Recorderfy.User.Service.Model.DTOs;
using Recorderfy.User.Service.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Recorderfy.User.Service.BLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepo;

        public UsuarioService(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync() => await _usuarioRepo.GetAllAsync();

        public async Task<Usuario?> GetByIdAsync(Guid id) => await _usuarioRepo.GetByIdAsync(id);

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            await _usuarioRepo.AddAsync(usuario);
            await _usuarioRepo.SaveAsync();
            return usuario;
        }

        public async Task<Usuario?> UpdateAsync(Usuario usuario)
        {
            var existing = await _usuarioRepo.GetByIdAsync(usuario.IdUsuario);
            if (existing == null) return null;

            _usuarioRepo.Update(usuario);
            await _usuarioRepo.SaveAsync();
            return usuario;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _usuarioRepo.GetByIdAsync(id);
            if (entity == null) return false;

            _usuarioRepo.Delete(entity);
            await _usuarioRepo.SaveAsync();
            return true;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Buscar usuario por email con relaciones
                var usuario = await _usuarioRepo.GetByEmailWithRelationsAsync(loginDto.Email);

                if (usuario == null)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Email o contraseña incorrectos",
                        Usuario = null
                    };
                }

                if (!VerifyPassword(loginDto.Password, usuario.PasswordHash))
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Email o contraseña incorrectos",
                        Usuario = null
                    };
                }

                // Actualizar último acceso
                //usuario.UltimoAcceso = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
                //_usuarioRepo.Update(usuario);
                //await _usuarioRepo.SaveAsync();

                // Mapear usuario a DTO
                var usuarioDto = MapToDto(usuario);

                return new LoginResponseDto
                {
                    Success = true,
                    Message = "Login exitoso",
                    Usuario = usuarioDto
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = $"Error al iniciar sesión: {ex.Message}",
                    Usuario = null
                };
            }
        }

        private static UsuarioDto MapToDto(Usuario usuario)
        {
            return new UsuarioDto
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                NroDocumento = usuario.NroDocumento,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                Genero = usuario.Genero,
                FechaNacimiento = usuario.FechaNacimiento,
                FechaRegistro = usuario.FechaRegistro,
                UltimoAcceso = usuario.UltimoAcceso,
                Estado = usuario.Estado,
                FotoPerfil = usuario.FotoPerfil,
                TipoDocumento = usuario.IdTipoDocumentoNavigation == null ? null : new TipoDocumentoDto
                {
                    IdTipoDocumento = usuario.IdTipoDocumentoNavigation.IdTipoDocumento,
                    NombreDocumento = usuario.IdTipoDocumentoNavigation.NombreDocumento,
                    Descripcion = usuario.IdTipoDocumentoNavigation.Descripcion
                },
                Rol = usuario.IdRolNavigation == null ? null : new RolDto
                {
                    IdRol = usuario.IdRolNavigation.IdRol,
                    NombreRol = usuario.IdRolNavigation.NombreRol
                }
            };
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // Verificar contraseña usando BCrypt
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
