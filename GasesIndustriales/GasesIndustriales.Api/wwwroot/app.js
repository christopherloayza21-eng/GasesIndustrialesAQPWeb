const statusText = document.querySelector("#statusText");
const refreshButton = document.querySelector("#refreshButton");
const movimientosBody = document.querySelector("#movimientosBody");
const tabButtons = document.querySelectorAll("[data-view]");
const views = document.querySelectorAll(".view");

const summaryElements = {
  cilindrosDisponibles: document.querySelector("#cilindrosDisponibles"),
  cilindrosEnClientes: document.querySelector("#cilindrosEnClientes"),
  cilindrosEnProveedor: document.querySelector("#cilindrosEnProveedor"),
  pedidosPendientes: document.querySelector("#pedidosPendientes")
};

const modules = {
  clientes: {
    title: "Clientes",
    endpoint: "/api/clientes",
    key: "idCliente",
    search: true,
    fields: [
      ["razonSocial", "Razón social", "text", true],
      ["ruc", "RUC", "text"],
      ["telefono", "Teléfono", "text"],
      ["direccion", "Dirección", "text"],
      ["idZona", "ID zona", "number"],
      ["tipoCliente", "Tipo", "select", true, ["EVENTUAL", "NUEVO", "FRECUENTE"]],
      ["requiereGarantia", "Requiere garantía", "checkbox"]
    ],
    columns: ["idCliente", "razonSocial", "ruc", "telefono", "tipoCliente", "activo"]
  },
  productos: {
    title: "Productos",
    endpoint: "/api/productos",
    key: "idProducto",
    fields: [
      ["codigo", "Código", "text", true],
      ["nombre", "Nombre", "text", true],
      ["tipoProducto", "Tipo", "select", true, ["GAS", "EQUIPO", "INSUMO", "SERVICIO"]],
      ["unidadMedida", "Unidad", "text", true],
      ["precioReferencia", "Precio referencia", "number"]
    ],
    columns: ["idProducto", "codigo", "nombre", "tipoProducto", "unidadMedida", "precioReferencia", "activo"]
  },
  cilindros: {
    title: "Cilindros",
    endpoint: "/api/cilindros",
    key: "idCilindro",
    fields: [
      ["codigoCilindro", "Código cilindro", "text", true],
      ["idProducto", "ID producto gas", "number", true],
      ["capacidad", "Capacidad", "number"],
      ["propietarioTipo", "Propietario", "select", true, ["EMPRESA", "CLIENTE"]],
      ["idClientePropietario", "ID cliente propietario", "number"],
      ["estadoActual", "Estado", "select", true, ["LLENO_ALMACEN", "VACIO_ALMACEN", "EN_CLIENTE", "EN_PROVEEDOR"]],
      ["ubicacionActual", "Ubicación", "text"]
    ],
    columns: ["idCilindro", "codigoCilindro", "producto", "estadoActual", "ubicacionActual", "activo"],
    extraAction: { label: "Historial", action: cargarHistorialCilindro }
  },
  pedidos: {
    title: "Pedidos",
    endpoint: "/api/pedidos",
    key: "idPedido",
    complex: true,
    sample: {
      idCliente: 1,
      direccionEntrega: "Dirección de entrega",
      idZona: null,
      idConductor: null,
      idVehiculo: null,
      observaciones: "Pedido demo",
      detalles: [{ idProducto: 1, cantidad: 1, precioUnitario: 14.3 }]
    },
    columns: ["idPedido", "cliente", "estadoPedido", "total", "fechaPedido"],
    extraAction: { label: "Cancelar", action: cancelarPedido }
  },
  movimientos: {
    title: "Movimientos de cilindros",
    endpoint: "/api/movimientos",
    readonly: true,
    columns: ["idMovimiento", "codigoCilindro", "tipoMovimiento", "cliente", "fechaMovimiento", "observacion"],
    actions: [
      ["Salida cliente", "/api/movimientos/salida-cliente"],
      ["Retorno cliente", "/api/movimientos/retorno-cliente"],
      ["Envío proveedor", "/api/movimientos/envio-proveedor"],
      ["Retorno recarga", "/api/movimientos/retorno-recarga"]
    ],
    sample: { idCilindro: 1, idCliente: 1, idPedido: null, idConductor: null, idVehiculo: null, observacion: "Movimiento demo" }
  },
  recargas: {
    title: "Recargas",
    endpoint: "/api/recargas/envios",
    readonly: true,
    columns: ["idEnvio", "proveedor", "estado", "pendientes", "numeroGuia", "fechaEnvio"],
    actions: [["Crear envío", "/api/recargas/envios"]],
    sample: { idProveedor: 1, numeroGuia: "GUIA-DEMO", observaciones: "Envío demo", cilindroIds: [2] }
  },
  zonas: {
    title: "Zonas",
    endpoint: "/api/zonas",
    key: "idZona",
    fields: [["nombre", "Nombre", "text", true], ["descripcion", "Descripción", "text"]],
    columns: ["idZona", "nombre", "descripcion", "activo"]
  },
  conductores: {
    title: "Conductores",
    endpoint: "/api/conductores",
    key: "idConductor",
    fields: [["nombre", "Nombre", "text", true], ["telefono", "Teléfono", "text"]],
    columns: ["idConductor", "nombre", "telefono", "activo"]
  },
  vehiculos: {
    title: "Vehículos",
    endpoint: "/api/vehiculos",
    key: "idVehiculo",
    fields: [["placa", "Placa", "text", true], ["descripcion", "Descripción", "text"]],
    columns: ["idVehiculo", "placa", "descripcion", "activo"]
  },
  proveedores: {
    title: "Proveedores",
    endpoint: "/api/proveedores",
    key: "idProveedor",
    fields: [["razonSocial", "Razón social", "text", true], ["ruc", "RUC", "text"], ["telefono", "Teléfono", "text"], ["direccion", "Dirección", "text"]],
    columns: ["idProveedor", "razonSocial", "ruc", "telefono", "activo"]
  }
};

