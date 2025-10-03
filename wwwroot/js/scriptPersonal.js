// Global variables
let resumenData = null;
let consultoriosList = [];
const API_BASE_URL = "http://localhost:5122";

// Initialize page
document.addEventListener("DOMContentLoaded", function () {
  loadResumen();
  initializeEventListeners();
});

// Initialize event listeners
// Initialize event listeners
function initializeEventListeners() {
  const searchForm = document.getElementById("searchForm");
  const clearBtn = document.getElementById("clearBtn");
  const addMedicoBtn = document.getElementById("addMedicoBtn");
  const addEnfermeriaBtn = document.getElementById("addEnfermeriaBtn");
  const addTecnicoBtn = document.getElementById("addTecnicoBtn");

  if (searchForm) {
    searchForm.addEventListener("submit", handleSearch);
  }

  if (clearBtn) {
    clearBtn.addEventListener("click", clearFilters);
  }

  if (addMedicoBtn) {
    addMedicoBtn.addEventListener("click", function () {
      showAlert("Funcionalidad de agregar médico en desarrollo", "error");
    });
  }

  if (addEnfermeriaBtn) {
    addEnfermeriaBtn.addEventListener("click", function () {
      showAlert("Funcionalidad de agregar enfermería en desarrollo", "error");
    });
  }

  if (addTecnicoBtn) {
    addTecnicoBtn.addEventListener("click", function () {
      showAlert(
        "Funcionalidad de agregar personal técnico en desarrollo",
        "error"
      );
    });
  }
}

// Load summary data
async function loadResumen() {
  try {
    const response = await fetch(`${API_BASE_URL}/personal/resumen`);
    const result = await response.json();

    if (result.success) {
      resumenData = result.data;
      consultoriosList = result.data.consultoriosList || [];
      renderSummaryCards(result.data);
      populateDropdowns(result.data);
      console.log("Resumen data loaded:", result.data);
    } else {
      showAlert(result.message, "error");
    }
  } catch (error) {
    console.error("Error loading resumen:", error);
    showAlert("Error al cargar el resumen del personal", "error");
  }
}

// Render summary cards
function renderSummaryCards(data) {
  const summaryCards = document.getElementById("summaryCards");

  summaryCards.innerHTML = `
        <div class="summary-card">
            <div class="card-icon blue">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"></path>
                    <circle cx="9" cy="7" r="4"></circle>
                    <path d="M22 21v-2a4 4 0 0 0-3-3.87"></path>
                    <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
                </svg>
            </div>
            <div class="card-value">${data.medicosActivos}</div>
            <div class="card-label">Médicos Activos</div>
        </div>
        <div class="summary-card">
            <div class="card-icon green">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                    <circle cx="12" cy="7" r="4"></circle>
                </svg>
            </div>
            <div class="card-value">${data.tecnicosActivos}</div>
            <div class="card-label">Técnicos Activos</div>
        </div>
        <div class="summary-card">
            <div class="card-icon orange">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <rect x="2" y="7" width="20" height="14" rx="2" ry="2"></rect>
                    <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"></path>
                </svg>
            </div>
            <div class="card-value">${data.consultorios}</div>
            <div class="card-label">Consultorios</div>
        </div>
        <div class="summary-card">
            <div class="card-icon purple">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect>
                    <line x1="16" y1="2" x2="16" y2="6"></line>
                    <line x1="8" y1="2" x2="8" y2="6"></line>
                    <line x1="3" y1="10" x2="21" y2="10"></line>
                </svg>
            </div>
            <div class="card-value">${data.personalCaja}</div>
            <div class="card-label">Personal de Caja</div>
        </div>
    `;
}

