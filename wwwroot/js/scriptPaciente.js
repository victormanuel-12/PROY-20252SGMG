const API_BASE_URL = "http://localhost:5122";

// Inicialización
document.addEventListener("DOMContentLoaded", function () {
  loadResumenPacientes();
  initializePacienteEvents();
});

// Inicializar eventos
function initializePacienteEvents() {
  const searchForm = document.getElementById("searchPacienteForm");
  const clearBtn = document.getElementById("clearPacienteBtn");
  const addBtn = document.getElementById("addPacienteBtn");

  if (searchForm) {
    searchForm.addEventListener("submit", handlePacienteSearch);
  }
  if (clearBtn) {
    clearBtn.addEventListener("click", clearPacienteFilters);
  }
}

// Cargar resumen de pacientes
async function loadResumenPacientes() {
  try {
    // Aquí puedes ajustar el endpoint para traer el resumen real
    // Ejemplo de datos simulados:
    const data = {
      totalPacientes: 0,
      hombres: 0,
      mujeres: 0,
      registradosHoy: 0,
    };
    // Si tienes endpoint real, reemplaza por fetch
    // const res = await fetch(`${API_BASE_URL}/pacientes/resumen`);
    // const data = await res.json();

    renderSummaryCardsPaciente(data);
  } catch (error) {
    showAlert("Error al cargar el resumen de pacientes.", "error");
  }
}

// Renderizar tarjetas resumen
function renderSummaryCardsPaciente(data) {
  const container = document.getElementById("summaryCardsPaciente");
  if (!container) return;
  container.innerHTML = `
        <div class="summary-card">
            <div class="card-icon blue"><i class="fas fa-users"></i></div>
            <div class="card-value">${data.totalPacientes ?? 0}</div>
            <div class="card-label">Total Pacientes</div>
        </div>
        <div class="summary-card">
            <div class="card-icon green"><i class="fas fa-mars"></i></div>
            <div class="card-value">${data.hombres ?? 0}</div>
            <div class="card-label">Hombres</div>
        </div>
        <div class="summary-card">
            <div class="card-icon purple"><i class="fas fa-venus"></i></div>
            <div class="card-value">${data.mujeres ?? 0}</div>
            <div class="card-label">Mujeres</div>
        </div>
        <div class="summary-card">
            <div class="card-icon orange"><i class="fas fa-calendar-day"></i></div>
            <div class="card-value">${data.registradosHoy ?? 0}</div>
            <div class="card-label">Registrados Hoy</div>
        </div>
    `;
}

// Manejar búsqueda de pacientes
async function handlePacienteSearch(e) {
  e.preventDefault();
  clearPacienteErrors();

  const tipoDocumento = document.getElementById("tipoBusqueda").value;
  const numeroDocumento = document
    .getElementById("numeroDocumento")
    .value.trim();

  // Validación simple
  let hasError = false;
  if (!tipoDocumento) {
    displayPacienteFieldError(
      "TipoDocumento",
      "Seleccione un tipo de documento."
    );
    hasError = true;
  }
  if (!numeroDocumento) {
    displayPacienteFieldError(
      "NumeroDocumento",
      "Ingrese el número de documento."
    );
    hasError = true;
  }
  if (hasError) return;

  try {
    // Usar el nuevo endpoint de búsqueda
    const res = await fetch(
      `${API_BASE_URL}/pacientes/search?tipoDocumento=${encodeURIComponent(
        tipoDocumento
      )}&numeroDocumento=${encodeURIComponent(numeroDocumento)}`
    );
    const result = await res.json();

    if (!result.success || !result.data) {
      renderPacienteTable([]);
      renderCitasPendientesTable([]); // Limpiar citas
      showAlert(result.message || "No se encontró ningún paciente.", "info");
      return;
    }

    // Renderizar paciente encontrado
    renderPacienteTable([result.data]);
    showAlert(result.message || "Paciente encontrado.", "success");

    // Cargar citas pendientes del paciente
    await loadCitasPendientes(result.data.IdPaciente || result.data.idPaciente);
  } catch (error) {
    console.error("Error:", error);
    showAlert("Error al buscar pacientes.", "error");
  }
}

