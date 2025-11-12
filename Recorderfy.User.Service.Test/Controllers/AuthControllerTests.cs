using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Recorderfy.User.Service.API.Controllers;
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.DTOs;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Recorderfy.User.Service.Test.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUsuarioService> _mockService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockService = new Mock<IUsuarioService>();
            _controller = new AuthController(_mockService.Object);

            // Configurar el contexto HTTP para que Response no sea null
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOkWithUser()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "juan@test.com",
                Password = "Password123"
            };

            var expectedResponse = new LoginResponseDto
            {
                Success = true,
                Message = "Login exitoso",
                Usuario = new UsuarioDto
                {
                    IdUsuario = Guid.NewGuid(),
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan@test.com",
                    NroDocumento = "12345678"
                }
            };

            _mockService.Setup(service => service.LoginAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Login exitoso", response.Message);
            Assert.NotNull(response.Usuario);
            _mockService.Verify(service => service.LoginAsync(It.IsAny<LoginDto>()), Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "juan@test.com",
                Password = "WrongPassword"
            };

            var expectedResponse = new LoginResponseDto
            {
                Success = false,
                Message = "Email o contraseña incorrectos",
                Usuario = null
            };

            _mockService.Setup(service => service.LoginAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(unauthorizedResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Email o contraseña incorrectos", response.Message);
            Assert.Null(response.Usuario);
            _mockService.Verify(service => service.LoginAsync(It.IsAny<LoginDto>()), Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "invalid-email",
                Password = ""
            };

            _controller.ModelState.AddModelError("Email", "Formato de email inválido");
            _controller.ModelState.AddModelError("Password", "La contraseña es requerida");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(badRequestResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Datos de entrada inválidos", response.Message);
            Assert.Null(response.Usuario);
            _mockService.Verify(service => service.LoginAsync(It.IsAny<LoginDto>()), Times.Never);
        }

        [Fact]
        public async Task Login_WithNullEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = null!,
                Password = "Password123"
            };

            _controller.ModelState.AddModelError("Email", "El email es requerido");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(service => service.LoginAsync(It.IsAny<LoginDto>()), Times.Never);
        }

        [Fact]
        public async Task Login_WithEmptyPassword_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "juan@test.com",
                Password = ""
            };

            _controller.ModelState.AddModelError("Password", "La contraseña es requerida");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(service => service.LoginAsync(It.IsAny<LoginDto>()), Times.Never);
        }
    }
}
