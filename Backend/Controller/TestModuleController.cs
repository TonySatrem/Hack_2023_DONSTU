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
    public static class TestModuleController
    {
        public static void MapTestModuleRoutes(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/test_module", async delegate (HttpContext context, ApplicationContext db)
            {
                var allTests = db.moduleTest.ToList();

                var json = JsonSerializer.Serialize(allTests, Helper.JsonOpt());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync(json);
            });

            app.MapGet("/api/test_module/{id}", async delegate (int id, HttpContext context, ApplicationContext db)
            {
                var test = db.moduleTest.FirstOrDefault(u => u.Id == id);

                if (test != null)
                {
                    var customObject = new
                    {
                        Header = test,
                        Questions = db.Questions.Where(q => q.TestModuleId == id)
                    };

                    var json = JsonSerializer.Serialize(customObject, Helper.JsonOpt());
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync(json);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Invalid id");
                }
            });

            app.MapPost("/api/test_module", async delegate (HttpContext context, ApplicationContext db)
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var test = JsonSerializer.Deserialize<TestModule>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (test == null)
                {
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    await context.Response.WriteAsync("No content for test");
                    return;
                }

                db.Add(test);
                db.SaveChanges();

                var customObject = new
                {
                    Id = test.Id,
                };

                var json = JsonSerializer.Serialize(customObject, Helper.JsonOpt());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync(json);
            });

            app.MapDelete("/api/test_module/{id}", async delegate (int id, HttpContext context, ApplicationContext db)
            {
                var test = db.moduleTest.FirstOrDefault(u => u.Id == id);

                if (test != null)
                {
                    var test_questions = db.Questions.Where(q => q.TestModuleId == id);
                    db.Questions.RemoveRange(test_questions);
                    db.Remove(test);
                    db.SaveChanges();

                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync("Test deleted");
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Invalid id");
                }
            });

            app.MapMethods("/api/test_module/{id}", new[] { "PATCH" }, async delegate (int id, HttpContext context, ApplicationContext db)
            {
                var test = db.moduleTest.FirstOrDefault(u => u.Id == id);
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var dest_test = JsonSerializer.Deserialize<TestModule>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (dest_test == null)
                {
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    await context.Response.WriteAsync("No content for test");
                    return;
                }

                if (test != null)
                {
                    test.Update(dest_test);
                    db.SaveChanges();

                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync("Test updated");
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Invalid id");
                }
            });
        }
    }
}