refreshButton.addEventListener("click", cargarTodo);

tabButtons.forEach((button) => {
  button.addEventListener("click", () => cambiarVista(button.dataset.view));
});

document.querySelectorAll("[data-module]").forEach((container) => {
  const config = modules[container.dataset.module];
  config.moduleKey = container.dataset.module;
  construirModulo(container, config);
});

cargarTodo();

function cambiarVista(viewId) {
  tabButtons.forEach((button) => button.classList.toggle("active", button.dataset.view === viewId));
  views.forEach((view) => view.classList.toggle("active", view.id === viewId));
}

async function cargarTodo() {
  await cargarDashboard();
  await Promise.all(Object.keys(modules).map((moduleKey) => cargarModulo(moduleKey)));
}

async function cargarDashboard() {
  statusText.textContent = "Cargando datos...";

  try {
    const data = await apiJson("/api/dashboard/resumen");

    summaryElements.cilindrosDisponibles.textContent = data.cilindrosDisponibles;
    summaryElements.cilindrosEnClientes.textContent = data.cilindrosEnClientes;
    summaryElements.cilindrosEnProveedor.textContent = data.cilindrosEnProveedor;
    summaryElements.pedidosPendientes.textContent = data.pedidosPendientes;
    renderizarMovimientosDashboard(data.movimientosRecientes);
    statusText.textContent = "Datos actualizados";
  } catch (error) {
    statusText.textContent = error.message;
  }
}

