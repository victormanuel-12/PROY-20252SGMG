const API_BASE_URL = "http://localhost:5122";
let idPacienteActual = null;
let triajesData = [];
let paginaActual = 1;
const itemsPorPagina = 5;

document.addEventListener('DOMContentLoaded', function() {
    obtenerParametrosURL();
    cargarHistorialTriaje();
});

function obtenerParametrosURL() {
    const params = new URLSearchParams(window.location.search);
    idPacienteActual = params.get('idPaciente');
    
    if (!idPacienteActual) {
        showError('No se especificó el ID del paciente');
        setTimeout(() => window.history.back(), 2000);
    }
}

async function cargarHistorialTriaje() {
    try {
        const res = await fetch(`${API_BASE_URL}/api/triaje/historial-paciente/${idPacienteActual}`);
        const result = await res.json();

        if (result.success) {
            mostrarInformacionPaciente(result.paciente);
            triajesData = result.triajes || [];
            mostrarHistorialTriajes();
        } else {
            showError(result.message || 'Error al cargar el historial');
        }
    } catch (error) {
        console.error('Error:', error);
        showError('Error al conectar con el servidor');
    }
}

function mostrarInformacionPaciente(paciente) {
    document.getElementById('patientInfo').innerHTML = `
        <h2 class="patient-info-title">Información del Paciente</h2>
        <div class="patient-info-grid">
            <div class="patient-info-item">
                <span class="patient-info-label">DNI</span>
                <span class="patient-info-value">${paciente.dni}</span>
            </div>
            <div class="patient-info-item">
                <span class="patient-info-label">Nombre Completo</span>
                <span class="patient-info-value">${paciente.nombreCompleto}</span>
            </div>
            <div class="patient-info-item">
                <span class="patient-info-label">Sexo</span>
                <span class="patient-info-value">${paciente.sexo}</span>
            </div>
            <div class="patient-info-item">
                <span class="patient-info-label">Edad</span>
                <span class="patient-info-value">${paciente.edad} años</span>
            </div>
            <div class="patient-info-item">
                <span class="patient-info-label">Seguro</span>
                <span class="patient-info-value">${paciente.seguro}</span>
            </div>
        </div>
    `;
}

