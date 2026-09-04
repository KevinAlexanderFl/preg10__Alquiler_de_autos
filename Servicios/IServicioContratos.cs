using SistemaAlquilerAutos.Models;

namespace SistemaAlquilerAutos.Servicios;

public interface IServicioContratos
{
    Task<ResultadoOperacion> CrearAsync(Contrato contrato);
    Task<ResultadoOperacion> ActualizarAsync(Contrato contrato);
    Task<ResultadoOperacion> FinalizarAsync(int id);
    Task<ResultadoOperacion> CancelarAsync(int id);
    Task<ResultadoOperacion> EliminarAsync(int id);
    decimal CalcularTotal(DateTime inicio, DateTime fin, decimal tarifaDiaria);
}
