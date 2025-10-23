const API_BASE_URL = "http://localhost:5122";
let tabActual = "por-triar";
let pacientesData = [];

document.addEventListener("DOMContentLoaded", function() {
    console.log("Página cargada, iniciando..."); // DEBUG
    cargarPacientesPorTriar();
});

// Cambiar de tab
function cambiarTab(tab) {
    console.log("Cambiando a tab:", tab); // DEBUG
    tabActual = tab;
    
    // Actualizar estilos de tabs
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));
    document.getElementById(`tab-${tab}`).classList.add('active');

    // Mostrar/ocultar campos de fecha según el tab
    const fechaInicioGroup = document.getElementById('fechaInicioGroup');
    const fechaFinGroup = document.getElementById('fechaFinGroup');
    const columnAction = document.getElementById('columnAction');

    if (tab === 'triados') {
        fechaInicioGroup.style.display = 'flex';
        fechaFinGroup.style.display = 'flex';
        columnAction.textContent = 'Acción';
    } else {
        fechaInicioGroup.style.display = 'none';
        fechaFinGroup.style.display = 'none';
        columnAction.textContent = 'Acción';
    }

    // Cargar datos según el tab
    if (tab === 'por-triar') {
        cargarPacientesPorTriar();
    } else if (tab === 'fuera-horario') {
        cargarPacientesFueraHorario();
    } else if (tab === 'triados') {
        cargarPacientesTriados();
    }
}

// Buscar pacientes
function buscarPacientes(e) {
    e.preventDefault();
    console.log("Buscando pacientes en tab:", tabActual); // DEBUG
    cambiarTab(tabActual);
}

// Cargar pacientes por triar
async function cargarPacientesPorTriar() {
    console.log("Llamando a /citas/pendientes..."); // DEBUG
    try {
        const res = await fetch(`${API_BASE_URL}/citas/pendientes`);
        console.log("Response status:", res.status); // DEBUG
        
        const result = await res.json();
        console.log("Resultado completo:", result); // DEBUG
        console.log("Success:", result.success); // DEBUG
        console.log("Data:", result.data); // DEBUG

        if (result.success && result.data) {
            console.log("Total registros:", result.data.length); // DEBUG
            // Ver estructura del primer elemento
            if (result.data.length > 0) {
                console.log("Primer elemento:", result.data[0]); // DEBUG
            }
            renderizarTabla(result.data, 'triar');
        } else {
            console.warn("No hay datos o error:", result.message); // DEBUG
            renderizarTabla([], 'triar');
        }
    } catch (error) {
        console.error("Error al cargar citas pendientes:", error); // DEBUG
        renderizarTabla([], 'triar');
    }
}

// Cargar pacientes fuera de horario
async function cargarPacientesFueraHorario() {
    console.log("Llamando a /citas/fuera-horario..."); // DEBUG
    try {
        const res = await fetch(`${API_BASE_URL}/citas/fuera-horario`);
        console.log("Response status:", res.status); // DEBUG
        
        const result = await res.json();
        console.log("Resultado completo:", result); // DEBUG

        if (result.success && result.data) {
            console.log("Total registros:", result.data.length); // DEBUG
            renderizarTabla(result.data, 'triar');
        } else {
            console.warn("No hay datos o error:", result.message); // DEBUG
            renderizarTabla([], 'triar');
        }
    } catch (error) {
        console.error("Error al cargar citas fuera de horario:", error); // DEBUG
        renderizarTabla([], 'triar');
    }
}

// Cargar pacientes ya triados
async function cargarPacientesTriados() {
    console.log("Llamando a /triaje/all..."); // DEBUG
    try {
        const res = await fetch(`${API_BASE_URL}/triaje/all`);
        console.log("Response status:", res.status); // DEBUG
        
        const result = await res.json();
        console.log("Resultado completo:", result); // DEBUG

        if (result.success && result.data) {
            console.log("Total registros:", result.data.length); // DEBUG
            if (result.data.length > 0) {
                console.log("Primer triaje:", result.data[0]); // DEBUG
            }
            renderizarTabla(result.data, 'editar');
        } else {
            console.warn("No hay datos o error:", result.message); // DEBUG
            renderizarTabla([], 'editar');
        }
    } catch (error) {
        console.error("Error al cargar triajes:", error); // DEBUG
        renderizarTabla([], 'editar');
    }
}

// Renderizar tabla
function renderizarTabla(data, tipo) {
    console.log("Renderizando:", data?.length, "registros, tipo:", tipo);
    const tbody = document.getElementById('tablaPacientesBody');
    
    if (!data || data.length === 0) {
        tbody.innerHTML = '<tr><td colspan="7" class="no-data">No hay datos disponibles</td></tr>';
        return;
    }

    if (tipo === 'editar') {
        // Renderizar triajes (pacientes ya triados) - Usando DTO
        tbody.innerHTML = data.map(item => {
            const documento = item.numeroDocumento || '';
            const nombre = item.nombreCompletoPaciente || '';
            const consultorio = item.consultorio || '';
            const horaCita = item.horaCita || '';
            const fechaCita = item.fechaCita || '';
            const nombreMedico = item.nombreCompletoMedico || '';
            const idPaciente = item.idPaciente;
            const idTriaje = item.idTriaje;

            const fechaFormateada = fechaCita ? new Date(fechaCita).toLocaleDateString('es-PE') : '-';

            return `
                <tr>
                    <td>${documento}</td>
                    <td>${nombre}</td>
                    <td>${consultorio}</td>
                    <td>${horaCita}</td>
                    <td>${fechaFormateada}</td>
                    <td>${nombreMedico}</td>
                    <td><button class="btn-action btn-editar" onclick="irAEditarTriaje(${idTriaje}, ${idPaciente})">Editar</button></td>
                </tr>
            `;
        }).join('');
    } else {
        // Renderizar citas (pacientes por triar) - Ya está con DTO
        tbody.innerHTML = data.map(item => {
            const documento = item.numeroDocumento || '';
            const nombre = item.nombreCompletoPaciente || '';
            const consultorio = item.consultorio || '';
            const horaCita = item.horaCita || '';
            const fechaCita = item.fechaCita || '';
            const nombreMedico = item.nombreCompletoMedico || '';
            const idPaciente = item.idPaciente;

            const fechaFormateada = fechaCita ? new Date(fechaCita).toLocaleDateString('es-PE') : '-';

            return `
                <tr>
                    <td>${documento}</td>
                    <td>${nombre}</td>
                    <td>${consultorio}</td>
                    <td>${horaCita}</td>
                    <td>${fechaFormateada}</td>
                    <td>${nombreMedico}</td>
                    <td><button class="btn-action btn-triar" onclick="irARegistrarTriaje(${idPaciente})">Triar</button></td>
                </tr>
            `;
        }).join('');
    }
}


// Ir a registrar triaje
function irARegistrarTriaje(idPaciente) {
    console.log("Redirigiendo a registrar triaje, paciente:", idPaciente); // DEBUG
    window.location.href = `/triaje/registrar?idPaciente=${idPaciente}`;
}

// Ir a editar triaje
function irAEditarTriaje(idTriaje, idPaciente) {
    console.log("Redirigiendo a editar triaje:", idTriaje, "paciente:", idPaciente); // DEBUG
    window.location.href = `/triaje/editar?idTriaje=${idTriaje}&idPaciente=${idPaciente}`;
}

// Paginación (por implementar)
function cambiarPagina(direccion) {
    console.log("Cambiar página:", direccion); // DEBUG
}