function mostrarHistorialTriajes() {
    const container = document.getElementById('triajeHistoryContainer');

    if (triajesData.length === 0) {
        container.innerHTML = `
            <div class="no-data">
                <div class="no-data-icon">📋</div>
                <p>No hay triajes registrados para este paciente</p>
            </div>
        `;
        return;
    }

    // Calcular paginación
    const inicio = (paginaActual - 1) * itemsPorPagina;
    const fin = inicio + itemsPorPagina;
    const triajesPagina = triajesData.slice(inicio, fin);

    const tabla = `
        <table class="triaje-table">
            <thead>
                <tr>
                    <th>CÓD. TRIAJE</th>
                    <th>FECHA Y HORA</th>
                    <th>PRESIÓN ARTERIAL</th>
                    <th>TEMPERATURA</th>
                    <th>ACCIONES</th>
                </tr>
            </thead>
            <tbody>
                ${triajesPagina.map((triaje, index) => `
                    <tr>
                        <td><strong>${triaje.codigoTriaje}</strong></td>
                        <td>${triaje.fechaHora}</td>
                        <td>${triaje.presionArterial}</td>
                        <td>${triaje.temperatura}</td>
                        <td>
                            <button class="btn-details" onclick="toggleDetalles(${inicio + index})">
                                Ver detalles
                            </button>
                        </td>
                    </tr>
                    <tr id="detalles-${inicio + index}" style="display: none;">
                        <td colspan="5">
                            <div class="triaje-details active">
                                <h3 class="details-title">Detalles del Triaje ${triaje.codigoTriaje}</h3>
                                <div class="details-grid">
                                    <div class="details-section">
                                        <h4 class="details-section-title">Signos Vitales</h4>
                                        <div class="details-row">
                                            <span class="details-label">Temperatura (°C):</span>
                                            <span class="details-value">${triaje.signos.temperatura}</span>
                                        </div>
                                        <div class="details-row">
                                            <span class="details-label">Presión arterial:</span>
                                            <span class="details-value">${triaje.signos.presionArterial}/80 mmHg</span>
                                        </div>
                                        <div class="details-row">
                                            <span class="details-label">Frecuencia cardiaca:</span>
                                            <span class="details-value">${triaje.signos.frecuenciaCardiaca} lpm</span>
                                        </div>
                                        <div class="details-row">
                                            <span class="details-label">Saturación O₂:</span>
                                            <span class="details-value">${triaje.signos.saturacion}%</span>
                                        </div>
                                    </div>

                                    <div class="details-section">
                                        <h4 class="details-section-title">Medidas Antropométricas</h4>
                                        <div class="details-row">
                                            <span class="details-label">Peso (kg):</span>
                                            <span class="details-value">${triaje.medidas.peso}</span>
                                        </div>
                                        <div class="details-row">
                                            <span class="details-label">Talla (cm):</span>
                                            <span class="details-value">${triaje.medidas.talla}</span>
                                        </div>
                                        <div class="details-row">
                                            <span class="details-label">IMC:</span>
                                            <span class="details-value">${triaje.medidas.imc} (${triaje.medidas.imcClasificacion})</span>
                                        </div>
                                    </div>

                                    <div class="details-section">
                                        <h4 class="details-section-title">Información del Triaje</h4>
                                        <div class="details-row">
                                            <span class="details-label">Fecha:</span>
                                            <span class="details-value">${triaje.informacion.fecha}</span>
                                        </div>
                                        <div class="details-row">
                                            <span class="details-label">Hora:</span>
                                            <span class="details-value">${triaje.informacion.hora}</span>
                                        </div>
                                        <div class="details-row">
                                            <span class="details-label">Realizado por:</span>
                                            <span class="details-value">${triaje.informacion.realizadoPor}</span>
                                        </div>
                                    </div>
                                </div>

                                ${triaje.observaciones ? `
                                    <div class="observaciones">
                                        <h4 class="observaciones-title">Observaciones</h4>
                                        <p class="observaciones-text">${triaje.observaciones}</p>
                                    </div>
                                ` : ''}
                            </div>
                        </td>
                    </tr>
                `).join('')}
            </tbody>
        </table>
        ${renderizarPaginacion()}
    `;

    container.innerHTML = tabla;
}

function toggleDetalles(index) {
    const detalleRow = document.getElementById(`detalles-${index}`);
    if (detalleRow.style.display === 'none') {
        // Cerrar todos los demás detalles
        document.querySelectorAll('[id^="detalles-"]').forEach(row => {
            row.style.display = 'none';
        });
        // Abrir el seleccionado
        detalleRow.style.display = 'table-row';
    } else {
        detalleRow.style.display = 'none';
    }
}

function renderizarPaginacion() {
    const totalPaginas = Math.ceil(triajesData.length / itemsPorPagina);

    if (totalPaginas <= 1) return '';

    let html = '<div class="pagination">';
    
    // Botón anterior
    html += `
        <button class="pagination-btn" onclick="cambiarPagina(${paginaActual - 1})" ${paginaActual === 1 ? 'disabled' : ''}>
            ← Anterior
        </button>
    `;

    // Números de página
    for (let i = 1; i <= totalPaginas; i++) {
        html += `
            <button class="pagination-btn ${i === paginaActual ? 'active' : ''}" onclick="cambiarPagina(${i})">
                ${i}
            </button>
        `;
    }

    // Botón siguiente
    html += `
        <button class="pagination-btn" onclick="cambiarPagina(${paginaActual + 1})" ${paginaActual === totalPaginas ? 'disabled' : ''}>
            Siguiente →
        </button>
    `;

    html += '</div>';
    return html;
}

function cambiarPagina(nuevaPagina) {
    const totalPaginas = Math.ceil(triajesData.length / itemsPorPagina);
    
    if (nuevaPagina < 1 || nuevaPagina > totalPaginas) return;
    
    paginaActual = nuevaPagina;
    mostrarHistorialTriajes();
    
    // Scroll al inicio de la tabla
    document.querySelector('.triaje-history-card').scrollIntoView({ behavior: 'smooth' });
}
