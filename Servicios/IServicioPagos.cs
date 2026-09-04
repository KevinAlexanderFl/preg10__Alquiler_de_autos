using SistemaAlquilerAutos.Models;

namespace SistemaAlquilerAutos.Servicios;

public interface IServicioPagos
{
    Task<ResultadoOperacion> CrearAsync(Pago pago);
    Task<ResultadoOperacion> ActualizarAsync(Pago pago);
    Task<ResultadoOperacion> EliminarAsync(int id);
}
