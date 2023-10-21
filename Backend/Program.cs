using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;
using Backend.DataBaseController;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Backend;
using Microsoft.Extensions.Options;
using Backend.Controller;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

var builder = WebApplication.CreateBuilder(args);
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Authorization.PrivateKey())
    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();

app.MapUserRoutes();
app.MapRequestRoutes();
app.MapQuestionRoutes();
app.MapTestModuleRoutes();
app.MapUserAnswerRoutes();

var currentDirectory = Directory.GetCurrentDirectory();
var parentDirectory = Path.GetDirectoryName(currentDirectory);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(parentDirectory, "Frontend")),
    RequestPath = string.Empty
});

app.Map("/", context =>
{
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
        context.Response.Redirect("/registration");
        return Task.CompletedTask;
    }
    else
    {
        string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Authorization.PrivateKey()),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            context.Response.Redirect("/login");
        }
        catch
        {
            context.Response.Redirect("/registration");
        }

        return Task.CompletedTask;
    }
});

app.Map("/registration", context =>
{
    return context.Response.WriteAsync(Helper.GetHTML("registration"));
});

app.Map("/login", context =>
{
    return context.Response.WriteAsync(Helper.GetHTML("login"));
});

app.Run();