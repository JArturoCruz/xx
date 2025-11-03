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

        public async Task ActualizarNombreJuego(int identificador, string nuevoNombre)
        {
            String ruta = "mibase.db";
            using var conexion = new SqliteConnection($"Datasource={ruta}");
            await conexion.OpenAsync();

            var comando = conexion.CreateCommand();
            comando.CommandText = "UPDATE juego SET nombre = @NOMBRE WHERE identificador = @IDENTIFICADOR";
            comando.Parameters.AddWithValue("@NOMBRE", nuevoNombre);
            comando.Parameters.AddWithValue("@IDENTIFICADOR", identificador);

            await comando.ExecuteNonQueryAsync();

            var juegoExistente = juegos.FirstOrDefault(j => j.Identificador == identificador);
            if (juegoExistente != null)
            {
                juegoExistente.Nombre = nuevoNombre;
            }
        }

        public async Task EliminarJuego(int identificador)
        {
            String ruta = "mibase.db";
            using var conexion = new SqliteConnection($"Datasource={ruta}");
            await conexion.OpenAsync();

            var comando = conexion.CreateCommand();
            comando.CommandText = "DELETE FROM juego WHERE identificador = @IDENTIFICADOR";
            comando.Parameters.AddWithValue("@IDENTIFICADOR", identificador);

            await comando.ExecuteNonQueryAsync();

            juegos.RemoveAll(j => j.Identificador == identificador);
        }


        public async Task<bool> ObtenerEstadoFiltro()
        {
            String ruta = "mibase.db";
            using var conexion = new SqliteConnection($"Datasource={ruta}");
            await conexion.OpenAsync();

            var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT valor FROM configuracion WHERE clave = 'MostrarSoloPendientes'";

            var resultado = await comando.ExecuteScalarAsync();

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

            comando.CommandText = @"
                INSERT OR REPLACE INTO configuracion (clave, valor)
                VALUES ('MostrarSoloPendientes', @VALOR)
            ";
            comando.Parameters.AddWithValue("@VALOR", estado.ToString());

            await comando.ExecuteNonQueryAsync();
        }

        public async Task<string> ObtenerFiltroNombre()
        {
            String ruta = "mibase.db";
            using var conexion = new SqliteConnection($"Datasource={ruta}");
            await conexion.OpenAsync();

            var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT valor FROM configuracion WHERE clave = 'FiltroNombre'";

            var resultado = await comando.ExecuteScalarAsync();

            return resultado?.ToString() ?? string.Empty;
        }

        public async Task GuardarFiltroNombre(string nombre)
        {
            String ruta = "mibase.db";
            using var conexion = new SqliteConnection($"Datasource={ruta}");
            await conexion.OpenAsync();

            var comando = conexion.CreateCommand();

            comando.CommandText = @"
                INSERT OR REPLACE INTO configuracion (clave, valor)
                VALUES ('FiltroNombre', @VALOR)
            ";
            comando.Parameters.AddWithValue("@VALOR", nombre ?? string.Empty);

            await comando.ExecuteNonQueryAsync();
        }
    }
}