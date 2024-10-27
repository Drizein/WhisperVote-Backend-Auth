using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.UseCases;

public class UCRegisterTest
{
    [Fact]
    public async Task Register_ReturnsSuccess_WhenUserIsRegistered()
    {
        var loggerMock = new Mock<ILogger<UCRegister>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var identRepositoryMock = new Mock<IIdentRepository>();
        var registerDTO = new RegisterDTO("newUser", "password", true, "ident123");

        userRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync((User)null);
        identRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<Ident, bool>>>()))
            .ReturnsAsync((Ident)null);
        userRepositoryMock.Setup(repo => repo.SelectAsync())
            .ReturnsAsync(new List<User>());

        var ucRegister = new UCRegister(loggerMock.Object, userRepositoryMock.Object, identRepositoryMock.Object);

        var result = await ucRegister.Register(registerDTO);

        Assert.True(result.Success);
        Assert.Equal("User registriert", result.Message);
    }

    [Fact]
    public async Task Register_ReturnsFailure_WhenAGBNotAccepted()
    {
        var loggerMock = new Mock<ILogger<UCRegister>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var identRepositoryMock = new Mock<IIdentRepository>();
        var registerDTO = new RegisterDTO("newUser", "password", false, "ident123");

        var ucRegister = new UCRegister(loggerMock.Object, userRepositoryMock.Object, identRepositoryMock.Object);

        var result = await ucRegister.Register(registerDTO);

        Assert.False(result.Success);
        Assert.Equal("AGB nicht akzeptiert", result.Message);
    }

    [Fact]
    public async Task Register_ReturnsFailure_WhenIdentIsDuplicate()
    {
        var loggerMock = new Mock<ILogger<UCRegister>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var identRepositoryMock = new Mock<IIdentRepository>();
        var registerDTO = new RegisterDTO("newUser", "password", true, "ident123");

        identRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<Ident, bool>>>()))
            .ReturnsAsync(new Ident("ident123"));

        var ucRegister = new UCRegister(loggerMock.Object, userRepositoryMock.Object, identRepositoryMock.Object);

        var result = await ucRegister.Register(registerDTO);

        Assert.False(result.Success);
        Assert.Equal("Ident doppelt", result.Message);
    }

    [Fact]
    public async Task Register_ReturnsFailure_WhenUsernameIsTaken()
    {
        var loggerMock = new Mock<ILogger<UCRegister>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var identRepositoryMock = new Mock<IIdentRepository>();
        var registerDTO = new RegisterDTO("existingUser", "password", true, "ident123");

        userRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new User("existingUser", "hashedPassword"));

        var ucRegister = new UCRegister(loggerMock.Object, userRepositoryMock.Object, identRepositoryMock.Object);

        var result = await ucRegister.Register(registerDTO);

        Assert.False(result.Success);
        Assert.Equal("Benutzername vergeben", result.Message);
    }
}