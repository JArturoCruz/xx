using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace xx.Components.Data
{
    public class ServicioJuegos
    {
        private List<Juego> juegos = new List<Juego>();

        public async Task<List<Juego>> ObtenerJuegos()
        {
            // Lógica para cargar desde la DB solo si la lista en memoria está vacía
            if (!juegos.Any())
            {
                String ruta = "mibase.db";
                using var conexion = new SqliteConnection($"Datasource={ruta}");
                await conexion.OpenAsync();

                var comando = conexion.CreateCommand();
                comando.CommandText = "SELECT identificador, nombre, jugado FROM juego";
                using var lector = await comando.ExecuteReaderAsync();

                while (await lector.ReadAsync())
                {
                    juegos.Add(new Juego
                    {
                        Identificador = lector.GetInt32(0),
                        Nombre = lector.GetString(1),
                        Jugado = lector.GetInt32(2) == 0 ? false : true
                    });
                }
            }
            return juegos;
        }

        public async Task AgregarJuego(Juego juego)
        {
            String ruta = "mibase.db";
            using var conexion = new SqliteConnection($"Datasource={ruta}");
            await conexion.OpenAsync();

            var comando = conexion.CreateCommand();
            comando.CommandText = "INSERT INTO juego (identificador, nombre, jugado) VALUES(@IDENTIFICADOR, @NOMBRE, @JUGADO)";
            comando.Parameters.AddWithValue("@IDENTIFICADOR", juego.Identificador);
            comando.Parameters.AddWithValue("@NOMBRE", juego.Nombre);
            comando.Parameters.AddWithValue("@JUGADO", juego.Jugado ? 1 : 0);

            await comando.ExecuteNonQueryAsync();

            juegos.Add(juego);
        }

        public async Task ActualizarJuego(Juego juego)
        {
            String ruta = "mibase.db";
            using var conexion = new SqliteConnection($"Datasource={ruta}");
            await conexion.OpenAsync();

            var comando = conexion.CreateCommand();
            comando.CommandText = "UPDATE juego SET jugado = @JUGADO WHERE identificador = @IDENTIFICADOR";
            comando.Parameters.AddWithValue("@JUGADO", juego.Jugado ? 1 : 0);
            comando.Parameters.AddWithValue("@IDENTIFICADOR", juego.Identificador);

            await comando.ExecuteNonQueryAsync();

            var juegoExistente = juegos.FirstOrDefault(j => j.Identificador == juego.Identificador);
            if (juegoExistente != null)
            {
                juegoExistente.Jugado = juego.Jugado;
            }
        }

        public async Task EliminarJuego(string nombre)
        {
            String ruta = "mibase.db";
            using var conexion = new SqliteConnection($"Datasource={ruta}");
            await conexion.OpenAsync();

            var comando = conexion.CreateCommand();
            comando.CommandText = "DELETE FROM juego WHERE nombre = @NOMBRE";
            comando.Parameters.AddWithValue("@NOMBRE", nombre);

            await comando.ExecuteNonQueryAsync();

            juegos.RemoveAll(j => j.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }

        // MÉTODOS DE PERSISTENCIA DEL ESTADO DEL FILTRO

        public async Task<bool> ObtenerEstadoFiltro()
        {
            String ruta = "mibase.db";
            using var conexion = new SqliteConnection($"Datasource={ruta}");
            await conexion.OpenAsync();

            var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT valor FROM configuracion WHERE clave = 'MostrarSoloPendientes'";

            var resultado = await comando.ExecuteScalarAsync();

            // Si encuentra el valor, lo parsea a booleano, si no, devuelve false (por defecto)
            if (resultado != null && resultado != DBNull.Value && bool.TryParse(resultado.ToString(), out bool estado))
            {
                return estado;
            }

            return false;
        }

        public async Task GuardarEstadoFiltro(bool estado)
        {
            String ruta = "mibase.db";
            using var conexion = new SqliteConnection($"Datasource={ruta}");
            await conexion.OpenAsync();

            var comando = conexion.CreateCommand();

            // INSERT OR REPLACE asegura que el valor se actualice si ya existe
            comando.CommandText = @"
                INSERT OR REPLACE INTO configuracion (clave, valor)
                VALUES ('MostrarSoloPendientes', @VALOR)
            ";
            comando.Parameters.AddWithValue("@VALOR", estado.ToString());

            await comando.ExecuteNonQueryAsync();
        }
    }
}