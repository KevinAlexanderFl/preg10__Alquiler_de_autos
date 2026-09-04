using Microsoft.EntityFrameworkCore;
using SistemaAlquilerAutos.Datos;
using SistemaAlquilerAutos.Models;

namespace SistemaAlquilerAutos.Servicios;

public class ServicioContratos : IServicioContratos
{
    private readonly ContextoAlquiler _contexto;

    public ServicioContratos(ContextoAlquiler contexto)
    {
        _contexto = contexto;
    }

    public decimal CalcularTotal(DateTime inicio, DateTime fin, decimal tarifaDiaria)
    {
        var dias = Math.Max(1, (fin.Date - inicio.Date).Days);
        return decimal.Round(dias * tarifaDiaria, 2);
    }

    public async Task<ResultadoOperacion> CrearAsync(Contrato contrato)
    {
        var validacion = await ValidarAsync(contrato);
        if (!validacion.Exitoso) return validacion;

        contrato.Total = CalcularTotal(contrato.FechaInicio, contrato.FechaFin, contrato.TarifaDiaria);
        contrato.Estado = contrato.FechaInicio.Date <= DateTime.Today
            ? EstadoContrato.Activo
            : EstadoContrato.Reservado;

        _contexto.Contratos.Add(contrato);
        await _contexto.SaveChangesAsync();
        await ActualizarEstadoVehiculoAsync(contrato.VehiculoId);
        return ResultadoOperacion.Correcto("Contrato registrado correctamente.");
    }

    public async Task<ResultadoOperacion> ActualizarAsync(Contrato contrato)
    {
        var existente = await _contexto.Contratos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == contrato.Id);
        if (existente is null) return ResultadoOperacion.Error("El contrato no existe.");
        if (existente.Estado is EstadoContrato.Finalizado or EstadoContrato.Cancelado)
            return ResultadoOperacion.Error("No se puede modificar un contrato finalizado o cancelado.");

        var validacion = await ValidarAsync(contrato);
        if (!validacion.Exitoso) return validacion;

        contrato.Total = CalcularTotal(contrato.FechaInicio, contrato.FechaFin, contrato.TarifaDiaria);
        var totalPagado = await _contexto.Pagos
            .Where(x => x.ContratoId == contrato.Id && x.Estado == EstadoPago.Confirmado)
            .SumAsync(x => (decimal?)x.Monto) ?? 0;
        if (contrato.Total < totalPagado)
            return ResultadoOperacion.Error($"El total no puede ser menor a los ${totalPagado:N2} ya pagados.");
        contrato.Estado = contrato.FechaInicio.Date <= DateTime.Today
            ? EstadoContrato.Activo
            : EstadoContrato.Reservado;

        _contexto.Contratos.Update(contrato);
        await _contexto.SaveChangesAsync();
        await ActualizarEstadoVehiculoAsync(existente.VehiculoId);
        if (existente.VehiculoId != contrato.VehiculoId)
            await ActualizarEstadoVehiculoAsync(contrato.VehiculoId);

