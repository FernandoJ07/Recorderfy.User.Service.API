using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.DAL.Data;
using Recorderfy.User.Service.Model.DTOs;
using Recorderfy.User.Service.Model.Entities;

namespace Recorderfy.User.Service.BLL.Services;

public class MedicoService : IMedicoService
{
    private readonly ApplicationDbContext _context;
    private const int ROL_MEDICO = 2; // Asume que ID 2 es el rol Médico

    public MedicoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MedicoDto> CreateMedicoAsync(CreateMedicoDto dto)
    {
        // Validar que no exista el email o documento
        var exists = await _context.Usuarios
            .AnyAsync(u => u.Email == dto.Email || u.NroDocumento == dto.NroDocumento);
        
        if (exists)
            throw new InvalidOperationException("Ya existe un usuario con ese email o documento");

        var medico = new Medico
        {
            IdUsuario = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            IdTipoDocumento = dto.IdTipoDocumento,
            NroDocumento = dto.NroDocumento,
            Email = dto.Email,
            Telefono = dto.Telefono,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IdRol = ROL_MEDICO,
            Genero = dto.Genero,
            FechaNacimiento = dto.FechaNacimiento,
            FechaRegistro = DateTime.Now,
            Estado = "activo",
            FotoPerfil = dto.FotoPerfil,
            // Campos específicos de Médico
            Especialidad = dto.Especialidad,
            CentroMedico = dto.CentroMedico,
            NotificacionesActivadas = dto.NotificacionesActivadas,
            PacientesAsignados = "[]"
        };

        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        return MapToDto(medico);
    }

    public async Task<MedicoDto?> GetMedicoByIdAsync(Guid id)
    {
        var medico = await _context.Medicos
            .Include(m => m.IdRolNavigation)
            .Include(m => m.IdTipoDocumentoNavigation)
            .FirstOrDefaultAsync(m => m.IdUsuario == id);

        return medico == null ? null : MapToDto(medico);
    }

    public async Task<IEnumerable<MedicoDto>> GetAllMedicosAsync()
    {
        var medicos = await _context.Medicos
            .Include(m => m.IdRolNavigation)
            .Include(m => m.IdTipoDocumentoNavigation)
            .ToListAsync();

        return medicos.Select(MapToDto);
    }

    public async Task<MedicoDto?> UpdateMedicoAsync(Guid id, CreateMedicoDto dto)
    {
        var medico = await _context.Medicos.FindAsync(id);
        if (medico == null) return null;

        // Actualizar campos de Usuario
        medico.Nombre = dto.Nombre;
        medico.Apellido = dto.Apellido;
        medico.Email = dto.Email;
        medico.Telefono = dto.Telefono;
        medico.Genero = dto.Genero;
        medico.FechaNacimiento = dto.FechaNacimiento;
        medico.FotoPerfil = dto.FotoPerfil;
        
        // Actualizar campos de Médico
        medico.Especialidad = dto.Especialidad;
        medico.CentroMedico = dto.CentroMedico;
        medico.NotificacionesActivadas = dto.NotificacionesActivadas;

        await _context.SaveChangesAsync();
        return MapToDto(medico);
    }

    public async Task<bool> DeleteMedicoAsync(Guid id)
    {
        var medico = await _context.Medicos.FindAsync(id);
        if (medico == null) return false;

        _context.Medicos.Remove(medico);
        await _context.SaveChangesAsync();
        return true;
    }

    private static MedicoDto MapToDto(Medico medico)
    {
        return new MedicoDto
        {
            IdUsuario = medico.IdUsuario,
            Nombre = medico.Nombre,
            Apellido = medico.Apellido,
            NroDocumento = medico.NroDocumento,
            Email = medico.Email,
            Telefono = medico.Telefono,
            Genero = medico.Genero,
            FechaNacimiento = medico.FechaNacimiento,
            FechaRegistro = medico.FechaRegistro,
            UltimoAcceso = medico.UltimoAcceso,
            Estado = medico.Estado,
            FotoPerfil = medico.FotoPerfil,
            TipoDocumento = medico.IdTipoDocumentoNavigation == null ? null : new TipoDocumentoDto
            {
                IdTipoDocumento = medico.IdTipoDocumentoNavigation.IdTipoDocumento,
                NombreDocumento = medico.IdTipoDocumentoNavigation.NombreDocumento,
                Descripcion = medico.IdTipoDocumentoNavigation.Descripcion
            },
            Rol = medico.IdRolNavigation == null ? null : new RolDto
            {
                IdRol = medico.IdRolNavigation.IdRol,
                NombreRol = medico.IdRolNavigation.NombreRol
            },
            Especialidad = medico.Especialidad,
            CentroMedico = medico.CentroMedico,
            NotificacionesActivadas = medico.NotificacionesActivadas,
            FirmaDigital = medico.FirmaDigital
        };
    }
}
