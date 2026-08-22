const statusText = document.querySelector("#statusText");
const refreshButton = document.querySelector("#refreshButton");
const movimientosBody = document.querySelector("#movimientosBody");
const tabButtons = document.querySelectorAll("[data-view]");
const views = document.querySelectorAll(".view");
const clienteForm = document.querySelector("#clienteForm");
const clientesBody = document.querySelector("#clientesBody");
const clientesStatus = document.querySelector("#clientesStatus");
const clienteBuscar = document.querySelector("#clienteBuscar");
const clienteIncluirInactivos = document.querySelector("#clienteIncluirInactivos");

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
clienteForm.addEventListener("submit", crearCliente);
clienteBuscar.addEventListener("input", cargarClientes);
clienteIncluirInactivos.addEventListener("change", cargarClientes);

tabButtons.forEach((button) => {
  button.addEventListener("click", () => {
    cambiarVista(button.dataset.view);
  });
});

cargarDashboard();
cargarClientes();

function cambiarVista(viewId) {
  tabButtons.forEach((button) => {
    button.classList.toggle("active", button.dataset.view === viewId);
  });

  views.forEach((view) => {
    view.classList.toggle("active", view.id === viewId);
  });
}

async function cargarClientes() {
  clientesStatus.textContent = "Cargando clientes...";

  const params = new URLSearchParams();
  const buscar = clienteBuscar.value.trim();

  if (buscar) {
    params.set("buscar", buscar);
  }

  if (clienteIncluirInactivos.checked) {
    params.set("incluirInactivos", "true");
  }

  const queryString = params.toString();
  const url = queryString ? `/api/clientes?${queryString}` : "/api/clientes";

  try {
    const response = await fetch(url);

    if (!response.ok) {
      throw new Error(`Error HTTP ${response.status}`);
    }

    const clientes = await response.json();

    renderizarClientes(clientes);
    clientesStatus.textContent = `${clientes.length} cliente(s) encontrados`;
  } catch (error) {
    clientesStatus.textContent = "No se pudo cargar clientes";
    clientesBody.innerHTML = `
      <tr>
        <td colspan="6">${error.message}</td>
      </tr>
    `;
  }
}

async function crearCliente(event) {
  event.preventDefault();
  clientesStatus.textContent = "Guardando cliente...";

  const formData = new FormData(clienteForm);
  const idZona = formData.get("idZona");

  const cliente = {
    razonSocial: formData.get("razonSocial"),
    ruc: normalizarValorOpcional(formData.get("ruc")),
    telefono: normalizarValorOpcional(formData.get("telefono")),
    direccion: normalizarValorOpcional(formData.get("direccion")),
    idZona: idZona ? Number(idZona) : null,
    tipoCliente: formData.get("tipoCliente"),
    requiereGarantia: formData.get("requiereGarantia") === "on"
  };

  try {
    const response = await fetch("/api/clientes", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(cliente)
    });

    if (!response.ok) {
      const message = await response.text();
      throw new Error(message || `Error HTTP ${response.status}`);
    }

    clienteForm.reset();
    document.querySelector("#clienteRequiereGarantia").checked = true;
    clientesStatus.textContent = "Cliente creado";

    await cargarClientes();
    await cargarDashboard();
  } catch (error) {
    clientesStatus.textContent = error.message;
  }
}

function renderizarClientes(clientes) {
  if (!clientes.length) {
    clientesBody.innerHTML = `
      <tr>
        <td colspan="6">No hay clientes para mostrar.</td>
      </tr>
    `;
    return;
  }

  clientesBody.innerHTML = clientes
    .map((cliente) => `
      <tr>
        <td>${cliente.razonSocial}</td>
        <td>${cliente.ruc ?? "-"}</td>
        <td>${cliente.telefono ?? "-"}</td>
        <td>${cliente.tipoCliente}</td>
        <td>
          <span class="status-pill ${cliente.activo ? "" : "inactive"}">
            ${cliente.activo ? "Activo" : "Inactivo"}
          </span>
        </td>
        <td>
          <button class="table-action" type="button" data-cliente-id="${cliente.idCliente}" data-cliente-action="${cliente.activo ? "desactivar" : "reactivar"}">
            ${cliente.activo ? "Desactivar" : "Reactivar"}
          </button>
        </td>
      </tr>
    `)
    .join("");

  clientesBody.querySelectorAll("[data-cliente-id]").forEach((button) => {
    button.addEventListener("click", () => cambiarEstadoCliente(button));
  });
}

async function cambiarEstadoCliente(button) {
  const id = button.dataset.clienteId;
  const action = button.dataset.clienteAction;
  const method = action === "desactivar" ? "DELETE" : "PATCH";
  const url = action === "desactivar"
    ? `/api/clientes/${id}`
    : `/api/clientes/${id}/reactivar`;

  clientesStatus.textContent = action === "desactivar" ? "Desactivando cliente..." : "Reactivando cliente...";

  try {
    const response = await fetch(url, { method });

    if (!response.ok) {
      throw new Error(`Error HTTP ${response.status}`);
    }

    await cargarClientes();
    await cargarDashboard();
  } catch (error) {
    clientesStatus.textContent = error.message;
  }
}

function normalizarValorOpcional(valor) {
  const texto = String(valor ?? "").trim();

  return texto || null;
}
