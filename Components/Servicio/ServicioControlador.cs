using xx.Components.Data;

namespace xx.Components.Servicio
{
    public class ServicioControlador
    {
        private readonly ServicioJuegos _servicioJuegos;

        // El estado del filtro se almacena aquí en memoria
        private bool _mostrarSoloPendientes = false;

        public ServicioControlador(ServicioJuegos servicioJuegos)
        {
            _servicioJuegos = servicioJuegos;
        }

        // Propiedad pública que lee/escribe el estado en memoria
        public bool MostrarSoloPendientes
        {
            get => _mostrarSoloPendientes;
            set => _mostrarSoloPendientes = value;
        }

        // Nuevos métodos para gestionar la persistencia
        public async Task CargarEstadoFiltro()
        {
            _mostrarSoloPendientes = await _servicioJuegos.ObtenerEstadoFiltro();
        }

        public async Task GuardarEstadoFiltro()
        {
            await _servicioJuegos.GuardarEstadoFiltro(_mostrarSoloPendientes);
        }

        // Métodos de CRUD (se mantienen igual)
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

        private async Task<int> GenerarNuevoID()
        {
            var juegos = await _servicioJuegos.ObtenerJuegos();
            return juegos.Any() ? juegos.Max(j => j.Identificador) + 1 : 1;
        }

        public async Task EliminarJuego(String juego)
        {
            await _servicioJuegos.EliminarJuego(juego);
        }


    }
}