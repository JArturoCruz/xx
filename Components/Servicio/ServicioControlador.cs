using xx.Components.Data;

namespace xx.Components.Servicio
{
    public class ServicioControlador
    {
        private readonly ServicioJuegos _servicioJuegos;

        private bool _mostrarSoloPendientes = false;
        private string _filtroNombre = string.Empty;

        public ServicioControlador(ServicioJuegos servicioJuegos)
        {
            _servicioJuegos = servicioJuegos;
        }

        public bool MostrarSoloPendientes
        {
            get => _mostrarSoloPendientes;
            set => _mostrarSoloPendientes = value;
        }

        public string FiltroNombre
        {
            get => _filtroNombre;
            set => _filtroNombre = value;
        }

        public async Task CargarEstadoFiltro()
        {
            _mostrarSoloPendientes = await _servicioJuegos.ObtenerEstadoFiltro();
        }

        public async Task GuardarEstadoFiltro()
        {
            await _servicioJuegos.GuardarEstadoFiltro(_mostrarSoloPendientes);
        }

        public async Task CargarFiltroNombre()
        {
            _filtroNombre = await _servicioJuegos.ObtenerFiltroNombre();
        }

        public async Task GuardarFiltroNombre()
        {
            await _servicioJuegos.GuardarFiltroNombre(_filtroNombre);
        }

        public async Task<List<Juego>> ObtenerJuegos()
        {
            return await _servicioJuegos.ObtenerJuegos();
        }

        public async Task AgregarJuego(Juego juego)
        {
            juego.Identificador = await GenerarNuevoID();
            await _servicioJuegos.AgregarJuego(juego);
        }

        public async Task ActualizarJuego(Juego juego)
        {
            await _servicioJuegos.ActualizarJuego(juego);
        }

        public async Task ActualizarNombreJuego(int identificador, string nuevoNombre)
        {
            await _servicioJuegos.ActualizarNombreJuego(identificador, nuevoNombre);
        }

        private async Task<int> GenerarNuevoID()
        {
            var juegos = await _servicioJuegos.ObtenerJuegos();
            return juegos.Any() ? juegos.Max(j => j.Identificador) + 1 : 1;
        }

        public async Task EliminarJuego(int identificador)
        {
            await _servicioJuegos.EliminarJuego(identificador);
        }
    }
}