using Lab11.Data;
using Lab11.Models;
using Lab11.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventosDB")));

builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();
    await SeedData.InicializarAsync(context);
}

app.Run();

static class SeedData
{
    public static async Task InicializarAsync(EventosContext context)
    {
        if (!await context.Categorias.AnyAsync())
        {
            var categorias = new List<Categoria>
            {
                new() { Nombre = "Tecnología", Activo = true },
                new() { Nombre = "Música", Activo = true },
                new() { Nombre = "Deportes", Activo = true }
            };
            context.Categorias.AddRange(categorias);
            await context.SaveChangesAsync();
        }

        if (!await context.Eventos.AnyAsync())
        {
            var categorias = await context.Categorias.ToListAsync();
            var tecnologia = categorias.First(c => c.Nombre == "Tecnología");
            var musica = categorias.First(c => c.Nombre == "Música");
            var deportes = categorias.First(c => c.Nombre == "Deportes");

            var eventos = new List<Evento>
            {
                new()
                {
                    Titulo = "Conferencia .NET 2026",
                    Fecha = DateTime.Now.AddDays(30),
                    Lugar = "Centro de Convenciones Lima",
                    Aforo = 200,
                    Precio = 150.00m,
                    CategoriaId = tecnologia.Id,
                    Activo = true
                },
                new()
                {
                    Titulo = "Hackathon TECSUP",
                    Fecha = DateTime.Now.AddDays(45),
                    Lugar = "Campus TECSUP",
                    Aforo = 100,
                    Precio = 0.00m,
                    CategoriaId = tecnologia.Id,
                    Activo = true
                },
                new()
                {
                    Titulo = "Festival de Rock",
                    Fecha = DateTime.Now.AddDays(60),
                    Lugar = "Estadio Nacional",
                    Aforo = 5000,
                    Precio = 80.00m,
                    CategoriaId = musica.Id,
                    Activo = true
                },
                new()
                {
                    Titulo = "Maratón Lima 10K",
                    Fecha = DateTime.Now.AddDays(20),
                    Lugar = "Malecón de Miraflores",
                    Aforo = 1000,
                    Precio = 40.00m,
                    CategoriaId = deportes.Id,
                    Activo = true
                }
            };
            context.Eventos.AddRange(eventos);
            await context.SaveChangesAsync();
        }

        if (!await context.Asistentes.AnyAsync())
        {
            var asistentes = new List<Asistente>
            {
                new() { Nombres = "Ana Torres Ramírez", Dni = "71234567", Correo = "ana.torres@correo.com", Activo = true },
                new() { Nombres = "Luis García Pérez", Dni = "72345678", Correo = "luis.garcia@correo.com", Activo = true },
                new() { Nombres = "María López Quispe", Dni = "73456789", Correo = "maria.lopez@correo.com", Activo = true },
                new() { Nombres = "Carlos Fernández Soto", Dni = "74567890", Correo = "carlos.fernandez@correo.com", Activo = true },
                new() { Nombres = "Rosa Mamani Huamán", Dni = "75678901", Correo = "rosa.mamani@correo.com", Activo = true }
            };
            context.Asistentes.AddRange(asistentes);
            await context.SaveChangesAsync();
        }
    }
}