// Populate dropdowns
function populateDropdowns(data) {
  const tipoPersonalSelect = document.getElementById("tipoPersonal");
  const consultorioSelect = document.getElementById("consultorio");

  // Populate Tipo Personal
  if (data.cargos && tipoPersonalSelect) {
    data.cargos.forEach((cargo) => {
      const option = document.createElement("option");
      option.value = cargo.nombreCargo;
      option.textContent = cargo.nombreCargo;
      tipoPersonalSelect.appendChild(option);
    });
  }

  // Populate Consultorio
  if (data.consultoriosList && consultorioSelect) {
    data.consultoriosList.forEach((consultorio) => {
      const option = document.createElement("option");
      option.value = consultorio.idConsultorio;
      option.textContent = consultorio.nombre;
      consultorioSelect.appendChild(option);
    });
  }
}

// Handle search form submission
async function handleSearch(e) {
  e.preventDefault();

  clearErrors();

  const tipoPersonalValue = document.getElementById("tipoPersonal").value;
  const consultorioValue = document.getElementById("consultorio").value;

  const formData = {
    Nombre: document.getElementById("nombre").value.trim() || null,
    Dni: document.getElementById("dni").value.trim() || null,
    Estado: document.getElementById("estado").value || null,
    TipoPersonal: tipoPersonalValue === "" ? "TODOS" : tipoPersonalValue,
    IdConsultorio: consultorioValue === "" ? null : parseInt(consultorioValue),
  };

  try {
    const response = await fetch(`${API_BASE_URL}/personal/buscar`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(formData),
    });

    const result = await response.json();

    if (result.success) {
      showAlert(result.message, "success");
      renderTable(result.data);
    } else {
      showAlert(result.message, "error");

      if (result.data && Array.isArray(result.data)) {
        result.data.forEach((error) => {
          displayFieldError(error.field, error.errors);
        });
      }
    }
  } catch (error) {
    console.error("Error searching personal:", error);
    showAlert("Error al buscar personal", "error");
  }
}

// Display field error
function displayFieldError(field, errors) {
  const errorElement = document.getElementById(`error-${field}`);
  const inputElement =
    document.getElementById(field.toLowerCase()) ||
    document.querySelector(`[name="${field}"]`);

  if (errorElement && errors && errors.length > 0) {
    errorElement.textContent = errors[0];
    errorElement.classList.add("show");
  }

  if (inputElement) {
    inputElement.classList.add("error");
  }
}

// Clear all errors
function clearErrors() {
  const errorMessages = document.querySelectorAll(".error-message");
  const errorInputs = document.querySelectorAll(".form-control.error");

  errorMessages.forEach((el) => {
    el.classList.remove("show");
    el.textContent = "";
  });

  errorInputs.forEach((el) => {
    el.classList.remove("error");
  });
}

// Clear filters
function clearFilters() {
  document.getElementById("searchForm").reset();
  clearErrors();

  const tableBody = document.getElementById("tableBody");
  tableBody.innerHTML = `
        <tr>
            <td colspan="6" class="no-data">No se han aplicado filtros. Use los filtros para buscar personal.</td>
        </tr>
    `;
}

// Render table
function renderTable(data) {
  const tableBody = document.getElementById("tableBody");

  if (!data || data.length === 0) {
    tableBody.innerHTML = `
            <tr>
                <td colspan="6" class="no-data">No se encontraron resultados</td>
            </tr>
        `;
    return;
  }

  tableBody.innerHTML = data
    .map(
      (personal) => `
        <tr>
            <td>${personal.dni}</td>
            <td>${personal.nombresApellidos}</td>
            <td>${personal.cargo}</td>
            <td>
                <span class="status-badge ${
                  personal.estado.toLowerCase() === "activo"
                    ? "active"
                    : "inactive"
                }">
                    ${personal.estado}
                </span>
            </td>
            <td>${personal.telefono}</td>
            <td>
                <div class="action-buttons">
                    <button class="action-btn view" onclick="viewPersonal(${
                      personal.id
                    }, '${personal.cargo}')" title="Ver">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                            <circle cx="12" cy="12" r="3"></circle>
                        </svg>
                    </button>
                    <button class="action-btn edit" onclick="editPersonal(${
                      personal.id
                    }, '${personal.cargo}')" title="Editar">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>
                            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>
                        </svg>
                    </button>
                    <button class="action-btn delete" onclick="deletePersonal(${
                      personal.id
                    }, '${personal.cargo}', '${personal.nombresApellidos}', '${
        personal.dni
      }')" title="Eliminar">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <polyline points="3 6 5 6 21 6"></polyline>
                            <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                            <line x1="10" y1="11" x2="10" y2="17"></line>
                            <line x1="14" y1="11" x2="14" y2="17"></line>
                        </svg>
                    </button>
                </div>
            </td>
        </tr>
    `
    )
    .join("");
}

