using Microsoft.EntityFrameworkCore;
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.BLL.Services;
using Recorderfy.User.Service.DAL.Data;
using Recorderfy.User.Service.Model.DTOs;
using Recorderfy.User.Service.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recorderfy.User.Service.Test.Services
{
    public class MedicoServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IMedicoService _medicoService;

        public MedicoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _medicoService = new MedicoService(_context);

            // Seed data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var rol = new Rol { IdRol = 2, NombreRol = "Médico" };
            var tipoDoc = new TipoDocumento { IdTipoDocumento = 1, NombreDocumento = "DNI", Descripcion = "Documento Nacional de Identidad" };

            _context.Roles.Add(rol);
            _context.TipoDocumentos.Add(tipoDoc);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CreateMedicoAsync_WithValidData_ShouldCreateMedico()
        {
            // Arrange
            var createDto = new CreateMedicoDto
            {
                Nombre = "Dr. Juan",
                Apellido = "Pérez",
                IdTipoDocumento = 1,
                NroDocumento = "12345678",
                Email = "dr.juan@hospital.com",
                Telefono = "123456789",
                Password = "Password123",
                IdRol = 2,
                Genero = "M",
                FechaNacimiento = new DateOnly(1980, 1, 1),
                Especialidad = "Cardiología",
                CentroMedico = "Hospital Central",
                NotificacionesActivadas = true
            };

            // Act
            var result = await _medicoService.CreateMedicoAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dr. Juan", result.Nombre);
            Assert.Equal("Pérez", result.Apellido);
            Assert.Equal("dr.juan@hospital.com", result.Email);
            Assert.Equal("Cardiología", result.Especialidad);
            Assert.Equal("Hospital Central", result.CentroMedico);
            Assert.True(result.NotificacionesActivadas);

            var medicoInDb = await _context.Medicos.FindAsync(result.IdUsuario);
            Assert.NotNull(medicoInDb);
        }

        [Fact]
        public async Task CreateMedicoAsync_WithDuplicateEmail_ShouldThrowException()
        {
            // Arrange
            var medico = new Medico
            {
                IdUsuario = Guid.NewGuid(),
                Nombre = "Existing",
                Apellido = "Doctor",
                Email = "existing@hospital.com",
                NroDocumento = "11111111",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
                IdRol = 2,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1980, 1, 1),
                FechaRegistro = DateTime.Now,
                Estado = "activo",
                PacientesAsignados = "[]"
            };

            await _context.Medicos.AddAsync(medico);
            await _context.SaveChangesAsync();

            var createDto = new CreateMedicoDto
            {
                Nombre = "Dr. New",
                Apellido = "Doctor",
                IdTipoDocumento = 1,
                NroDocumento = "22222222",
                Email = "existing@hospital.com", // Email duplicado
                Telefono = "123456789",
                Password = "Password123",
                IdRol = 2,
                Genero = "M",
                FechaNacimiento = new DateOnly(1985, 1, 1),
                Especialidad = "Pediatría",
                CentroMedico = "Clínica Norte"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _medicoService.CreateMedicoAsync(createDto)
            );
        }

        [Fact]
        public async Task CreateMedicoAsync_WithDuplicateDocument_ShouldThrowException()
        {
            // Arrange
            var medico = new Medico
            {
                IdUsuario = Guid.NewGuid(),
                Nombre = "Existing",
                Apellido = "Doctor",
                Email = "existing@hospital.com",
                NroDocumento = "11111111",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
                IdRol = 2,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1980, 1, 1),
                FechaRegistro = DateTime.Now,
                Estado = "activo",
                PacientesAsignados = "[]"
            };

            await _context.Medicos.AddAsync(medico);
            await _context.SaveChangesAsync();

            var createDto = new CreateMedicoDto
            {
                Nombre = "Dr. New",
                Apellido = "Doctor",
                IdTipoDocumento = 1,
                NroDocumento = "11111111", // Documento duplicado
                Email = "new@hospital.com",
                Telefono = "123456789",
                Password = "Password123",
                IdRol = 2,
                Genero = "M",
                FechaNacimiento = new DateOnly(1985, 1, 1),
                Especialidad = "Pediatría",
                CentroMedico = "Clínica Norte"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _medicoService.CreateMedicoAsync(createDto)
            );
        }

        [Fact]
        public async Task GetMedicoByIdAsync_WithValidId_ShouldReturnMedico()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var medico = new Medico
            {
                IdUsuario = medicoId,
                Nombre = "Dr. Juan",
                Apellido = "Pérez",
                Email = "dr.juan@hospital.com",
                NroDocumento = "12345678",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
                IdRol = 2,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1980, 1, 1),
                FechaRegistro = DateTime.Now,
                Estado = "activo",
                Especialidad = "Cardiología",
                CentroMedico = "Hospital Central",
                PacientesAsignados = "[]"
            };

            await _context.Medicos.AddAsync(medico);
            await _context.SaveChangesAsync();

            // Act
            var result = await _medicoService.GetMedicoByIdAsync(medicoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(medicoId, result.IdUsuario);
            Assert.Equal("Dr. Juan", result.Nombre);
            Assert.Equal("Cardiología", result.Especialidad);
        }

        [Fact]
        public async Task GetMedicoByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var medicoId = Guid.NewGuid();

            // Act
            var result = await _medicoService.GetMedicoByIdAsync(medicoId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllMedicosAsync_ShouldReturnAllMedicos()
        {
            // Arrange
            var medico1 = new Medico
            {
                IdUsuario = Guid.NewGuid(),
                Nombre = "Dr. Juan",
                Apellido = "Pérez",
                Email = "dr.juan@hospital.com",
                NroDocumento = "12345678",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
                IdRol = 2,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1980, 1, 1),
                FechaRegistro = DateTime.Now,
                Estado = "activo",
                Especialidad = "Cardiología",
                CentroMedico = "Hospital Central",
                PacientesAsignados = "[]"
            };

            var medico2 = new Medico
            {
                IdUsuario = Guid.NewGuid(),
                Nombre = "Dra. María",
                Apellido = "González",
                Email = "dra.maria@hospital.com",
                NroDocumento = "87654321",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
                IdRol = 2,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1985, 5, 15),
                FechaRegistro = DateTime.Now,
                Estado = "activo",
                Especialidad = "Pediatría",
                CentroMedico = "Clínica Norte",
                PacientesAsignados = "[]"
            };

            await _context.Medicos.AddRangeAsync(medico1, medico2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _medicoService.GetAllMedicosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllMedicosAsync_WithNoMedicos_ShouldReturnEmptyList()
        {
            // Act
            var result = await _medicoService.GetAllMedicosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateMedicoAsync_WithValidId_ShouldUpdateMedico()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var medico = new Medico
            {
                IdUsuario = medicoId,
                Nombre = "Dr. Juan",
                Apellido = "Pérez",
                Email = "dr.juan@hospital.com",
                NroDocumento = "12345678",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
                IdRol = 2,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1980, 1, 1),
                FechaRegistro = DateTime.Now,
                Estado = "activo",
                Especialidad = "Cardiología",
                CentroMedico = "Hospital Central",
                PacientesAsignados = "[]"
            };

            await _context.Medicos.AddAsync(medico);
            await _context.SaveChangesAsync();

            var updateDto = new CreateMedicoDto
            {
                Nombre = "Dr. Juan Carlos",
                Apellido = "Pérez García",
                IdTipoDocumento = 1,
                NroDocumento = "12345678",
                Email = "dr.juancarlos@hospital.com",
                Telefono = "987654321",
                Password = "NewPassword123",
                IdRol = 2,
                Genero = "M",
                FechaNacimiento = new DateOnly(1980, 1, 1),
                Especialidad = "Cardiología Intervencionista",
                CentroMedico = "Hospital Universitario",
                NotificacionesActivadas = false
            };

            // Act
            var result = await _medicoService.UpdateMedicoAsync(medicoId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(medicoId, result.IdUsuario);
            Assert.Equal("Dr. Juan Carlos", result.Nombre);
            Assert.Equal("Cardiología Intervencionista", result.Especialidad);
            Assert.Equal("Hospital Universitario", result.CentroMedico);
            Assert.False(result.NotificacionesActivadas);
        }

        [Fact]
        public async Task UpdateMedicoAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var updateDto = new CreateMedicoDto
            {
                Nombre = "Dr. Juan",
                Apellido = "Pérez",
                IdTipoDocumento = 1,
                NroDocumento = "12345678",
                Email = "dr.juan@hospital.com",
                Telefono = "123456789",
                Password = "Password123",
                IdRol = 2,
                Genero = "M",
                FechaNacimiento = new DateOnly(1980, 1, 1),
                Especialidad = "Cardiología",
                CentroMedico = "Hospital Central"
            };

            // Act
            var result = await _medicoService.UpdateMedicoAsync(medicoId, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteMedicoAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var medico = new Medico
            {
                IdUsuario = medicoId,
                Nombre = "Dr. Juan",
                Apellido = "Pérez",
                Email = "dr.juan@hospital.com",
                NroDocumento = "12345678",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
                IdRol = 2,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1980, 1, 1),
                FechaRegistro = DateTime.Now,
                Estado = "activo",
                Especialidad = "Cardiología",
                CentroMedico = "Hospital Central",
                PacientesAsignados = "[]"
            };

            await _context.Medicos.AddAsync(medico);
            await _context.SaveChangesAsync();

            // Act
            var result = await _medicoService.DeleteMedicoAsync(medicoId);

            // Assert
            Assert.True(result);

            var deletedMedico = await _context.Medicos.FindAsync(medicoId);
            Assert.Null(deletedMedico);
        }

        [Fact]
        public async Task DeleteMedicoAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var medicoId = Guid.NewGuid();

            // Act
            var result = await _medicoService.DeleteMedicoAsync(medicoId);

            // Assert
            Assert.False(result);
        }
    }
}
