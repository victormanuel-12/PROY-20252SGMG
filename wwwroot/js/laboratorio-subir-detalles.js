const API_BASE_URL = "http://localhost:5122";
let idOrdenActual = null;
let ordenData = null;

document.addEventListener("DOMContentLoaded", function () {
  obtenerParametrosURL();
  cargarInformacionOrden();
});

function obtenerParametrosURL() {
  const params = new URLSearchParams(window.location.search);
  idOrdenActual = params.get("idOrden");

  if (!idOrdenActual) {
    showError("No se especificó el ID de la orden");
    setTimeout(() => window.history.back(), 2000);
  }
}

async function cargarInformacionOrden() {
  try {
    const res = await fetch(
      `${API_BASE_URL}/laboratorio/api/orden/${idOrdenActual}`
    );
    const result = await res.json();

    if (result.success && result.orden) {
      ordenData = result.orden;
      mostrarInformacionOrden(result.orden);
    } else {
      showError("No se pudo cargar la información de la orden");
      setTimeout(() => window.history.back(), 2000);
    }
  } catch (error) {
    console.error("Error:", error);
    showError("Error al cargar datos de la orden");
  }
}

function mostrarInformacionOrden(orden) {
  const fecha = new Date(orden.fechaSolicitud).toLocaleDateString("es-PE");

  document.getElementById("ordenInfo").innerHTML = `
        <div class="info-item">
            <span class="info-label"># DE ORDEN</span>
            <span class="info-value">${orden.numeroOrden}</span>
        </div>

        <div class="info-item">
            <span class="info-label">TIPO DE EXAMEN</span>
            <span class="info-value">${orden.tipoExamen}</span>
        </div>

        <div class="info-item">
            <span class="info-label">PACIENTE</span>
            <span class="info-value">${orden.nombreCompletoPaciente}</span>
        </div>

        <div class="info-item">
            <span class="info-label">DNI PACIENTE</span>
            <span class="info-value">${orden.dniPaciente || "N/A"}</span>
        </div>

        <div class="info-item">
            <span class="info-label">FECHA DE ORDEN</span>
            <span class="info-value">${fecha}</span>
        </div>

        <div class="info-item">
            <span class="info-label">ESTADO ACTUAL</span>
            <span class="info-badge">${orden.estado}</span>
        </div>

        <div class="info-item">
            <span class="info-label">OBSERVACIONES DE LA ORDEN</span>
            <span class="info-value">${
              orden.observacionesAdicionales || "Sin observaciones"
            }</span>
        </div>
    `;
}
async function guardarDetalles(event) {
  event.preventDefault();

  const resultados = document.getElementById("resultados").value.trim();
  const observacionesFinales = document
    .getElementById("observacionesFinales")
    .value.trim();

  if (!resultados) {
    showWarning("Por favor ingrese los resultados del examen");
    return;
  }

  const detallesDTO = {
    idOrden: parseInt(idOrdenActual),
    resultados: resultados,
    fechaResultado: new Date().toISOString(),
    observacionesFinales: observacionesFinales,
    estado: "Realizado",
  };

  try {
    const res = await fetch(
      `${API_BASE_URL}/laboratorio/api/actualizar-resultados`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(detallesDTO),
      }
    );

    const result = await res.json();

    if (result.success) {
      showSuccess(result.message || "Resultados guardados exitosamente");
      setTimeout(() => {
        window.location.href = `/laboratorio?idPaciente=${ordenData.idPaciente}`;
      }, 1500);
    } else {
      showError(result.message || "Error al guardar los resultados");
    }
  } catch (error) {
    console.error("Error:", error);
    showError(
      "No se pudo cargar la información de la orden de examen. Por favor, regrese e intente nuevamente."
    );
  }
}

function volver() {
  if (
    confirm(
      "¿Está seguro de que desea salir? Los datos no guardados se perderán."
    )
  ) {
    window.location.href = `/laboratorio?idPaciente=${
      ordenData?.idPaciente || ""
    }`;
  }
}
