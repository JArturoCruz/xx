using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xx.Components.Data
{
    public class ServicioJuegos
    {
        private List<Juego> juegos = new List<Juego>
        {
            new Juego { Identificador = 1, Nombre = "Ravel", Jugado = false },
            new Juego { Identificador = 2, Nombre = "Carcassone", Jugado = true },
        };

        public bool MostrarSoloPendientes { get; set; } = false;
        public Task<List<Juego>> ObtenerJuegos() => Task.FromResult(juegos);

        public Task AgregarJuego(Juego juego)
        {
            juegos.Add(juego);
            return Task.CompletedTask;
        }

        public Task ActualizarJuego(Juego juego)
        {
            var juegoExistente = juegos.FirstOrDefault(j => j.Identificador == juego.Identificador);
            if (juegoExistente != null)
            {
                juegoExistente.Jugado = juego.Jugado;
            }
            return Task.CompletedTask;
        }

        public Task EliminarJuego(String juego)
        {
            juegos.RemoveAll(j => j.Nombre.Equals(juego, StringComparison.OrdinalIgnoreCase));

            return Task.CompletedTask;
        }

    
    }
}