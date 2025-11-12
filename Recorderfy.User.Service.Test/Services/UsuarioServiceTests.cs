using Moq;
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.BLL.Services;
using Recorderfy.User.Service.DAL.Interfaces;
using Recorderfy.User.Service.Model.DTOs;
using Recorderfy.User.Service.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recorderfy.User.Service.Test.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockRepository;
        private readonly IUsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _mockRepository = new Mock<IUsuarioRepository>();
            _usuarioService = new UsuarioService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsuarios()
        {
            // Arrange
            var expectedUsuarios = new List<Usuario>
            {
                new Usuario
                {
                    IdUsuario = Guid.NewGuid(),
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan@test.com",
                    NroDocumento = "12345678",
                    PasswordHash = "hash123",
                    IdRol = 1,
                    IdTipoDocumento = 1,
                    FechaNacimiento = new DateOnly(1990, 1, 1),
                    FechaRegistro = DateTime.Now
                },
                new Usuario
                {
                    IdUsuario = Guid.NewGuid(),
                    Nombre = "María",
                    Apellido = "González",
                    Email = "maria@test.com",
                    NroDocumento = "87654321",
                    PasswordHash = "hash456",
                    IdRol = 2,
                    IdTipoDocumento = 1,
                    FechaNacimiento = new DateOnly(1985, 5, 15),
                    FechaRegistro = DateTime.Now
                }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedUsuarios);

            // Act
            var result = await _usuarioService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnUsuario()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var expectedUsuario = new Usuario
            {
                IdUsuario = usuarioId,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@test.com",
                NroDocumento = "12345678",
                PasswordHash = "hash123",
                IdRol = 1,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                FechaRegistro = DateTime.Now
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(usuarioId))
                .ReturnsAsync(expectedUsuario);

            // Act
            var result = await _usuarioService.GetByIdAsync(usuarioId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuarioId, result.IdUsuario);
            Assert.Equal("Juan", result.Nombre);
            _mockRepository.Verify(repo => repo.GetByIdAsync(usuarioId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(usuarioId))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _usuarioService.GetByIdAsync(usuarioId);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(usuarioId), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateUsuario()
        {
            // Arrange
            var newUsuario = new Usuario
            {
                IdUsuario = Guid.NewGuid(),
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@test.com",
                NroDocumento = "12345678",
                PasswordHash = "hash123",
                IdRol = 1,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                FechaRegistro = DateTime.Now
            };

            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Usuario>()))
                .Returns(Task.CompletedTask);
            _mockRepository.Setup(repo => repo.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _usuarioService.CreateAsync(newUsuario);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newUsuario.IdUsuario, result.IdUsuario);
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Usuario>()), Times.Once);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldUpdateUsuario()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var existingUsuario = new Usuario
            {
                IdUsuario = usuarioId,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@test.com",
                NroDocumento = "12345678",
                PasswordHash = "hash123",
                IdRol = 1,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                FechaRegistro = DateTime.Now
            };

            var updatedUsuario = new Usuario
            {
                IdUsuario = usuarioId,
                Nombre = "Juan Carlos",
                Apellido = "Pérez García",
                Email = "juancarlos@test.com",
                NroDocumento = "12345678",
                PasswordHash = "hash123",
                IdRol = 1,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                FechaRegistro = DateTime.Now
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(usuarioId))
                .ReturnsAsync(existingUsuario);
            _mockRepository.Setup(repo => repo.Update(It.IsAny<Usuario>()));
            _mockRepository.Setup(repo => repo.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _usuarioService.UpdateAsync(updatedUsuario);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuarioId, result.IdUsuario);
            _mockRepository.Verify(repo => repo.GetByIdAsync(usuarioId), Times.Once);
            _mockRepository.Verify(repo => repo.Update(It.IsAny<Usuario>()), Times.Once);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var usuario = new Usuario
            {
                IdUsuario = usuarioId,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@test.com",
                NroDocumento = "12345678",
                PasswordHash = "hash123",
                IdRol = 1,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                FechaRegistro = DateTime.Now
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(usuarioId))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _usuarioService.UpdateAsync(usuario);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(usuarioId), Times.Once);
            _mockRepository.Verify(repo => repo.Update(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var existingUsuario = new Usuario
            {
                IdUsuario = usuarioId,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@test.com",
                NroDocumento = "12345678",
                PasswordHash = "hash123",
                IdRol = 1,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                FechaRegistro = DateTime.Now
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(usuarioId))
                .ReturnsAsync(existingUsuario);
            _mockRepository.Setup(repo => repo.Delete(It.IsAny<Usuario>()));
            _mockRepository.Setup(repo => repo.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _usuarioService.DeleteAsync(usuarioId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(usuarioId), Times.Once);
            _mockRepository.Verify(repo => repo.Delete(It.IsAny<Usuario>()), Times.Once);
            _mockRepository.Verify(repo => repo.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(usuarioId))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _usuarioService.DeleteAsync(usuarioId);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(usuarioId), Times.Once);
            _mockRepository.Verify(repo => repo.Delete(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var email = "juan@test.com";
            var password = "Password123";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var usuario = new Usuario
            {
                IdUsuario = Guid.NewGuid(),
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = email,
                NroDocumento = "12345678",
                PasswordHash = passwordHash,
                IdRol = 1,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                FechaRegistro = DateTime.Now,
                IdRolNavigation = new Rol { IdRol = 1, NombreRol = "Usuario" },
                IdTipoDocumentoNavigation = new TipoDocumento { IdTipoDocumento = 1, NombreDocumento = "DNI" }
            };

            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            _mockRepository.Setup(repo => repo.GetByEmailWithRelationsAsync(email))
                .ReturnsAsync(usuario);

            // Act
            var result = await _usuarioService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Login exitoso", result.Message);
            Assert.NotNull(result.Usuario);
            Assert.Equal(email, result.Usuario.Email);
            _mockRepository.Verify(repo => repo.GetByEmailWithRelationsAsync(email), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidEmail_ShouldReturnFailure()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "noexiste@test.com",
                Password = "Password123"
            };

            _mockRepository.Setup(repo => repo.GetByEmailWithRelationsAsync(loginDto.Email))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _usuarioService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Email o contraseña incorrectos", result.Message);
            Assert.Null(result.Usuario);
            _mockRepository.Verify(repo => repo.GetByEmailWithRelationsAsync(loginDto.Email), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ShouldReturnFailure()
        {
            // Arrange
            var email = "juan@test.com";
            var correctPassword = "Password123";
            var wrongPassword = "WrongPassword";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            var usuario = new Usuario
            {
                IdUsuario = Guid.NewGuid(),
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = email,
                NroDocumento = "12345678",
                PasswordHash = passwordHash,
                IdRol = 1,
                IdTipoDocumento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                FechaRegistro = DateTime.Now,
                IdRolNavigation = new Rol { IdRol = 1, NombreRol = "Usuario" },
                IdTipoDocumentoNavigation = new TipoDocumento { IdTipoDocumento = 1, NombreDocumento = "DNI" }
            };

            var loginDto = new LoginDto
            {
                Email = email,
                Password = wrongPassword
            };

            _mockRepository.Setup(repo => repo.GetByEmailWithRelationsAsync(email))
                .ReturnsAsync(usuario);

            // Act
            var result = await _usuarioService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Email o contraseña incorrectos", result.Message);
            Assert.Null(result.Usuario);
            _mockRepository.Verify(repo => repo.GetByEmailWithRelationsAsync(email), Times.Once);
        }

        [Fact]
        public async Task GetByDocumentAndRoleAsync_WithValidData_ShouldReturnUsuario()
        {
            // Arrange
            var nroDocumento = "12345678";
            var idRol = 1;

            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    IdUsuario = Guid.NewGuid(),
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan@test.com",
                    NroDocumento = nroDocumento,
                    PasswordHash = "hash123",
                    IdRol = idRol,
                    IdTipoDocumento = 1,
                    FechaNacimiento = new DateOnly(1990, 1, 1),
                    FechaRegistro = DateTime.Now,
                    IdRolNavigation = new Rol { IdRol = 1, NombreRol = "Usuario" },
                    IdTipoDocumentoNavigation = new TipoDocumento { IdTipoDocumento = 1, NombreDocumento = "DNI" }
                }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(usuarios);
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(usuarios[0]);

            // Act
            var result = await _usuarioService.GetByDocumentAndRoleAsync(nroDocumento, idRol);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nroDocumento, result.NroDocumento);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByDocumentAndRoleAsync_WithInvalidData_ShouldReturnNull()
        {
            // Arrange
            var nroDocumento = "99999999";
            var idRol = 1;

            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    IdUsuario = Guid.NewGuid(),
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan@test.com",
                    NroDocumento = "12345678",
                    PasswordHash = "hash123",
                    IdRol = 2,
                    IdTipoDocumento = 1,
                    FechaNacimiento = new DateOnly(1990, 1, 1),
                    FechaRegistro = DateTime.Now
                }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(usuarios);

            // Act
            var result = await _usuarioService.GetByDocumentAndRoleAsync(nroDocumento, idRol);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}
