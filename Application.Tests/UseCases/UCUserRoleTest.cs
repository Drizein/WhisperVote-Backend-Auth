using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases;
using Application.Util;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.UseCases;

public class UCUserRoleTest
{
    [Fact]
    public async Task ChangeRole_ReturnsSuccess_WhenRoleIsChangedByOperator()
    {
        var loggerMock = new Mock<ILogger<UCUserRole>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var jwtUtilMock = new Mock<IJwtUtil>();
        var authorization = "validToken";
        var changeRoleDto = new ChangeRoleDTO("userId", Role.Moderator);

        var user = new User("TestUser", "Role.Operator123");
        user.Role = Role.Operator;
        var userToChangeRole = new User("UserToChangeRole", "Role.Operator123");

        userRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);
        userRepositoryMock.Setup(repo => repo.FindByAsync(x => x.Id == Guid.Parse(changeRoleDto.userId)))
            .ReturnsAsync(userToChangeRole);

        jwtUtilMock.Setup(jwt => jwt.ParseJwt(It.IsAny<string>())).Returns(user.Id.ToString());

        var ucUserRole = new UCUserRole(loggerMock.Object, userRepositoryMock.Object,
            jwtUtilMock.Object);

        var result = await ucUserRole.ChangeRole(authorization, changeRoleDto);

        Assert.True(result.Success);
        Assert.Equal("Rolle geändert", result.Message);
    }

    [Fact]
    public async Task ChangeRole_ReturnsFailure_WhenUserNotFound()
    {
        var loggerMock = new Mock<ILogger<UCUserRole>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var jwtUtilMock = new Mock<IJwtUtil>();
        var authorization = "validToken";
        var changeRoleDto = new ChangeRoleDTO("invalidUserId", Role.User);

        var user = new User("TestUser", "Role.Operator123");

        userRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);
        userRepositoryMock.Setup(repo => repo.FindByAsync(x => x.Id == Guid.Parse(changeRoleDto.userId)))
            .ReturnsAsync((User)null);
        jwtUtilMock.Setup(jwt => jwt.ParseJwt(It.IsAny<string>())).Returns("userId");

        var ucUserRole = new UCUserRole(loggerMock.Object, userRepositoryMock.Object,
            jwtUtilMock.Object);

        var result = await ucUserRole.ChangeRole(authorization, changeRoleDto);

        Assert.False(result.Success);
        Assert.Equal("Zuändernden User nicht gefunden", result.Message);
    }

    [Fact]
    public async Task GetRole_ReturnsRole_WhenAuthorizationIsValid()
    {
        var loggerMock = new Mock<ILogger<UCUserRole>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var jwtUtilMock = new Mock<IJwtUtil>();
        var authorization = "validToken";

        var user = new User("TestUser", "Role.Operator123");
        user.Role = Role.Operator;

        userRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);
        jwtUtilMock.Setup(jwt => jwt.ParseJwt(It.IsAny<string>())).Returns(user.Id.ToString());

        var ucUserRole = new UCUserRole(loggerMock.Object, userRepositoryMock.Object,
            jwtUtilMock.Object);

        var result = await ucUserRole.GetRole(authorization);

        Assert.True(result.Success);
        Assert.Equal("Operator", result.Message);
    }

    [Fact]
    public async Task GetRole_ReturnsFailure_WhenAuthorizationIsInvalid()
    {
        var loggerMock = new Mock<ILogger<UCUserRole>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var authorization = "invalidToken";

        var ucUserRole = new UCUserRole(loggerMock.Object, userRepositoryMock.Object);

        var result = await ucUserRole.GetRole(authorization);

        Assert.False(result.Success);
        Assert.Equal("Fehler beim parsen des JWT", result.Message);
    }

    [Fact]
    public async Task StrikeUser_ReturnsSuccess_WhenUserIsStruck()
    {
        var loggerMock = new Mock<ILogger<UCUserRole>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var jwtUtilMock = new Mock<IJwtUtil>();
        var authorization = "validToken";
        var strikedUserName = "strikedUserName";

        var user = new User("TestUser", "Role.Operator123");
        user.Role = Role.Operator;
        var userToStrike = new User(strikedUserName, "Role.Operator123");

        userRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);
        userRepositoryMock.Setup(repo => repo.FindByAsync(x => x.Username == strikedUserName))
            .ReturnsAsync(userToStrike);
        jwtUtilMock.Setup(jwt => jwt.ParseJwt(It.IsAny<string>())).Returns(user.Id.ToString());

        var ucUserRole = new UCUserRole(loggerMock.Object, userRepositoryMock.Object,
            jwtUtilMock.Object);

        var result = await ucUserRole.StrikeUser(authorization, userToStrike.Id);

        Assert.True(result.Success);
        Assert.Equal("Benutzer gestriked", result.Message);
    }

    [Fact]
    public async Task StrikeUser_ReturnsFailure_WhenUserNotFound()
    {
        var loggerMock = new Mock<ILogger<UCUserRole>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var jwtUtilMock = new Mock<IJwtUtil>();
        var authorization = "validToken";
        var strikedUserId = Guid.NewGuid();

        var user = new User("TestUser", "Role.Operator123");
        user.Role = Role.Operator;

        userRepositoryMock.Setup(repo => repo.FindByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);
        userRepositoryMock.Setup(repo => repo.FindByAsync(x => x.Id == strikedUserId))
            .ReturnsAsync((User)null);
        jwtUtilMock.Setup(jwt => jwt.ParseJwt(It.IsAny<string>())).Returns("userId");

        var ucUserRole = new UCUserRole(loggerMock.Object, userRepositoryMock.Object,
            jwtUtilMock.Object);

        var result = await ucUserRole.StrikeUser(authorization, strikedUserId);

        Assert.False(result.Success);
        Assert.Equal("Benutzer nicht gefunden", result.Message);
    }
}