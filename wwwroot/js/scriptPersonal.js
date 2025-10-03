// Global variables
let resumenData = null;
const API_BASE_URL = "http://localhost:5122";

// Initialize page
document.addEventListener("DOMContentLoaded", function () {
  loadResumen();
  initializeEventListeners();
});

// Initialize event listeners
function initializeEventListeners() {
  const searchForm = document.getElementById("searchForm");
  const clearBtn = document.getElementById("clearBtn");
  const addPersonalBtn = document.getElementById("addPersonalBtn");

  if (searchForm) {
    searchForm.addEventListener("submit", handleSearch);
  }

  if (clearBtn) {
    clearBtn.addEventListener("click", clearFilters);
  }

  if (addPersonalBtn) {
    addPersonalBtn.addEventListener("click", function () {
      showAlert("Funcionalidad de agregar personal en desarrollo", "error");
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
      renderSummaryCards(result.data);
      populateDropdowns(result.data);
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

  // Clear previous errors
  clearErrors();

  // Get form data
  const tipoPersonalValue = document.getElementById("tipoPersonal").value;
  const consultorioValue = document.getElementById("consultorio").value;

  const formData = {
    Nombre: document.getElementById("nombre").value.trim() || null,
    Dni: document.getElementById("dni").value.trim() || null,
    Estado: document.getElementById("estado").value || null,

    // 👇 Aquí la lógica que pediste
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

      // Display field errors
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

  // Reset table to initial state
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
                    })" title="Ver">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                            <circle cx="12" cy="12" r="3"></circle>
                        </svg>
                    </button>
                    <button class="action-btn edit" onclick="editPersonal(${
                      personal.id
                    })" title="Editar">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>
                            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>
                        </svg>
                    </button>
                    <button class="action-btn delete" onclick="deletePersonal(${
                      personal.id
                    })" title="Eliminar">
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

// Action functions
function viewPersonal(id) {
  showAlert(`Ver personal con ID: ${id}`, "success");
  console.log("View personal:", id);
}

function editPersonal(id) {
  showAlert(`Editar personal con ID: ${id}`, "success");
  console.log("Edit personal:", id);
}

function deletePersonal(id) {
  if (confirm("¿Está seguro que desea eliminar este personal?")) {
    showAlert(`Eliminar personal con ID: ${id}`, "success");
    console.log("Delete personal:", id);
  }
}

// Make functions globally accessible
window.viewPersonal = viewPersonal;
window.editPersonal = editPersonal;
window.deletePersonal = deletePersonal;
