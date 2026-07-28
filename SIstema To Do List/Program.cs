
using SIstema_To_Do_List.Interfaces;
using SIstema_To_Do_List.Servicios;

namespace SIstema_To_Do_List
{
    public class Program
    {
 
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registro del servicio de tareas con inyección de dependencias.
            // AddScoped: una instancia por cada request HTTP.
            builder.Services.AddScoped<ITarea, TareaServices>();

            // Configuración de CORS para permitir que el frontend
            // pueda consumir la API desde cualquier origen.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PermitirFrontend", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Activar la política de CORS definida arriba
            app.UseCors("PermitirFrontend");

            app.UseHttpsRedirection();

            // Habilita los archivos estáticos de la carpeta wwwroot
            // (index.html, styles.css, app.js)
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("PermitirFrontend");
            app.UseAuthorization();
            app.MapControllers();
            app.Run();

           
        }
    }
}