// Renderizar tabla de pacientes
function renderPacienteTable(data) {
  const tbody = document.getElementById("pacienteTableBody");
  const addHistoriaSection = document.getElementById("addHistoriaSection");
  if (!tbody) return;

  if (!data || data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="6" class="no-data">No hay datos disponibles. Use los filtros para buscar pacientes.</td></tr>`;
    if (addHistoriaSection) addHistoriaSection.style.display = "block";
    return;
  }

  if (addHistoriaSection) addHistoriaSection.style.display = "none";

  tbody.innerHTML = data
    .map(
      (p) => `
        <tr>
            <td>${p.TipoDocumento || p.tipoDocumento || ""}</td>
            <td>${p.NumeroDocumento || p.numeroDocumento || ""}</td>
            <td>${
              (p.ApellidoPaterno || p.apellidoPaterno || "") +
              " " +
              (p.ApellidoMaterno || p.apellidoMaterno || "") +
              ", " +
              (p.Nombre || p.nombre || "")
            }</td>
            <td>${
              (p.Edad !== undefined
                ? p.Edad
                : p.edad !== undefined
                ? p.edad
                : "-") + " años"
            }</td>
            <td>${
              p.Sexo === "M" || p.sexo === "M"
                ? "Masculino"
                : p.Sexo === "F" || p.sexo === "F"
                ? "Femenino"
                : ""
            }</td>
            <td>
                <div class="action-buttons">
                <button class="action-btn view" title="Ver" onclick="viewPaciente(${
                  p.IdPaciente || p.idPaciente
                })">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="action-btn edit" title="Editar" onclick="editPaciente(${
                  p.IdPaciente || p.idPaciente
                })">
                    <i class="fas fa-edit"></i>
                </button>
            </div>
            </td>
        </tr>
    `
    )
    .join("");
}

/* // Calcular edad (puedes ajustar según tu modelo)
function calcularEdad(fechaNacimiento) {
    if (!fechaNacimiento) return "-";
    const nacimiento = new Date(fechaNacimiento);
    const hoy = new Date();
    let edad = hoy.getFullYear() - nacimiento.getFullYear();
    const m = hoy.getMonth() - nacimiento.getMonth();
    if (m < 0 || (m === 0 && hoy.getDate() < nacimiento.getDate())) {
        edad--;
    }
    return `${edad} años`;
} */

// Mostrar errores de campo
function displayPacienteFieldError(field, message) {
  const el = document.getElementById(`error-${field}`);
  if (el) {
    el.textContent = message;
    el.classList.add("show");
  }
}

// Limpiar errores
function clearPacienteErrors() {
  document.querySelectorAll(".error-message").forEach((e) => {
    e.textContent = "";
    e.classList.remove("show");
  });
}

// Limpiar filtros
function clearPacienteFilters() {
  document.getElementById("tipoBusqueda").value = "";
  document.getElementById("numeroDocumento").value = "";
  clearPacienteErrors();
  renderPacienteTable([]);
  renderCitasPendientesTable([]); // Limpiar también las citas
  const addHistoriaSection = document.getElementById("addHistoriaSection");
  if (addHistoriaSection) addHistoriaSection.style.display = "none";
}
// Mostrar alertas
function showAlert(message, type = "success") {
  const alertContainer = document.getElementById("alertContainer");
  if (!alertContainer) return;
  const alert = document.createElement("div");
  alert.className = `alert alert-${type}`;
  alert.innerHTML = `
        <span>${message}</span>
        <button class="alert-close" onclick="this.parentElement.remove()">×</button>
    `;
  alertContainer.appendChild(alert);
  setTimeout(() => alert.remove(), 4000);
}

// Acciones
// Función para editar Historia Clínica
function editarHistoriaClinica(id) {
  showAlert(
    "Funcionalidad de editar Historia Clinica del paciente aún no implementada.",
    "info"
  );
}
function solicitarCita(id) {
  showAlert("Funcionalidad para solicitar cita aún no implementada.", "info");
}

// Cargar citas pendientes de un paciente
async function loadCitasPendientes(idPaciente) {
  try {
    const res = await fetch(
      `${API_BASE_URL}/pacientes/${idPaciente}/citas-pendientes`
    );
    const result = await res.json();

    if (!result.success || !result.data) {
      renderCitasPendientesTable([]);
      return;
    }

    renderCitasPendientesTable(result.data);
  } catch (error) {
    console.error("Error al cargar citas pendientes:", error);
    renderCitasPendientesTable([]);
  }
}

// Renderizar tabla de citas pendientes
function renderCitasPendientesTable(citas) {
  const tbody = document.getElementById("citasPendientesBody");
  if (!tbody) return;

  if (!citas || citas.length === 0) {
    tbody.innerHTML = `<tr><td colspan="7" class="no-data">No hay citas pendientes.</td></tr>`;
    return;
  }

    tbody.innerHTML = citas.map(c => {
        // Con el DTO ya vienen los datos formateados
        const fechaFormateada = c.fechaCita ? new Date(c.fechaCita).toLocaleDateString('es-PE') : "-";
        const horaFormateada = c.horaCita || "-";
        
        return `
            <tr>
                <td>${c.tipoDocumento || ""}</td>
                <td>${c.numeroDocumento || ""}</td>
                <td>${c.nombreCompletoPaciente || ""}</td>
                <td>${fechaFormateada}</td>
                <td>${horaFormateada}</td>
                <td><span class="badge badge-${getBadgeClass(c.estadoCita)}">${c.estadoCita || ""}</span></td>
                <td>${c.nombreCompletoMedico || ""}</td>
            </tr>
        `;
    })
    .join("");
}

// Obtener clase CSS para el badge según el estado
function getBadgeClass(estado) {
    switch(estado) {
        case "Confirmada":
            return "success";
        case "Pendiente":
            return "warning";
        case "Cancelada":
            return "danger";
        case "En Curso":
            return "info";
        default:
            return "secondary";
    }
}