// ==================== VER DETALLES ====================
async function viewPersonal(id, cargo) {
  try {
    let endpoint = "";
    let title = "";

    if (cargo === "MEDICO GENERAL") {
      endpoint = `/medicos/${id}`;
      title = "Detalles del Médico";
    } else if (cargo === "ENFERMERIA") {
      endpoint = `/enfermerias/${id}`;
      title = "Detalles de Enfermería";
    } else if (cargo === "CAJERO" || cargo === "ADMINISTRADOR") {
      endpoint = `/personal-tecnico/${id}`;
      title = "Detalles del Personal Técnico";
    }

    const response = await fetch(`${API_BASE_URL}${endpoint}`);
    const result = await response.json();

    if (result.success) {
      showViewModal(result.data, cargo, title);
    } else {
      showAlert(result.message, "error");
    }
  } catch (error) {
    console.error("Error al obtener detalles:", error);
    showAlert("Error al cargar los detalles del personal", "error");
  }
}

function showViewModal(data, cargo, title) {
  const modal = document.getElementById("viewModal");
  const modalTitle = document.getElementById("viewModalTitle");
  const modalBody = document.getElementById("viewModalBody");

  modalTitle.textContent = title;

  let html = "";

  if (cargo === "MEDICO GENERAL") {
    html = `
      <div class="personal-detail-grid">
        <div class="personal-detail-item"><div class="personal-detail-label">DNI:</div><div class="personal-detail-value">${
          data.numeroDni
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Nombre:</div><div class="personal-detail-value">${
          data.nombre
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Apellido Paterno:</div><div class="personal-detail-value">${
          data.apellidoPaterno
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Apellido Materno:</div><div class="personal-detail-value">${
          data.apellidoMaterno
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Sexo:</div><div class="personal-detail-value">${
          data.sexo === "M" ? "Masculino" : "Femenino"
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Fecha de Nacimiento:</div><div class="personal-detail-value">${formatDate(
          data.fechaNacimiento
        )}</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Dirección:</div><div class="personal-detail-value">${
          data.direccion
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Teléfono:</div><div class="personal-detail-value">${
          data.telefono
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Correo Electrónico:</div><div class="personal-detail-value">${
          data.correoElectronico
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Estado Laboral:</div><div class="personal-detail-value"><span class="status-badge ${
          data.estadoLaboral.toLowerCase() === "activo" ? "active" : "inactive"
        }">${data.estadoLaboral}</span></div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Fecha de Ingreso:</div><div class="personal-detail-value">${formatDate(
          data.fechaIngreso
        )}</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Turno:</div><div class="personal-detail-value">${
          data.turno
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Área/Servicio:</div><div class="personal-detail-value">${
          data.areaServicio
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Cargo:</div><div class="personal-detail-value">${
          data.cargoMedico
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Número de Colegiatura:</div><div class="personal-detail-value">${
          data.numeroColegiatura
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Tipo de Médico:</div><div class="personal-detail-value">${
          data.tipoMedico
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">ID Consultorio:</div><div class="personal-detail-value">${
          data.idConsultorio || "N/A"
        }</div></div>
      </div>
    `;
  } else if (cargo === "ENFERMERIA") {
    const personal = data.personal;
    html = `
      <h4 style="margin-bottom: 15px; color: #2c3e50;">Datos Personales</h4>
      <div class="personal-detail-grid">
        <div class="personal-detail-item"><div class="personal-detail-label">DNI:</div><div class="personal-detail-value">${
          personal.numeroDni
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Nombre:</div><div class="personal-detail-value">${
          personal.nombre
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Apellido Paterno:</div><div class="personal-detail-value">${
          personal.apellidoPaterno
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Apellido Materno:</div><div class="personal-detail-value">${
          personal.apellidoMaterno
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Sexo:</div><div class="personal-detail-value">${
          personal.sexo === "M" ? "Masculino" : "Femenino"
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Fecha de Nacimiento:</div><div class="personal-detail-value">${formatDate(
          personal.fechaNacimiento
        )}</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Dirección:</div><div class="personal-detail-value">${
          personal.direccion
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Teléfono:</div><div class="personal-detail-value">${
          personal.telefono
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Email:</div><div class="personal-detail-value">${
          personal.email
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Estado Laboral:</div><div class="personal-detail-value"><span class="status-badge ${
          personal.estadoLaboral.toLowerCase() === "activo"
            ? "active"
            : "inactive"
        }">${personal.estadoLaboral}</span></div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Fecha de Ingreso:</div><div class="personal-detail-value">${formatDate(
          personal.fechaIngreso
        )}</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Turno:</div><div class="personal-detail-value">${
          personal.turno
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Área/Servicio:</div><div class="personal-detail-value">${
          personal.areaServicio
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Cargo:</div><div class="personal-detail-value">${
          personal.cargo
        }</div></div>
      </div>
      <h4 style="margin: 25px 0 15px; color: #2c3e50;">Información de Enfermería</h4>
      <div class="personal-detail-grid">
        <div class="personal-detail-item"><div class="personal-detail-label">ID Enfermería:</div><div class="personal-detail-value">${
          data.idEnfermeria
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Número de Colegiatura:</div><div class="personal-detail-value">${
          data.numeroColegiaturaEnfermeria
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Nivel Profesional:</div><div class="personal-detail-value">${
          data.nivelProfesional
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">ID Consultorio:</div><div class="personal-detail-value">${
          data.idConsultorio || "N/A"
        }</div></div>
      </div>
    `;
  } else {
    html = `
      <div class="personal-detail-grid">
        <div class="personal-detail-item"><div class="personal-detail-label">DNI:</div><div class="personal-detail-value">${
          data.numeroDni
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Nombre:</div><div class="personal-detail-value">${
          data.nombre
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Apellido Paterno:</div><div class="personal-detail-value">${
          data.apellidoPaterno
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Apellido Materno:</div><div class="personal-detail-value">${
          data.apellidoMaterno
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Sexo:</div><div class="personal-detail-value">${
          data.sexo === "M" ? "Masculino" : "Femenino"
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Fecha de Nacimiento:</div><div class="personal-detail-value">${formatDate(
          data.fechaNacimiento
        )}</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Dirección:</div><div class="personal-detail-value">${
          data.direccion
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Teléfono:</div><div class="personal-detail-value">${
          data.telefono
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Email:</div><div class="personal-detail-value">${
          data.email
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Estado Laboral:</div><div class="personal-detail-value"><span class="status-badge ${
          data.estadoLaboral.toLowerCase() === "activo" ? "active" : "inactive"
        }">${data.estadoLaboral}</span></div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Fecha de Ingreso:</div><div class="personal-detail-value">${formatDate(
          data.fechaIngreso
        )}</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Turno:</div><div class="personal-detail-value">${
          data.turno
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Área/Servicio:</div><div class="personal-detail-value">${
          data.areaServicio
        }</div></div>
        <div class="personal-detail-item"><div class="personal-detail-label">Cargo:</div><div class="personal-detail-value">${
          data.cargo
        }</div></div>
      </div>
    `;
  }

  modalBody.innerHTML = html;
  modal.classList.add("show");
}

