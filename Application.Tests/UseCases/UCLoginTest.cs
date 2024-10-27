using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases;
using Application.Util;
using Domain.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.UseCases;

[TestSubject(typeof(UCLogin))]
public class UCLoginTest
{
    private IConfiguration LoadConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.ToString())
                .Parent.ToString() + "\\Presentation")
            .AddJsonFile("appsettings.json")
            .Build();
        return configuration;
    }

    [Fact]
    public async Task Login_ReturnsSuccessAndToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UCRegister>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var loginDTO = new LoginDTO("validUser", "Passwort123!");
        var user = new User("validUser",
            "AQAAAAEAACcQAAAAEAnSsVmbKB7EfE8oOJYh0RXCfwoNxm+vR5p/UWKxc3IH43j43f651X3gPx1k3wKMag=="); // "Password123!" as Hash

        userRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);

        var passwordUtilMock = new Mock<IPasswordUtil>();
        passwordUtilMock.Setup(util => util.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        var jwtUtilMock = new Mock<IJwtUtil>();
        jwtUtilMock.Setup(util => util.GenerateToken(It.IsAny<User>()))
            .Returns("validToken");

        var ucLogin = new UCLogin(loggerMock.Object, userRepositoryMock.Object);

        // Act
        var result = await ucLogin.Login(loginDTO);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Message is string);
    }

    [Fact]
    public async Task Login_ReturnsFailure_WhenUserNotFound()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UCRegister>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var loginDTO = new LoginDTO("invalidUser", "password");

        userRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync((User)null);

        var ucLogin = new UCLogin(loggerMock.Object, userRepositoryMock.Object);

        // Act
        var result = await ucLogin.Login(loginDTO);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Benutzername oder Passwort falsch", result.Message);
    }

    [Fact]
    public async Task Login_ReturnsFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UCRegister>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var loginDTO = new LoginDTO("validUser", "invalidPassword");
        var user = new User("validUser", "cGFzc3dvcmQ="); // "password" in Base-64

        userRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);

        var passwordUtilMock = new Mock<IPasswordUtil>();
        passwordUtilMock.Setup(util => util.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var ucLogin = new UCLogin(loggerMock.Object, userRepositoryMock.Object);

        // Act
        var result = await ucLogin.Login(loginDTO);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Benutzername oder Passwort falsch", result.Message);
    }
}