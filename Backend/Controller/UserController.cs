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

namespace Backend.Controller
{
    public static class UserController
    {
        public static void MapUserRoutes(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/users", async delegate (ApplicationContext db, HttpContext context)
            {
                Console.WriteLine("Try register user");
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                Console.WriteLine(body);
                var user = JsonSerializer.Deserialize<RegisterUser>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (user == null)
                {
                    Console.WriteLine("Null json");
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    await context.Response.WriteAsync("User is null");
                    return;
                }

                if (db.Users.FirstOrDefault(u => u.phonenumber == user.phonenumber) != null)
                {
                    Console.WriteLine("UserExists");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("User already exists.");
                    return;
                }

                AccountUser acc_user = new AccountUser().FromRegister(user);
                db.Add(acc_user);
                db.SaveChanges();
                context.Response.StatusCode = StatusCodes.Status200OK;

                var customObject = new
                {
                    Id = acc_user.Id,
                };

                var json = JsonSerializer.Serialize(customObject, Helper.JsonOpt());
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            });

            app.MapPost("/api/login", async delegate (ApplicationContext db, HttpContext context)
            {
                Console.WriteLine("Try login");
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var loginInfo = JsonSerializer.Deserialize<LoginInfo>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loginInfo == null)
                {
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    await context.Response.WriteAsync("Login info is null");
                    Console.WriteLine("Login info is null");
                    return;
                }

                var user = db.Users.FirstOrDefault(u => u.phonenumber == loginInfo.phonenumber && u.hashPassword == Helper.SHA512(loginInfo.password));
                if (user == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid username or password");
                    Console.WriteLine("Invalid username or password");
                    return;
                }

                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.phonenumber)
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Authorization.PrivateKey()), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                user.token = tokenString;
                db.SaveChanges();

                var customObject = new
                {
                    Id = user.Id,
                    token = user.token,
                };

                var json = JsonSerializer.Serialize(customObject, Helper.JsonOpt());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                Console.WriteLine(json);
                await context.Response.WriteAsync(json);
            });

            app.MapGet("/api/users/{id}", async delegate (int id, HttpContext context, ApplicationContext db)
            {
                var user = Authorization.Authorizate(id, db, context, Authorization.PrivateKey());
                if (user != null)
                {
                    var json = JsonSerializer.Serialize(user, Helper.JsonOpt());
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync(json);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token.");
                }
            });

            app.MapGet("/api/users/", async delegate (HttpContext context, ApplicationContext db)
            {
                var json = JsonSerializer.Serialize(db.Users.ToList(), Helper.JsonOpt());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync(json);
            });

            app.MapMethods("/api/users/{id}", new[] { "PATCH" }, async delegate (int id, HttpContext context, ApplicationContext db)
            {
                var user = Authorization.Authorizate(id, db, context, Authorization.PrivateKey());
                if (user != null)
                {
                    using var reader = new StreamReader(context.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    var dest_user = JsonSerializer.Deserialize<AccountUser>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (dest_user != null)
                    {
                        user.Update(dest_user);
                        db.SaveChanges();

                        var json = JsonSerializer.Serialize(user, Helper.JsonOpt());
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        await context.Response.WriteAsync(json);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status204NoContent;
                        await context.Response.WriteAsync("No content for user");
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token.");
                }
            });

            app.MapDelete("/api/users/{id}", async delegate (int id, HttpContext context, ApplicationContext db)
            {
                var user = Authorization.Authorizate(id, db, context, Authorization.PrivateKey());
                if (user != null)
                {
                    db.Remove(user);
                    db.SaveChanges();
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync("User deleted");
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token.");
                }
            });
        }
    }
}