        return ResultadoOperacion.Correcto("Contrato actualizado correctamente.");
    }

    public async Task<ResultadoOperacion> FinalizarAsync(int id)
    {
        var contrato = await _contexto.Contratos.Include(x => x.Pagos).FirstOrDefaultAsync(x => x.Id == id);
        if (contrato is null) return ResultadoOperacion.Error("El contrato no existe.");
        if (contrato.Estado == EstadoContrato.Cancelado)
            return ResultadoOperacion.Error("Un contrato cancelado no se puede finalizar.");
        if (contrato.SaldoPendiente > 0)
            return ResultadoOperacion.Error($"No se puede finalizar: queda un saldo de ${contrato.SaldoPendiente:N2}.");

        contrato.Estado = EstadoContrato.Finalizado;
        await _contexto.SaveChangesAsync();
        await ActualizarEstadoVehiculoAsync(contrato.VehiculoId);
        return ResultadoOperacion.Correcto("Contrato finalizado y vehículo liberado.");
    }

    public async Task<ResultadoOperacion> CancelarAsync(int id)
    {
        var contrato = await _contexto.Contratos.Include(x => x.Pagos).FirstOrDefaultAsync(x => x.Id == id);
        if (contrato is null) return ResultadoOperacion.Error("El contrato no existe.");
        if (contrato.Estado == EstadoContrato.Finalizado)
            return ResultadoOperacion.Error("Un contrato finalizado no se puede cancelar.");
        if (contrato.Pagos.Any(x => x.Estado == EstadoPago.Confirmado))
            return ResultadoOperacion.Error("Anule los pagos confirmados antes de cancelar el contrato.");

        contrato.Estado = EstadoContrato.Cancelado;
        await _contexto.SaveChangesAsync();
        await ActualizarEstadoVehiculoAsync(contrato.VehiculoId);
        return ResultadoOperacion.Correcto("Contrato cancelado correctamente.");
    }

    public async Task<ResultadoOperacion> EliminarAsync(int id)
    {
        var contrato = await _contexto.Contratos.Include(x => x.Pagos).FirstOrDefaultAsync(x => x.Id == id);
        if (contrato is null) return ResultadoOperacion.Error("El contrato no existe.");
        if (contrato.Pagos.Any())
            return ResultadoOperacion.Error("No se puede eliminar un contrato que tiene pagos; puede cancelarlo.");

        var vehiculoId = contrato.VehiculoId;
        _contexto.Contratos.Remove(contrato);
        await _contexto.SaveChangesAsync();
        await ActualizarEstadoVehiculoAsync(vehiculoId);
        return ResultadoOperacion.Correcto("Contrato eliminado correctamente.");
    }

    private async Task<ResultadoOperacion> ValidarAsync(Contrato contrato)
    {
        if (contrato.FechaFin.Date <= contrato.FechaInicio.Date)
            return ResultadoOperacion.Error("La fecha de devolución debe ser posterior a la fecha de inicio.");

        var clienteActivo = await _contexto.Clientes.AnyAsync(x => x.Id == contrato.ClienteId && x.Activo);
        if (!clienteActivo) return ResultadoOperacion.Error("Seleccione un cliente activo.");

        var vehiculo = await _contexto.Vehiculos.FirstOrDefaultAsync(x => x.Id == contrato.VehiculoId && x.Activo);
        if (vehiculo is null) return ResultadoOperacion.Error("Seleccione un vehículo activo.");
        if (vehiculo.Estado == EstadoVehiculo.Mantenimiento)
            return ResultadoOperacion.Error("El vehículo está en mantenimiento.");

        var existeCruce = await _contexto.Contratos.AnyAsync(x =>
            x.Id != contrato.Id &&
            x.VehiculoId == contrato.VehiculoId &&
            x.Estado != EstadoContrato.Cancelado &&
            x.Estado != EstadoContrato.Finalizado &&
            contrato.FechaInicio < x.FechaFin &&
            contrato.FechaFin > x.FechaInicio);

        return existeCruce
            ? ResultadoOperacion.Error("El vehículo ya tiene un contrato en ese rango de fechas.")
            : ResultadoOperacion.Correcto("Datos válidos.");
    }

    private async Task ActualizarEstadoVehiculoAsync(int vehiculoId)
    {
        var vehiculo = await _contexto.Vehiculos.FindAsync(vehiculoId);
        if (vehiculo is null || vehiculo.Estado == EstadoVehiculo.Mantenimiento) return;

        var hoy = DateTime.Today;
        var contratoActual = await _contexto.Contratos
            .Where(x => x.VehiculoId == vehiculoId &&
                        x.Estado != EstadoContrato.Cancelado &&
                        x.Estado != EstadoContrato.Finalizado &&
                        x.FechaInicio <= hoy && x.FechaFin >= hoy)
            .FirstOrDefaultAsync();

        if (contratoActual is not null)
        {
            contratoActual.Estado = EstadoContrato.Activo;
            vehiculo.Estado = EstadoVehiculo.Alquilado;
        }
        else
        {
            var tieneReserva = await _contexto.Contratos.AnyAsync(x =>
                x.VehiculoId == vehiculoId && x.Estado == EstadoContrato.Reservado && x.FechaInicio > hoy);
            vehiculo.Estado = tieneReserva ? EstadoVehiculo.Reservado : EstadoVehiculo.Disponible;
        }

        await _contexto.SaveChangesAsync();
    }
}