function closeViewModal() {
  const modal = document.getElementById("viewModal");
  modal.classList.remove("show");
}

// ==================== EDITAR ====================
async function editPersonal(id, cargo) {
  try {
    let endpoint = "";
    let title = "";

    if (cargo === "MEDICO GENERAL") {
      endpoint = `/medicos/${id}`;
      title = "Actualizar Médico";
    } else if (cargo === "ENFERMERIA") {
      endpoint = `/enfermerias/${id}`;
      title = "Actualizar Enfermería";
    } else if (cargo === "CAJERO" || cargo === "ADMINISTRADOR") {
      endpoint = `/personal-tecnico/${id}`;
      title = "Actualizar Personal Técnico";
    }

    const response = await fetch(`${API_BASE_URL}${endpoint}`);
    const result = await response.json();

    if (result.success) {
      showEditModal(result.data, cargo, title);
    } else {
      showAlert(result.message, "error");
    }
  } catch (error) {
    console.error("Error al obtener datos para editar:", error);
    showAlert("Error al cargar los datos del personal", "error");
  }
}

function showEditModal(data, cargo, title) {
  const modal = document.getElementById("editModal");
  const modalTitle = document.getElementById("editModalTitle");

  modalTitle.textContent = title;

  // Populate consultorio selects
  populateConsultorioSelects();

  // Ocultar todas las secciones especializadas
  document.getElementById("medicoSection").style.display = "none";
  document.getElementById("enfermeriaSection").style.display = "none";

  // Ajustar opciones de turno según el cargo
  const turnoSelect = document.getElementById("edit_turno");
  turnoSelect.innerHTML = '<option value="">Seleccione...</option>';

  if (cargo === "ADMINISTRADOR") {
    turnoSelect.innerHTML += '<option value="Mañana">Mañana</option>';
    turnoSelect.innerHTML += '<option value="Tarde">Tarde</option>';
    turnoSelect.innerHTML += '<option value="Noche">Noche</option>';
  } else {
    turnoSelect.innerHTML += '<option value="Mañana">Mañana</option>';
    turnoSelect.innerHTML += '<option value="Tarde">Tarde</option>';
  }

  if (cargo === "MEDICO GENERAL") {
    document.getElementById("edit_id").value = data.idMedico;
    document.getElementById("edit_cargo").value = cargo;
    document.getElementById("edit_dni").value = data.numeroDni;
    document.getElementById(
      "edit_nombre"
    ).value = `${data.nombre} ${data.apellidoPaterno} ${data.apellidoMaterno}`;
    document.getElementById("edit_fechaNacimiento").value = formatDateForInput(
      data.fechaNacimiento
    );
    document.getElementById("edit_sexo").value = data.sexo;
    document.getElementById("edit_direccion").value = data.direccion;
    document.getElementById("edit_telefono").value = data.telefono;
    document.getElementById("edit_email").value = data.correoElectronico;
    document.getElementById("edit_estadoLaboral").value = data.estadoLaboral;
    document.getElementById("edit_fechaIngreso").value = formatDateForInput(
      data.fechaIngreso
    );
    document.getElementById("edit_turno").value = data.turno;
    document.getElementById("edit_areaServicio").value = data.areaServicio;
    document.getElementById("edit_cargoDisplay").value = data.cargoMedico;

    // Mostrar y llenar campos de médico
    document.getElementById("medicoSection").style.display = "block";
    document.getElementById("edit_numeroColegiatura").value =
      data.numeroColegiatura;
    document.getElementById("edit_tipoMedico").value = data.tipoMedico;
    document.getElementById("edit_consultorioMedico").value =
      data.idConsultorio || "";
  } else if (cargo === "ENFERMERIA") {
    const personal = data.personal;
    document.getElementById("edit_id").value = data.idEnfermeria;
    document.getElementById("edit_cargo").value = cargo;
    document.getElementById("edit_dni").value = personal.numeroDni;
    document.getElementById(
      "edit_nombre"
    ).value = `${personal.nombre} ${personal.apellidoPaterno} ${personal.apellidoMaterno}`;
    document.getElementById("edit_fechaNacimiento").value = formatDateForInput(
      personal.fechaNacimiento
    );
    document.getElementById("edit_sexo").value = personal.sexo;
    document.getElementById("edit_direccion").value = personal.direccion;
    document.getElementById("edit_telefono").value = personal.telefono;
    document.getElementById("edit_email").value = personal.email;
    document.getElementById("edit_estadoLaboral").value =
      personal.estadoLaboral;
    document.getElementById("edit_fechaIngreso").value = formatDateForInput(
      personal.fechaIngreso
    );
    document.getElementById("edit_turno").value = personal.turno;
    document.getElementById("edit_areaServicio").value = personal.areaServicio;
    document.getElementById("edit_cargoDisplay").value = personal.cargo;

    // Mostrar y llenar campos de enfermería
    document.getElementById("enfermeriaSection").style.display = "block";
    document.getElementById("edit_numeroColegiaturaEnf").value =
      data.numeroColegiaturaEnfermeria;
    document.getElementById("edit_nivelProfesional").value =
      data.nivelProfesional;
    document.getElementById("edit_consultorioEnf").value =
      data.idConsultorio || "";

    // Guardar idPersonal como atributo
    document
      .getElementById("edit_id")
      .setAttribute("data-id-personal", data.idPersonal);
  } else {
    document.getElementById("edit_id").value = data.idPersonal;
    document.getElementById("edit_cargo").value = cargo;
    document.getElementById("edit_dni").value = data.numeroDni;
    document.getElementById(
      "edit_nombre"
    ).value = `${data.nombre} ${data.apellidoPaterno} ${data.apellidoMaterno}`;
    document.getElementById("edit_fechaNacimiento").value = formatDateForInput(
      data.fechaNacimiento
    );
    document.getElementById("edit_sexo").value = data.sexo;
    document.getElementById("edit_direccion").value = data.direccion;
    document.getElementById("edit_telefono").value = data.telefono;
    document.getElementById("edit_email").value = data.email;
    document.getElementById("edit_estadoLaboral").value = data.estadoLaboral;
    document.getElementById("edit_fechaIngreso").value = formatDateForInput(
      data.fechaIngreso
    );
    document.getElementById("edit_turno").value = data.turno;
    document.getElementById("edit_areaServicio").value = data.areaServicio;
    document.getElementById("edit_cargoDisplay").value = data.cargo;
  }

  modal.classList.add("show");
}

