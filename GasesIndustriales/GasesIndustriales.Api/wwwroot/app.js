const statusText = document.querySelector("#statusText");
const refreshButton = document.querySelector("#refreshButton");
const movimientosBody = document.querySelector("#movimientosBody");

const summaryElements = {
  cilindrosDisponibles: document.querySelector("#cilindrosDisponibles"),
  cilindrosEnClientes: document.querySelector("#cilindrosEnClientes"),
  cilindrosEnProveedor: document.querySelector("#cilindrosEnProveedor"),
  pedidosPendientes: document.querySelector("#pedidosPendientes")
};

async function cargarDashboard() {
  statusText.textContent = "Cargando datos...";

  try {
    const response = await fetch("/api/dashboard/resumen");

    if (!response.ok) {
      throw new Error(`Error HTTP ${response.status}`);
    }

    const data = await response.json();

    summaryElements.cilindrosDisponibles.textContent = data.cilindrosDisponibles;
    summaryElements.cilindrosEnClientes.textContent = data.cilindrosEnClientes;
    summaryElements.cilindrosEnProveedor.textContent = data.cilindrosEnProveedor;
    summaryElements.pedidosPendientes.textContent = data.pedidosPendientes;

    renderizarMovimientos(data.movimientosRecientes);

    statusText.textContent = "Datos actualizados";
  } catch (error) {
    statusText.textContent = "No se pudo cargar el dashboard";
    movimientosBody.innerHTML = `
      <tr>
        <td colspan="6">${error.message}</td>
      </tr>
    `;
  }
}

function renderizarMovimientos(movimientos) {
  if (!movimientos.length) {
    movimientosBody.innerHTML = `
      <tr>
        <td colspan="6">No hay movimientos registrados.</td>
      </tr>
    `;
    return;
  }

  movimientosBody.innerHTML = movimientos
    .map((movimiento) => `
      <tr>
        <td>${formatearFecha(movimiento.fechaMovimiento)}</td>
        <td>${movimiento.codigoCilindro}</td>
        <td>${movimiento.producto}</td>
        <td>${movimiento.tipoMovimiento}</td>
        <td>${movimiento.cliente ?? "Sin cliente"}</td>
        <td>${movimiento.observacion ?? ""}</td>
      </tr>
    `)
    .join("");
}

function formatearFecha(fecha) {
  return new Intl.DateTimeFormat("es-PE", {
    dateStyle: "short",
    timeStyle: "short"
  }).format(new Date(fecha));
}

refreshButton.addEventListener("click", cargarDashboard);

cargarDashboard();
