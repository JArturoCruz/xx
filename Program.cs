using Microsoft.Data.Sqlite;
using xx.Components;
using xx.Components.Data;
using xx.Components.Servicio;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ServicioControlador>();
builder.Services.AddSingleton<ServicioJuegos>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

String ruta = "mibase.db";

using var conexion = new SqliteConnection($"DataSource ={ruta}");
conexion.Open();
var comando = conexion.CreateCommand();

comando.CommandText = @"
    CREATE TABLE IF NOT EXISTS
    juego( identificador integer, nombre text, jugado integer);

    -- Nueva tabla para guardar configuraciones persistentes
    CREATE TABLE IF NOT EXISTS
    configuracion( clave TEXT PRIMARY KEY, valor TEXT);

    -- Insertar el estado inicial del filtro ('False') si no existe
    INSERT OR IGNORE INTO configuracion (clave, valor) VALUES ('MostrarSoloPendientes', 'False');

    -- --- LÍNEA NUEVA AÑADIDA ---
    -- Insertar el estado inicial del filtro de nombre (vacío) si no existe
    INSERT OR IGNORE INTO configuracion (clave, valor) VALUES ('FiltroNombre', '');
";
comando.ExecuteNonQuery();
conexion.Close(); 
app.Run();