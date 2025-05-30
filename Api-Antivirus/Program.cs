using Api_Antivirus.Config;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno desde el archivo .env
Env.Load();
// Acceder a las variables de entorno 
string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
 
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //options.UseNpgsql(connectionString));


// Asegura que .NET reemplace ${VARIABLES} por entornos
//builder.Configuration.AddEnvironmentVariables()
//.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.WebHost.UseUrls("http://*:80");


//configuracion de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Antivirus V1");
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }
    await next();
});

//estoy imprimiendo la conexion de bd
//Console.WriteLine("CADENA DE CONEXIÓN: " + connectionString);

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
