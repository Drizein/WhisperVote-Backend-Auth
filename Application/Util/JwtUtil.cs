﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Util;

public class JwtUtil : IJwtUtil
{
    public JwtUtil()
    {
    }

    public string GenerateToken(User user)
    {
        var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt_Key")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(null, null,
            claims,
            expires: DateTime.Now.AddMinutes(double.Parse(Environment.GetEnvironmentVariable("TokenLifetime"))),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string ParseJwt(string token)
    {
        // JWT besteht aus drei Teilen: Header, Payload, und Signature, getrennt durch Punkte ('.')
        var parts = token.Split('.');

        if (parts.Length != 3) throw new ArgumentException("Ungültiges JWT-Format.");

        // Der Payload-Teil ist Base64-url-encoded
        var payload = parts[1];

        // Base64 URL Dekodierung
        var jsonBytes = DecodeBase64Url(payload);

        // JSON-String in ein dynamisches Objekt umwandeln
        var jsonPayload = Encoding.UTF8.GetString(jsonBytes);
        var jsonDocument = JsonDocument.Parse(jsonPayload);

        // Den NameIdentifier (oft als "sub" oder "nameid") aus dem JSON extrahieren
        if (jsonDocument.RootElement.TryGetProperty("nameidentifier", out var nameIdentifier))
            return nameIdentifier.GetString();
        if (jsonDocument.RootElement.TryGetProperty(
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", out nameIdentifier))
            return nameIdentifier.GetString();

        throw new Exception("NameIdentifier nicht gefunden.");
    }

    private static byte[] DecodeBase64Url(string input)
    {
        var base64 = input
            .Replace('-', '+') // URL-sichere Zeichen ersetzen
            .Replace('_', '/');

        // Base64 muss ein Vielfaches von 4 sein. Bei Bedarf mit '=' auffüllen.
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
}