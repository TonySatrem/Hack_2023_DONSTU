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

namespace Backend.Controller
{
    public static class RequestController
    {
        public static void MapRequestRoutes(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/users/{id}/requests", async delegate (int id, HttpContext context, ApplicationContext db)
            {
                var user = Authorization.Authorizate(id, db, context, Authorization.PrivateKey());
                if (user != null)
                {
                    List<RequestUser> allRequests = db.RequestUsers.Where(u => u.AccountUserId == user.Id).ToList();

                    var json = JsonSerializer.Serialize(allRequests, Helper.JsonOpt());
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

            app.MapGet("/api/users/{id}/requests/{req_id}", async delegate (int id, int req_id, HttpContext context, ApplicationContext db)
            {
                var user = Authorization.Authorizate(id, db, context, Authorization.PrivateKey());
                if (user != null)
                {
                    var request = db.RequestUsers.FirstOrDefault(u => u.AccountUserId == user.Id && u.Id == req_id);

                    var json = JsonSerializer.Serialize(request, Helper.JsonOpt());
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

            app.MapPost("/api/users/{id}/requests", async delegate (int id, HttpContext context, ApplicationContext db)
            {
                var user = Authorization.Authorizate(id, db, context, Authorization.PrivateKey());
                if (user != null)
                {
                    using var reader = new StreamReader(context.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    var request = JsonSerializer.Deserialize<RequestUser>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (request == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status204NoContent;
                        await context.Response.WriteAsync("No content for request");
                        return;
                    }

                    request.AccountUserId = user.Id;
                    db.RequestUsers.Add(request);
                    db.SaveChanges();

                    var customObject = new
                    {
                        Id = request.Id,
                    };

                    var json = JsonSerializer.Serialize(customObject, Helper.JsonOpt());
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

            app.MapDelete("/api/users/{id}/requests/{req_id}", async delegate (int id, int req_id, HttpContext context, ApplicationContext db)
            {
                var user = Authorization.Authorizate(id, db, context, Authorization.PrivateKey());
                if (user != null)
                {
                    var request = db.RequestUsers.FirstOrDefault(u => u.AccountUserId == user.Id && u.Id == req_id);

                    if (request == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync("Request not found");
                        return;
                    }

                    db.Remove(request);
                    db.SaveChanges();

                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync("Request deleted");
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
