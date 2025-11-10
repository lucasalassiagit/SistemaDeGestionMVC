using CrudMVCApp.Data;
using Microsoft.EntityFrameworkCore;
using CrudMVCApp.Models; 

var builder = WebApplication.CreateBuilder(args);

// Registrar tu DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

var app = builder.Build();




if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Crear usuarios al inicar la aplicacion
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    string adminUser = configuration["AdminUserSeed:Username"];
    string adminPass = configuration["AdminUserSeed:Password"];
    string adminTipo = configuration["AdminUserSeed:Tipo"];

    string demoUser = configuration["DemoUserSeed:Username"];
    string demoPass = configuration["DemoUserSeed:Password"];
    string demoTipo = configuration["DemoUserSeed:Tipo"];

    // Validar que los valores existan 
    if (string.IsNullOrEmpty(adminUser) || string.IsNullOrEmpty(adminPass) || string.IsNullOrEmpty(adminTipo) || string.IsNullOrEmpty(demoPass) || string.IsNullOrEmpty(demoTipo))
    {
        
        throw new InvalidOperationException("No se encontraron las configuraciones 'AdminUserSeed' en appsettings.json");
    }

    // Usa las variables para crear el usuario admin
    if (!context.Usuario.Any(u => u.user == adminUser))
    {
        context.Usuario.Add(new Usuario
        {
            user = adminUser,   
            Clave = adminPass,  
            Tipo = adminTipo    
        });

        context.SaveChanges();
    }

    // Usa las variables para crear el usuario demo
    if (!context.Usuario.Any(u => u.user == demoUser))
    {
        context.Usuario.Add(new Usuario
        {
            user = demoUser,
            Clave = demoPass,
            Tipo = demoTipo
        });

        context.SaveChanges();
    }
}

app.Run();

