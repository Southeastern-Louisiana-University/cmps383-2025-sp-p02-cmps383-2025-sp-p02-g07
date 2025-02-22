using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Features.Users;
using Selu383.SP25.P02.Api.Features.Roles;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;

namespace Selu383.SP25.P02.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")
                ?? throw new InvalidOperationException("Connection string 'DataContext' not found.")));

            // Configure Identity
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = false;
            })
             .AddEntityFrameworkStores<DataContext>()
             .AddDefaultTokenProviders();
            builder.Services.AddScoped<RoleManager<Role>>();

            builder.Services.AddAuthorization();

            // Configure Cookie Authentication
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/api/auth/login";
                options.LogoutPath = "/api/auth/logout";
                options.AccessDeniedPath = "/api/auth/access-denied";
                options.SlidingExpiration = true;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()  // Or specify specific origins with .WithOrigins("http://localhost:5173")
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });



            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                await SeedTheaters.Initialize(scope.ServiceProvider);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                    c.RoutePrefix = string.Empty;  // Makes Swagger UI available at the root
                });
                app.MapOpenApi();



                app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseRouting();

                app.UseCors("AllowAll");

                app.UseAuthorization();  // <-- Authorization should come after Authentication

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

                app.UseStaticFiles();
                if (app.Environment.IsDevelopment())
                {
                    app.UseSpa(x =>
                    {
                        x.UseProxyToSpaDevelopmentServer("http://localhost:5173");
                    });
                }
                else
                {
                    app.MapFallbackToFile("/index.html");
                }
                app.MapControllers();

                app.Run();
            }
        }
    }
}