function populateConsultorioSelects() {
  const medicoSelect = document.getElementById("edit_consultorioMedico");
  const enfSelect = document.getElementById("edit_consultorioEnf");

  const options =
    '<option value="">Seleccione...</option>' +
    consultoriosList
      .map((c) => `<option value="${c.idConsultorio}">${c.nombre}</option>`)
      .join("");

  medicoSelect.innerHTML = options;
  enfSelect.innerHTML = options;
}

async function saveEdit() {
  const cargo = document.getElementById("edit_cargo").value;
  const id = document.getElementById("edit_id").value;

  // Dividir el nombre completo
  const nombreCompleto = document
    .getElementById("edit_nombre")
    .value.trim()
    .split(" ");
  const nombre = nombreCompleto[0] || "";
  const apellidoPaterno = nombreCompleto[1] || "";
  const apellidoMaterno = nombreCompleto.slice(2).join(" ") || "";

  let payload = {};
  let endpoint = "";

  if (cargo === "MEDICO GENERAL") {
    // DTO para Médico
    payload = {
      idMedico: parseInt(id),
      numeroDni: document.getElementById("edit_dni").value,
      nombre: nombre,
      apellidoPaterno: apellidoPaterno,
      apellidoMaterno: apellidoMaterno,
      sexo: document.getElementById("edit_sexo").value,
      fechaNacimiento: document.getElementById("edit_fechaNacimiento").value,
      direccion: document.getElementById("edit_direccion").value,
      telefono: document.getElementById("edit_telefono").value,
      correoElectronico: document.getElementById("edit_email").value,
      estadoLaboral: document.getElementById("edit_estadoLaboral").value,
      fechaIngreso: document.getElementById("edit_fechaIngreso").value,
      turno: document.getElementById("edit_turno").value,
      areaServicio: document.getElementById("edit_areaServicio").value,
      cargoMedico: "MEDICO GENERAL",
      numeroColegiatura: document.getElementById("edit_numeroColegiatura")
        .value,
      tipoMedico: document.getElementById("edit_tipoMedico").value,
      idConsultorio:
        parseInt(document.getElementById("edit_consultorioMedico").value) ||
        null,
    };
    endpoint = "/medicos/update";
  } else if (cargo === "ENFERMERIA") {
    // DTO para Enfermería
    const idPersonal = document
      .getElementById("edit_id")
      .getAttribute("data-id-personal");

    payload = {
      idEnfermeria: parseInt(id),
      idPersonal: parseInt(idPersonal),
      numeroColegiaturaEnfermeria: document.getElementById(
        "edit_numeroColegiaturaEnf"
      ).value,
      nivelProfesional: document.getElementById("edit_nivelProfesional").value,
      numeroDni: document.getElementById("edit_dni").value,
      nombre: nombre,
      apellidoPaterno: apellidoPaterno,
      apellidoMaterno: apellidoMaterno,
      fechaNacimiento: document.getElementById("edit_fechaNacimiento").value,
      sexo: document.getElementById("edit_sexo").value,
      direccion: document.getElementById("edit_direccion").value,
      telefono: document.getElementById("edit_telefono").value,
      email: document.getElementById("edit_email").value,
      estadoLaboral: document.getElementById("edit_estadoLaboral").value,
      fechaIngreso: document.getElementById("edit_fechaIngreso").value,
      turno: document.getElementById("edit_turno").value,
      areaServicio: document.getElementById("edit_areaServicio").value,
      cargo: "ENFERMERIA",
      idConsultorio:
        parseInt(document.getElementById("edit_consultorioEnf").value) || null,
    };
    endpoint = "/enfermerias/update";
  } else {
    // DTO para Personal Técnico (Cajero/Administrador)
    payload = {
      idPersonalT: parseInt(id),
      numeroDni: document.getElementById("edit_dni").value,
      nombre: nombre,
      apellidoPaterno: apellidoPaterno,
      apellidoMaterno: apellidoMaterno,
      fechaNacimiento: document.getElementById("edit_fechaNacimiento").value,
      sexo: document.getElementById("edit_sexo").value,
      direccion: document.getElementById("edit_direccion").value,
      telefono: document.getElementById("edit_telefono").value,
      email: document.getElementById("edit_email").value,
      estadoLaboral: document.getElementById("edit_estadoLaboral").value,
      fechaIngreso: document.getElementById("edit_fechaIngreso").value,
      turno: document.getElementById("edit_turno").value,
      areaServicio: document.getElementById("edit_areaServicio").value,
      cargo: cargo,
    };
    endpoint = "/personal-tecnico/update";
  }

  console.log("Payload a enviar:", payload);
  console.log("Endpoint:", endpoint);

  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    });

    const result = await response.json();

    if (result.success) {
      showAlert(result.message, "success");
      closeEditModal();
      // Recargar la búsqueda actual
      document.getElementById("searchForm").dispatchEvent(new Event("submit"));
    } else {
      showAlert(result.message || "Error al actualizar", "error");

      // Mostrar errores de validación si existen
      if (result.errors) {
        console.error("Errores de validación:", result.errors);
      }
    }
  } catch (error) {
    console.error("Error al actualizar:", error);
    showAlert("Error al actualizar el personal", "error");
  }
}