function construirModulo(container, config) {
  const formHtml = config.readonly ? construirAcciones(config) : construirFormulario(config);

  container.innerHTML = `
    <section class="panel form-panel">
      <div class="panel-header">
        <div><p class="eyebrow">${config.title}</p><h2>${config.readonly ? "Operaciones" : "Registrar"}</h2></div>
        <p id="status-${config.moduleKey}">Listo</p>
      </div>
      ${formHtml}
    </section>
    <section class="panel">
      <div class="panel-header">
        <div><p class="eyebrow">Consulta</p><h2>Listado</h2></div>
        <div class="filters">
          ${config.search ? `<input data-search="${config.title}" type="search" placeholder="Buscar">` : ""}
          <label class="check-field"><input data-include-inactive="${config.title}" type="checkbox"> Inactivos</label>
        </div>
      </div>
      <div class="table-wrapper">
        <table>
          <thead><tr>${config.columns.map((column) => `<th>${column}</th>`).join("")}<th>Acción</th></tr></thead>
          <tbody data-body="${config.title}"></tbody>
        </table>
      </div>
    </section>
  `;

  container.querySelector("form")?.addEventListener("submit", (event) => guardarFormulario(event, config));
  container.querySelector("[data-search]")?.addEventListener("input", () => cargarModuloPorTitulo(config.title));
  container.querySelector("[data-include-inactive]")?.addEventListener("change", () => cargarModuloPorTitulo(config.title));
  container.querySelectorAll("[data-action-url]").forEach((button) => {
    button.addEventListener("click", () => ejecutarAccionJson(button.dataset.actionUrl, config));
  });
}

function construirFormulario(config) {
  return `
    <form class="form-grid" data-form="${config.title}">
      ${config.fields.map((field) => campoHtml(field)).join("")}
      <button type="submit">Crear</button>
    </form>
  `;
}

function construirAcciones(config) {
  return `
    <div class="json-action">
      <textarea data-json="${config.title}" spellcheck="false">${JSON.stringify(config.sample ?? {}, null, 2)}</textarea>
      <div class="action-row">
        ${(config.actions ?? []).map(([label, url]) => `<button type="button" data-action-url="${url}">${label}</button>`).join("")}
      </div>
    </div>
  `;
}

function campoHtml([name, label, type, required, options]) {
  if (type === "checkbox") {
    return `<label class="check-field"><input name="${name}" type="checkbox"> ${label}</label>`;
  }

  if (type === "select") {
    return `
      <label>${label}
        <select name="${name}" ${required ? "required" : ""}>
          ${options.map((option) => `<option value="${option}">${option}</option>`).join("")}
        </select>
      </label>
    `;
  }

  return `<label>${label}<input name="${name}" type="${type}" ${required ? "required" : ""}></label>`;
}

async function guardarFormulario(event, config) {
  event.preventDefault();

  const form = event.currentTarget;
  const payload = {};

  config.fields.forEach(([name, , type]) => {
    const field = form.elements[name];

    if (type === "checkbox") {
      payload[name] = field.checked;
      return;
    }

    if (type === "number") {
      payload[name] = field.value ? Number(field.value) : null;
      return;
    }

    payload[name] = field.value.trim() || null;
  });

  await enviar(config.endpoint, "POST", payload, config);
  form.reset();
}

async function ejecutarAccionJson(url, config) {
  const textarea = document.querySelector(`[data-json="${config.title}"]`);
  const payload = JSON.parse(textarea.value);
  await enviar(url, "POST", payload, config);
}

