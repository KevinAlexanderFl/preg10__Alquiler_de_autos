const barra = document.getElementById('barraLateral');
const fondo = document.getElementById('fondoMenu');
const boton = document.getElementById('botonMenu');

function alternarMenu() {
    barra?.classList.toggle('abierta');
    fondo?.classList.toggle('visible');
}

boton?.addEventListener('click', alternarMenu);
fondo?.addEventListener('click', alternarMenu);

document.querySelectorAll('.alerta button').forEach(botonAlerta => {
    botonAlerta.addEventListener('click', () => botonAlerta.parentElement.remove());
});

document.querySelectorAll('[data-confirmar]').forEach(elemento => {
    elemento.addEventListener('click', evento => {
        if (!window.confirm(elemento.dataset.confirmar)) evento.preventDefault();
    });
});
