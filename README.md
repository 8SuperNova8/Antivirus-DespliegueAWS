El objetivo de este proyecto es desarrollar una  series de API’S desarrollada con .NET , C# , en las cuales se pueden gestionar bases como lo son ,usuarios, instituciones, oportunidades,  bootcamps y como se relacionan entre ellas.
Se Utilizan  PostgreSQL y maven como base de datos y la documentación de la API se genera con Swagger/OpenApi, además la autenticación para el rol de administrador con JWT.
#s
Tabla de contenido

Tecnologías utilizadas
Instalacion
Creacion base de datos 
Modificaciones MVC
Estrutura del proyecto
Uso de las API’S
Documentación de las API’S

TECNOLOGIAS UTILIZADAS

GitHub para la gestión del repositorio.
.NET9, C#, Entity Framework Core para el desarrollo del backend.
PostgreSQL como base de datos. 
Swagger/OpenAPI para la documentación.
 JWT para la autenticación.

INSTALACION
-Clonar en un tu equipo local el siguiente repositorio:
git clone https://github.com/Backend-API-REST-Antivirus
Cd Api-Antivirus/

-Una vez clonado el repositorio y navegas hacia la carpeta asegúrate de ejecutar los siguientes comandos:

dotnet add package DotNetEnv

dotnet add package Microsoft.EntityFrameworkCore

dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet add package Microsoft.EntityFrameworkCore.SqlServer

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

dotnet tool install --global dotnet-ef

Configurar la conexión a PostgreSQL 

Modificar appsettings.json Añade la cadena de conexión para PostgreSQL: 

{ "ConnectionStrings": { "DefaultConnection": "Host=localhost;Port=5432;Database=antivirus;Username=tu_usuario;Password=tu_cont raseña" } }

CREACION DE LA BASE DE DATOS

Crear el DbContext

Crea la clase ApplicationDbContext.cs en la carpeta Data/:

using Microsoft.EntityFrameworkCore;
using Api_Antivirus.Models;
using System;
using System.Collections.Generic;

namespace Api_Antivirus.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Agrega DbSet para cada modelo

        public DbSet<Category> Categories { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<OpportunityInstitution> OpportunityInstitutions { get; set; }
        public DbSet<User_Opportunity> UserOpportunities { get; set; }
        public DbSet<Bootcamp> Bootcamps { get; set; }
        public DbSet<BootcampTopics> BootcampTopics { get; set; }
        public DbSet<InstitutionBootcamp> InstitutionBootcamps { get; set; }

MODIFICAR MVC
-Crear el modelo de datos para cada una de las tablas 

using System.ComponentModel.DataAnnotations;
using Api_Antivirus.Models;

namespace Api_Antivirus.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }

        public required IEnumerable<Opportunity> Opportunities {get; set;}
    }
}

-Crea y aplica las siguientes migraciones

dotnet tool install --global dotnet-ef (para instalar la herramienta) 
dotnet ef migrations add InitialCreate

Aplicar la migración y crear la base de datos

dotnet ef database update 

Verifica que la base de datos y la tablas  se crearon correctamente en PostgreSQL con el comando: 

psql -U postgres -d ProductDb -c "\dt"

Implementar el servicio

Crear la interfaz

using Api_Antivirus.Models;

namespace Api_Antivirus.Services
{
    public interface ICategoryService
    {
        Task <IEnumerable<Category>> GetAllAsync();        
        Task<Category?> GetByIdAsync(int id);
        Task CreateAsync (Category category);
        Task UpdateAsync (Category category);
        Task DeleteAsync (int id);
    }
}

Implementar CategoryService.cs

using Api_Antivirus.Data;
using Api_Antivirus.Models;
using Microsoft.EntityFrameworkCore;


namespace Api_Antivirus.Services 
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService (ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task <IEnumerable<Category>> GetAllAsync ()
        {  
            return await _context.Categories.ToListAsync();
        }
        public async Task <Category?> GetByIdAsync(int id)
        {
            var category= await _context.Categories.FindAsync(id);
            if (category != null)
            {
                return category;
            }
            Console.WriteLine("Categoria No encontrada");
            return null;
        }
        public async Task CreateAsync (Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            category.Id = category.Id;
        }
        public async Task UpdateAsync (Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync  (int id)
        {
            var category =await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}


Crear el controlador API

using Microsoft.AspNetCore.Mvc;
using Api_Antivirus.Services;
using Api_Antivirus.Models;


namespace Api_Antivirus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categorySercice;

        public CategoriesController (ICategoryService categoryService)
        {
            _categorySercice = categoryService;
        }

        [HttpGet]
        public async Task <ActionResult<IEnumerable<Category>>> GetAll ()
        {
            return Ok(await _categorySercice.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task <ActionResult<Category>> GetId (int Id)
        {
            var category = await _categorySercice.GetByIdAsync(Id);
            if (category != null)
            {
                return Ok(category);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<ActionResult> Create ([FromBody] Category category)
        {
            if(category == null)
            {
                return BadRequest("Los datos no pueden ser Nulos.");
            }

            try
            {
                await _categorySercice.CreateAsync(category);
                return CreatedAtAction(nameof(Create), new{ id = category.Id}, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Category category)
        {
            var existingCategory = await _categorySercice.GetByIdAsync(id);
            if (existingCategory != null)
            {
                category.Id = id;
                await _categorySercice.UpdateAsync(category);
                return NoContent();
            }
            return NotFound();
            
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete (int id)
        {
            var existingCategory = await _categorySercice.GetByIdAsync(id);
            if (existingCategory != null)
            {
                await _categorySercice.DeleteAsync(id);
                return NoContent();
            }
            return NotFound();
        }
    }
}


Configurar la Inyección de Dependencias (DI) 

Modifica Program.cs para registrar CategoryService: 

builder.Services.AddScoped<ICategoryService, CategoryService>();

Ejecutar y probar la API

Ejecuta los siguientes comandos:

dotnet build
dotnet run

ESTRUCTURA DEL PROYECTO

![Imagen de WhatsApp 2025-02-15 a las 13 44 00_59f4a638](https://github.com/user-attachments/assets/0d056bce-e2aa-471f-985e-4233bf14592f)



USO DE LAS API’S

Así se ven las estructuras de las rutas de la API:

* Instituciones : CRUD para gestionar instituciones educativas.
    * GET /api/instituciones: Obtiene la lista de instituciones.
    * POST /api/instituciones: Crea una nueva institución.
    * PUT /api/instituciones/{id}:Actualiza una institución.
    * DELETE /api/instituciones/{id}:Elimina una institución.

* Oportunidades : CRUD para gestionar oportunidades de estudio.
    * GET /api/oportunidades: Obtiene la lista de oportunidades.
    * POST /api/oportunidades:Crea una nueva oportunidad.
    * PUT /api/oportunidades/{id}:Actualiza una oportunidad.
    * DELETE /api/oportunidades/{id}:Elimina una oportunidad.

* Bootcamps : CRUD para gestionar bootcamps.
    * GET /api/bootcamps: Obtiene la lista de bootcamps.
    * POST /api/bootcamps:Crea un nuevo bootcamp.
    * PUT /api/bootcamps/{id}:Actualiza un bootcamp.
    * DELETE /api/bootcamps/{id}:Eliminar un bootcamp.

DOCUMENTACION DE LA API

La documentación de la API se genera automáticamente con Swagger/OpenAPI para acceder  a la interfaz de Swagger, abre tu navegador y dirígete a:

http://localhost:5041/swagger/index.html
