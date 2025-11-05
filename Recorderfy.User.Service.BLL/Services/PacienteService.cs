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

public class PacienteService : IPacienteService
{
    private readonly ApplicationDbContext _context;

    public PacienteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PacienteDto> CreatePacienteAsync(CreatePacienteDto dto)
    {
        // Validar que no exista el email o documento
        var exists = await _context.Usuarios
            .AnyAsync(u => u.Email == dto.Email || u.NroDocumento == dto.NroDocumento);
        
        if (exists)
            throw new InvalidOperationException("Ya existe un usuario con ese email o documento");

        var paciente = new Paciente
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
            // Campos específicos de Paciente
            DiagnosticoInicial = dto.DiagnosticoInicial,
            FechaIngreso = DateOnly.FromDateTime(DateTime.Today),
            IdCuidador = dto.IdCuidador,
            IdMedico = dto.IdMedico,
            ObservacionesClinicas = dto.ObservacionesClinicas,
            FotoReferencia = dto.FotoReferencia
        };

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        var savedPaciente = await GetPacienteByIdAsync(paciente.IdUsuario);

        if (savedPaciente == null)
            throw new Exception("No se pudo recuperar el médico después de guardarlo.");

        return savedPaciente;
    }

    public async Task<PacienteDto?> GetPacienteByIdAsync(Guid id)
    {
        var paciente = await _context.Pacientes
            .Include(p => p.IdRolNavigation)
            .Include(p => p.IdTipoDocumentoNavigation)
            .Include(p => p.Cuidador)
            .Include(p => p.Medico)
            .FirstOrDefaultAsync(p => p.IdUsuario == id);

        return paciente == null ? null : MapToDto(paciente);
    }

    public async Task<IEnumerable<PacienteDto>> GetAllPacientesAsync()
    {
        var pacientes = await _context.Pacientes
            .Include(p => p.IdRolNavigation)
            .Include(p => p.IdTipoDocumentoNavigation)
            .Include(p => p.Cuidador)
            .Include(p => p.Medico)
            .ToListAsync();

        return pacientes.Select(MapToDto);
    }

    public async Task<PacienteDto?> UpdatePacienteAsync(Guid id, CreatePacienteDto dto)
    {
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return null;

        // Actualizar campos de Usuario
        paciente.Nombre = dto.Nombre;
        paciente.Apellido = dto.Apellido;
        paciente.Email = dto.Email;
        paciente.Telefono = dto.Telefono;
        paciente.Genero = dto.Genero;
        paciente.FechaNacimiento = dto.FechaNacimiento;
        paciente.FotoPerfil = dto.FotoPerfil;
        
        // Actualizar campos de Paciente
        paciente.DiagnosticoInicial = dto.DiagnosticoInicial;
        paciente.IdCuidador = dto.IdCuidador;
        paciente.IdMedico = dto.IdMedico;
        paciente.ObservacionesClinicas = dto.ObservacionesClinicas;
        paciente.FotoReferencia = dto.FotoReferencia;

        await _context.SaveChangesAsync();
        return await GetPacienteByIdAsync(id);
    }

    public async Task<bool> DeletePacienteAsync(Guid id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return false;

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();
        return true;
    }

    private static PacienteDto MapToDto(Paciente paciente)
    {
        return new PacienteDto
        {
            IdUsuario = paciente.IdUsuario,
            Nombre = paciente.Nombre,
            Apellido = paciente.Apellido,
            NroDocumento = paciente.NroDocumento,
            Email = paciente.Email,
            Telefono = paciente.Telefono,
            Genero = paciente.Genero,
            FechaNacimiento = paciente.FechaNacimiento,
            FechaRegistro = paciente.FechaRegistro,
            UltimoAcceso = paciente.UltimoAcceso,
            Estado = paciente.Estado,
            FotoPerfil = paciente.FotoPerfil,
            TipoDocumento = paciente.IdTipoDocumentoNavigation == null ? null : new TipoDocumentoDto
            {
                IdTipoDocumento = paciente.IdTipoDocumentoNavigation.IdTipoDocumento,
                NombreDocumento = paciente.IdTipoDocumentoNavigation.NombreDocumento,
                Descripcion = paciente.IdTipoDocumentoNavigation.Descripcion
            },
            Rol = paciente.IdRolNavigation == null ? null : new RolDto
            {
                IdRol = paciente.IdRolNavigation.IdRol,
                NombreRol = paciente.IdRolNavigation.NombreRol
            },
            DiagnosticoInicial = paciente.DiagnosticoInicial,
            FechaIngreso = paciente.FechaIngreso,
            ObservacionesClinicas = paciente.ObservacionesClinicas,
            FotoReferencia = paciente.FotoReferencia,
            IdCuidador = paciente.IdCuidador,
            NombreCuidador = paciente.Cuidador == null ? null : $"{paciente.Cuidador.Nombre} {paciente.Cuidador.Apellido}",
            IdMedico = paciente.IdMedico,
            NombreMedico = paciente.Medico == null ? null : $"{paciente.Medico.Nombre} {paciente.Medico.Apellido}"
        };
    }
}
