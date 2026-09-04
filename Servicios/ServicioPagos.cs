using Microsoft.EntityFrameworkCore;
using SistemaAlquilerAutos.Datos;
using SistemaAlquilerAutos.Models;

namespace SistemaAlquilerAutos.Servicios;

public class ServicioPagos : IServicioPagos
{
    private readonly ContextoAlquiler _contexto;

    public ServicioPagos(ContextoAlquiler contexto)
    {
        _contexto = contexto;
    }

    public async Task<ResultadoOperacion> CrearAsync(Pago pago)
    {
        var validacion = await ValidarAsync(pago);
        if (!validacion.Exitoso) return validacion;

        _contexto.Pagos.Add(pago);
        await _contexto.SaveChangesAsync();
        return ResultadoOperacion.Correcto("Pago registrado correctamente.");
    }

    public async Task<ResultadoOperacion> ActualizarAsync(Pago pago)
    {
        if (!await _contexto.Pagos.AnyAsync(x => x.Id == pago.Id))
            return ResultadoOperacion.Error("El pago no existe.");

        var validacion = await ValidarAsync(pago);
        if (!validacion.Exitoso) return validacion;

        _contexto.Pagos.Update(pago);
        await _contexto.SaveChangesAsync();
        return ResultadoOperacion.Correcto("Pago actualizado correctamente.");
    }

    public async Task<ResultadoOperacion> EliminarAsync(int id)
    {
        var pago = await _contexto.Pagos.FindAsync(id);
        if (pago is null) return ResultadoOperacion.Error("El pago no existe.");
        _contexto.Pagos.Remove(pago);
        await _contexto.SaveChangesAsync();
        return ResultadoOperacion.Correcto("Pago eliminado correctamente.");
    }

    private async Task<ResultadoOperacion> ValidarAsync(Pago pago)
    {
        var contrato = await _contexto.Contratos
            .AsNoTracking()
            .Include(x => x.Pagos)
            .FirstOrDefaultAsync(x => x.Id == pago.ContratoId);

        if (contrato is null) return ResultadoOperacion.Error("Seleccione un contrato válido.");
        if (contrato.Estado == EstadoContrato.Cancelado)
            return ResultadoOperacion.Error("No se puede registrar pagos en un contrato cancelado.");

        if (pago.Estado == EstadoPago.Confirmado)
        {
            var pagadoSinActual = contrato.Pagos
                .Where(x => x.Id != pago.Id && x.Estado == EstadoPago.Confirmado)
                .Sum(x => x.Monto);
            if (pagadoSinActual + pago.Monto > contrato.Total)
                return ResultadoOperacion.Error($"El pago supera el saldo disponible de ${contrato.Total - pagadoSinActual:N2}.");
        }

        if (pago.Metodo != MetodoPago.Efectivo && string.IsNullOrWhiteSpace(pago.NumeroReferencia))
            return ResultadoOperacion.Error("Ingrese el número de referencia para pagos electrónicos.");

        return ResultadoOperacion.Correcto("Datos válidos.");
    }
}
