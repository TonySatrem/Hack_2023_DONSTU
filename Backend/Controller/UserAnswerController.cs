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
using static System.Net.Mime.MediaTypeNames;

namespace Backend.Controller
{
    public static class UserAnswerController
    {
        public static void MapUserAnswerRoutes(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/test_module/{id}/question/{q_id}/user/{u_id}/answer", async delegate (int id, int q_id, int u_id, HttpContext context, ApplicationContext db)
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var answer = JsonSerializer.Deserialize<UserAnswerTest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (answer == null)
                {
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    await context.Response.WriteAsync("No content for answer");
                    return;
                }

                var user = Authorization.Authorizate(u_id, db, context, Authorization.PrivateKey());
                if (user == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid user token");
                    return;
                }

                answer.AccountUserId = user.Id;
                answer.QuestionId = q_id;
                answer.TestModuleId = id;
                db.Add(answer);
                db.SaveChanges();

                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync("Answer created");
            });

            app.MapGet("/api/test_module/{id}/user/{u_id}/answer", async delegate (int id, int u_id, HttpContext context, ApplicationContext db)
            {
                var json = JsonSerializer.Serialize(db.TestsUsers.Where(a => a.AccountUserId == u_id && a.TestModuleId == id), Helper.JsonOpt());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync(json);
            });

            app.MapGet("/api/test_module/{id}/user/{u_id}/result", async delegate (int id, int u_id, HttpContext context, ApplicationContext db)
            {
                var customObject = new
                {
                    result = UserAnswerTest.GetScoreTest(db, id, u_id),
                };

                var json = JsonSerializer.Serialize(customObject, Helper.JsonOpt());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync(json);
            });
        }
    }
}
