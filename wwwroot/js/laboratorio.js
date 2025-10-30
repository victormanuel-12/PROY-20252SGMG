const API_BASE_URL = "http://localhost:5122";
let idPacienteActual = null;
let ordenesData = [];

document.addEventListener('DOMContentLoaded', function() {
    obtenerParametrosURL();
    cargarHistorialLaboratorio();
});

function obtenerParametrosURL() {
    const params = new URLSearchParams(window.location.search);
    idPacienteActual = params.get('idPaciente');
    
    if (!idPacienteActual) {
        showError('No se especificó el ID del paciente');
        setTimeout(() => window.history.back(), 2000);
    }
}

async function cargarHistorialLaboratorio() {
    try {
        const res = await fetch(`${API_BASE_URL}/laboratorio/api/historial/${idPacienteActual}`);
        const result = await res.json();

        if (result.success) {
            mostrarInformacionPaciente(result.paciente);
            ordenesData = result.ordenes || [];
            mostrarOrdenes();
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

function mostrarOrdenes() {
    const container = document.getElementById('ordersContainer');

    if (ordenesData.length === 0) {
        container.innerHTML = `
            <div class="no-data">
                <div class="no-data-icon">🧪</div>
                <p>No hay órdenes de laboratorio registradas</p>
            </div>
        `;
        return;
    }

    const tabla = `
        <table class="orders-table">
            <thead>
                <tr>
                    <th># Orden</th>
                    <th>Examen</th>
                    <th>Paciente</th>
                    <th>Fecha</th>
                    <th>Observaciones</th>
                    <th>Estado</th>
                    <th>Opciones</th>
                </tr>
            </thead>
            <tbody>
                ${ordenesData.map((orden, index) => {
                    const estadoClass = orden.estado === 'Realizado' ? 'badge-realizado' 
                        : orden.estado === 'Cancelado' ? 'badge-cancelado' 
                        : 'badge-pendiente';
                    
                    const fecha = new Date(orden.fechaSolicitud).toLocaleDateString('es-PE');
                    const observaciones = orden.observacionesAdicionales.length > 30
                        ? orden.observacionesAdicionales.substring(0, 30) + '...'
                        : orden.observacionesAdicionales || '-';

                    return `
                        <tr onclick="toggleDetalles(${index})">
                            <td><strong>${orden.numeroOrden}</strong></td>
                            <td>${orden.tipoExamen}</td>
                            <td>${orden.nombreCompletoPaciente}</td>
                            <td>${fecha}</td>
                            <td>${observaciones}</td>
                            <td><span class="badge ${estadoClass}">${orden.estado}</span></td>
                            <td onclick="event.stopPropagation()">
                                <button class="btn-details" onclick="verDetalles(${index})">Ver detalles</button>
                            </td>
                        </tr>
                        <tr id="detalles-${index}" style="display: none;">
                            <td colspan="7">
                                <div class="order-details active">
                                    <h3 class="details-title">Detalles de la Orden ${orden.numeroOrden}</h3>
                                    <div class="details-grid">
                                        <div class="details-section">
                                            <h4 class="details-section-title">Examen</h4>
                                            <p style="padding: 12px; background: #f9fafb; border-radius: 6px;">
                                                ${orden.tipoExamen}
                                            </p>
                                        </div>
                                        <div class="details-section">
                                            <h4 class="details-section-title">Fecha de Solicitud</h4>
                                            <p style="padding: 12px; background: #f9fafb; border-radius: 6px;">
                                                ${fecha}
                                            </p>
                                        </div>
                                        <div class="details-section">
                                            <h4 class="details-section-title">Observaciones</h4>
                                            <p style="padding: 12px; background: #f9fafb; border-radius: 6px; line-height: 1.6;">
                                                ${orden.observacionesAdicionales || 'Sin observaciones'}
                                            </p>
                                        </div>
                                        ${orden.resultados ? `
                                            <div class="details-section">
                                                <h4 class="details-section-title">Resultados</h4>
                                                <p style="padding: 12px; background: #f9fafb; border-radius: 6px; line-height: 1.6;">
                                                    ${orden.resultados}
                                                </p>
                                            </div>
                                        ` : ''}
                                    </div>
                                </div>
                            </td>
                        </tr>
                    `;
                }).join('')}
            </tbody>
        </table>
    `;

    container.innerHTML = tabla;
}

function toggleDetalles(index) {
    const detalleRow = document.getElementById(`detalles-${index}`);
    if (detalleRow.style.display === 'none') {
        document.querySelectorAll('[id^="detalles-"]').forEach(row => {
            row.style.display = 'none';
        });
        detalleRow.style.display = 'table-row';
    } else {
        detalleRow.style.display = 'none';
    }
}

function verDetalles(index) {
    toggleDetalles(index);
}

function crearNuevaOrden() {
    window.location.href = `/laboratorio/nueva-orden?idPaciente=${idPacienteActual}`;
}