function closeEditModal() {
  const modal = document.getElementById("editModal");
  modal.classList.remove("show");
  document.getElementById("editForm").reset();
}

// ==================== ELIMINAR ====================
function deletePersonal(id, cargo, nombre, dni) {
  const modal = document.getElementById("deleteModal");

  document.getElementById("delete_id").value = id;
  document.getElementById("delete_tipoPersonal").value = cargo;
  document.getElementById("delete_nombre").textContent = nombre;
  document.getElementById("delete_cargo").textContent = cargo;
  document.getElementById("delete_dni").textContent = dni;

  modal.classList.add("show");
}

async function confirmDelete() {
  const id = document.getElementById("delete_id").value;
  const cargo = document.getElementById("delete_tipoPersonal").value;

  let endpoint = "";

  if (cargo === "MEDICO GENERAL") {
    endpoint = `/medicos/delete/${id}`;
  } else if (cargo === "ENFERMERIA") {
    endpoint = `/enfermerias/delete/${id}`;
  } else {
    endpoint = `/personal-tecnico/delete/${id}`;
  }

  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: "DELETE",
    });

    const result = await response.json();

    if (result.success) {
      showAlert(result.message, "success");
      closeDeleteModal();
      // Recargar la búsqueda actual
      document.getElementById("searchForm").dispatchEvent(new Event("submit"));
    } else {
      showAlert(result.message, "error");
    }
  } catch (error) {
    console.error("Error al eliminar:", error);
    showAlert("Error al eliminar el personal", "error");
  }
}