async function enviar(url, method, payload, config) {
  try {
    await apiJson(url, {
      method,
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    setStatus(config, "Operación realizada");
    await cargarTodo();
  } catch (error) {
    setStatus(config, error.message);
  }
}

async function cargarModulo(moduleKey) {
  const config = modules[moduleKey];
  const params = new URLSearchParams();
  const search = document.querySelector(`[data-search="${config.title}"]`);
  const includeInactive = document.querySelector(`[data-include-inactive="${config.title}"]`);

  if (search?.value.trim()) {
    params.set("buscar", search.value.trim());
  }

  if (includeInactive?.checked) {
    params.set("incluirInactivos", "true");
  }

  const queryString = params.toString();
  const url = queryString ? `${config.endpoint}?${queryString}` : config.endpoint;

  try {
    const data = await apiJson(url);
    renderizarTabla(config, Array.isArray(data) ? data : []);
    setStatus(config, "Datos actualizados");
  } catch (error) {
    setStatus(config, error.message);
  }
}

function cargarModuloPorTitulo(title) {
  const moduleKey = Object.keys(modules).find((key) => modules[key].title === title);

  if (moduleKey) {
    cargarModulo(moduleKey);
  }
}

function renderizarTabla(config, rows) {
  const tbody = document.querySelector(`[data-body="${config.title}"]`);

  if (!tbody) {
    return;
  }

  if (!rows.length) {
    tbody.innerHTML = `<tr><td colspan="${config.columns.length + 1}">Sin registros.</td></tr>`;
    return;
  }

  tbody.innerHTML = rows.map((row) => `
    <tr>
      ${config.columns.map((column) => `<td>${formatearValor(row[column])}</td>`).join("")}
      <td>${accionesFila(config, row)}</td>
    </tr>
  `).join("");

  tbody.querySelectorAll("[data-row-action]").forEach((button) => {
    button.addEventListener("click", () => ejecutarAccionFila(config, button));
  });
}

function accionesFila(config, row) {
  const id = row[config.key];
  const buttons = [];

  if (config.extraAction) {
    buttons.push(`<button class="table-action" type="button" data-row-action="extra" data-id="${id}">${config.extraAction.label}</button>`);
  }

  if (config.key && typeof row.activo === "boolean") {
    buttons.push(`<button class="table-action" type="button" data-row-action="${row.activo ? "desactivar" : "reactivar"}" data-id="${id}">${row.activo ? "Desactivar" : "Reactivar"}</button>`);
  }

  return buttons.join(" ") || "-";
}

async function ejecutarAccionFila(config, button) {
  const action = button.dataset.rowAction;
  const id = button.dataset.id;

  if (action === "extra") {
    await config.extraAction(id);
    return;
  }

  const url = action === "desactivar" ? `${config.endpoint}/${id}` : `${config.endpoint}/${id}/reactivar`;
  const method = action === "desactivar" ? "DELETE" : "PATCH";

  await enviar(url, method, null, config);
}

async function cancelarPedido(id) {
  await enviar(`/api/pedidos/${id}/cancelar`, "PATCH", null, modules.pedidos);
}

async function cargarHistorialCilindro(id) {
  const data = await apiJson(`/api/cilindros/${id}/movimientos`);
  alert(JSON.stringify(data, null, 2));
}

function renderizarMovimientosDashboard(movimientos) {
  movimientosBody.innerHTML = movimientos.length
    ? movimientos.map((movimiento) => `
      <tr>
        <td>${formatearFecha(movimiento.fechaMovimiento)}</td>
        <td>${movimiento.codigoCilindro}</td>
        <td>${movimiento.producto}</td>
        <td>${movimiento.tipoMovimiento}</td>
        <td>${movimiento.cliente ?? "Sin cliente"}</td>
        <td>${movimiento.observacion ?? ""}</td>
      </tr>
    `).join("")
    : `<tr><td colspan="6">No hay movimientos registrados.</td></tr>`;
}

async function apiJson(url, options = {}) {
  const response = await fetch(url, options);

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Error HTTP ${response.status}`);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

function setStatus(config, message) {
  const status = document.querySelector(`#status-${config.moduleKey}`);

  if (status) {
    status.textContent = message;
  }
}

function formatearValor(value) {
  if (value === null || value === undefined) {
    return "-";
  }

  if (typeof value === "boolean") {
    return `<span class="status-pill ${value ? "" : "inactive"}">${value ? "Activo" : "Inactivo"}</span>`;
  }

  if (String(value).includes("T")) {
    const date = new Date(value);

    if (!Number.isNaN(date.getTime())) {
      return formatearFecha(value);
    }
  }

  return value;
}

function formatearFecha(fecha) {
  return new Intl.DateTimeFormat("es-PE", {
    dateStyle: "short",
    timeStyle: "short"
  }).format(new Date(fecha));
}
