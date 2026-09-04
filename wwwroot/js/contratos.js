const selectorVehiculo = document.getElementById('selectorVehiculo');
const inicio = document.getElementById('fechaInicio');
const fin = document.getElementById('fechaFin');
const tarifa = document.getElementById('tarifaDiaria');
const total = document.getElementById('totalEstimado');
const textoDias = document.getElementById('diasAlquiler');

function calcularTotal() {
    if (!inicio?.value || !fin?.value || !tarifa?.value) return;
    const fechaInicio = new Date(`${inicio.value}T00:00:00`);
    const fechaFin = new Date(`${fin.value}T00:00:00`);
    const diferencia = Math.round((fechaFin - fechaInicio) / 86400000);
    const dias = Math.max(1, diferencia);
    const monto = dias * Number(tarifa.value);
    total.textContent = monto.toLocaleString('es-EC', { style: 'currency', currency: 'USD' });
    textoDias.textContent = `${dias} día${dias === 1 ? '' : 's'} × $${Number(tarifa.value).toFixed(2)}`;
}

selectorVehiculo?.addEventListener('change', async () => {
    if (!selectorVehiculo.value) return;
    try {
        const respuesta = await fetch(`/Contratos/TarifaVehiculo/${selectorVehiculo.value}`);
        if (respuesta.ok) {
            const datos = await respuesta.json();
            tarifa.value = datos.tarifa;
            calcularTotal();
        }
    } catch { /* La tarifa aún puede ingresarse manualmente. */ }
});

[inicio, fin, tarifa].forEach(campo => campo?.addEventListener('input', calcularTotal));
calcularTotal();
