const metodo = document.getElementById('metodoPago');
const referencia = document.getElementById('campoReferencia');

function actualizarReferencia() {
    if (!metodo || !referencia) return;
    referencia.style.opacity = metodo.value === '0' ? '.55' : '1';
}

metodo?.addEventListener('change', actualizarReferencia);
actualizarReferencia();
