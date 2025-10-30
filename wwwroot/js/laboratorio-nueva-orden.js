const API_BASE_URL = "http://localhost:5122";
let idPacienteActual = null;

document.addEventListener('DOMContentLoaded', function() {
    obtenerParametrosURL();
    cargarInformacionPaciente();
});

function obtenerParametrosURL() {
    const params = new URLSearchParams(window.location.search);
    idPacienteActual = params.get('idPaciente');
    
    if (!idPacienteActual) {
        showError('No se especificó el ID del paciente');
        setTimeout(() => window.history.back(), 2000);
    }
}

async function cargarInformacionPaciente() {
    try {
        const res = await fetch(`${API_BASE_URL}/laboratorio/api/historial/${idPacienteActual}`);
        const result = await res.json();

        if (result.success && result.paciente) {
            mostrarInformacionPaciente(result.paciente);
        } else {
            showError('No se pudo cargar la información del paciente');
        }
    } catch (error) {
        console.error('Error:', error);
        showError('Error al cargar datos del paciente');
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

async function guardarOrden(event) {
    event.preventDefault();

    const tipoExamen = document.getElementById('tipoExamen').value;
    const observaciones = document.getElementById('observaciones').value.trim();

    if (!tipoExamen) {
        showWarning('Por favor seleccione un tipo de examen');
        return;
    }

    const ordenDTO = {
        idPaciente: parseInt(idPacienteActual),
        idMedico: 1, // TODO: Obtener del contexto del usuario logueado
        tipoExamen: tipoExamen,
        observacionesAdicionales: observaciones,
        estado: 'Pendiente' // Estado por defecto
    };

    try {
        const res = await fetch(`${API_BASE_URL}/laboratorio/api/crear`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(ordenDTO)
        });

        const result = await res.json();

        if (result.success) {
            showSuccess('Orden de laboratorio creada exitosamente');
            setTimeout(() => {
                window.location.href = `/laboratorio?idPaciente=${idPacienteActual}`;
            }, 1500);
        } else {
            showError(result.message || 'Error al crear la orden');
        }
    } catch (error) {
        console.error('Error:', error);
        showError('Error al conectar con el servidor');
    }
}

function volver() {
    if (confirm('¿Está seguro de que desea salir? Los datos no guardados se perderán.')) {
        window.location.href = `/laboratorio?idPaciente=${idPacienteActual}`;
    }
}