function closeDeleteModal() {
  const modal = document.getElementById("deleteModal");
  modal.classList.remove("show");
}

// ==================== UTILITY FUNCTIONS ====================
function formatDate(dateString) {
  if (!dateString) return "N/A";
  const date = new Date(dateString);
  return date.toLocaleDateString("es-ES", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
}

function formatDateForInput(dateString) {
  if (!dateString) return "";
  const date = new Date(dateString);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

function showAlert(message, type) {
  const alertContainer = document.getElementById("alertContainer");
  const alert = document.createElement("div");
  alert.className = `alert alert-${type}`;
  alert.innerHTML = `
    <span>${message}</span>
    <button class="alert-close" onclick="this.parentElement.remove()">&times;</button>
  `;

  alertContainer.appendChild(alert);

  setTimeout(() => {
    alert.remove();
  }, 5000);
}

// Cerrar modal al hacer clic fuera del contenido
window.addEventListener("click", function (event) {
  const viewModal = document.getElementById("viewModal");
  const editModal = document.getElementById("editModal");
  const deleteModal = document.getElementById("deleteModal");

  if (event.target === viewModal) {
    closeViewModal();
  }
  if (event.target === editModal) {
    closeEditModal();
  }
  if (event.target === deleteModal) {
    closeDeleteModal();
  }
});

// Cerrar modal con tecla ESC
window.addEventListener("keydown", function (event) {
  if (event.key === "Escape") {
    const viewModal = document.getElementById("viewModal");
    const editModal = document.getElementById("editModal");
    const deleteModal = document.getElementById("deleteModal");

    if (viewModal.classList.contains("show")) {
      closeViewModal();
    }
    if (editModal.classList.contains("show")) {
      closeEditModal();
    }
    if (deleteModal.classList.contains("show")) {
      closeDeleteModal();
    }
  }
});

// Hacer funciones globalmente accesibles
window.viewPersonal = viewPersonal;
window.editPersonal = editPersonal;
window.deletePersonal = deletePersonal;
window.closeViewModal = closeViewModal;
window.closeEditModal = closeEditModal;
window.closeDeleteModal = closeDeleteModal;
window.saveEdit = saveEdit;
window.confirmDelete = confirmDelete;
