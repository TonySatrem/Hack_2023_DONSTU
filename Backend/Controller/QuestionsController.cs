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
    public static class QuestionsController
    {
        public static void MapQuestionRoutes(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/test_module/{id}/questions", async delegate (int id, HttpContext context, ApplicationContext db)
            {
                var questions = db.Questions.Where(q => q.TestModuleId == id);
                var json = JsonSerializer.Serialize(questions, Helper.JsonOpt());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync(json);
            });

            app.MapGet("/api/test_module/{id}/questions/{q_id}", async delegate (int id, int q_id, HttpContext context, ApplicationContext db)
            {
                var questions = db.Questions.Where(q => q.TestModuleId == id);
                var question = questions.FirstOrDefault(q => q.id == q_id);
                var json = JsonSerializer.Serialize(question, Helper.JsonOpt());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync(json);
            });

            app.MapPost("/api/test_module/{id}/questions", async delegate (int id, HttpContext context, ApplicationContext db)
            {
                if (db.moduleTest.FirstOrDefault(t => t.Id == id) == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Test not found");
                    return;
                }

                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var question = JsonSerializer.Deserialize<QuestionsComponent>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (question == null)
                {
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    await context.Response.WriteAsync("No content for question");
                    return;
                }

                question.TestModuleId = id;
                db.Add(question);
                db.SaveChanges();

                var customObject = new
                {
                    Id = question.id,
                };

                var json = JsonSerializer.Serialize(customObject, Helper.JsonOpt());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync(json);
            });

            app.MapDelete("/api/test_module/{id}/questions/{q_id}", async delegate (int id, int q_id, HttpContext context, ApplicationContext db)
            {
                var questions = db.Questions.Where(q => q.TestModuleId == id);
                var question = questions.FirstOrDefault(q => q.id == q_id);

                if (question == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Invalid id");
                    return;
                }

                db.Remove(question);
                db.SaveChanges();

                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync("Question deleted");
            });

            app.MapMethods("/api/test_module/{id}/questions/{q_id}", new[] { "PATCH" }, async delegate (int id, int q_id, HttpContext context, ApplicationContext db)
            {
                var questions = db.Questions.Where(q => q.TestModuleId == id);
                var question = questions.FirstOrDefault(q => q.id == q_id);

                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var dest_question = JsonSerializer.Deserialize<QuestionsComponent>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (dest_question == null)
                {
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    await context.Response.WriteAsync("No content for question");
                    return;
                }

                if (question == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Invalid id");
                    return;
                }

                question.Update(dest_question);
                db.SaveChanges();

                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync("Question updated");
            });
        }
    }
}
