  const API_BASE_URL = "http://localhost:5122";
    
    // Inicializar cuando cargue la página
    document.addEventListener("DOMContentLoaded", function() {
        cargarDatosPaciente();
        document.getElementById("pesoTriaje").addEventListener("change", calcularIMC);
        document.getElementById("tallaTriaje").addEventListener("change", calcularIMC);
        document.getElementById("riesgoEnfermedad").addEventListener("change", mostrarRiesgoInfo);
    });

    // Cargar datos del paciente desde parámetros
    function cargarDatosPaciente() {
        const params = new URLSearchParams(window.location.search);
        const idPaciente = params.get("idPaciente");
        
        if (!idPaciente) {
            alert("No se especificó el paciente");
            window.history.back();
            return;
        }

        document.getElementById("idPaciente").value = idPaciente;
        obtenerDatosPaciente(idPaciente);
    }

    // Obtener datos del paciente desde la API
    async function obtenerDatosPaciente(idPaciente) {
        try {
            const res = await fetch(`${API_BASE_URL}/pacientes/${idPaciente}`);
            const result = await res.json();

            if (result.success && result.data) {
                const paciente = result.data;
                mostrarDatosPaciente(paciente);
            } else {
                alert("No se pudo cargar los datos del paciente");
            }
        } catch (error) {
            console.error("Error:", error);
            alert("Error al cargar los datos del paciente");
        }
    }

    // Mostrar datos del paciente en la vista
    function mostrarDatosPaciente(paciente) {
        const nombre = `${paciente.apellidoPaterno || paciente.ApellidoPaterno || ""} ${paciente.apellidoMaterno || paciente.ApellidoMaterno || ""}, ${paciente.nombre || paciente.Nombre || ""}`;
        
        document.getElementById("pacienteName").textContent = nombre;
        document.getElementById("pacienteDocumento").textContent = paciente.numeroDocumento || paciente.NumeroDocumento || "-";
        document.getElementById("pacienteSexo").textContent = (paciente.sexo === "M" || paciente.Sexo === "M") ? "Masculino" : "Femenino";
        document.getElementById("pacienteEdad").textContent = (paciente.edad || paciente.Edad) + " años";
        document.getElementById("pacienteTipo").textContent = paciente.tipoDocumento || paciente.TipoDocumento || "-";
    }

    // Calcular IMC y clasificación
    function calcularIMC() {
        const peso = parseFloat(document.getElementById("pesoTriaje").value);
        const talla = parseFloat(document.getElementById("tallaTriaje").value) / 100; // Convertir cm a m

        if (peso && talla) {
            const imc = (peso / (talla * talla)).toFixed(2);
            document.getElementById("imc").value = imc;

            // Determinar clasificación de IMC
            let clasificacion = "";
            if (imc < 18.5) clasificacion = "Bajo peso";
            else if (imc >= 18.5 && imc < 25) clasificacion = "Peso normal";
            else if (imc >= 25 && imc < 30) clasificacion = "Sobrepeso";
            else if (imc >= 30 && imc < 35) clasificacion = "Obesidad I";
            else if (imc >= 35) clasificacion = "Obesidad II";

            document.getElementById("clasificacionImc").value = clasificacion;
            document.getElementById("imcInfo").innerHTML = `<strong>IMC: ${imc}</strong> - ${clasificacion}`;
        }
    }

    // Mostrar información del riesgo
    function mostrarRiesgoInfo() {
        const riesgo = document.getElementById("riesgoEnfermedad").value;
        const riesgoInfo = document.getElementById("riesgoInfo");
        
        if (!riesgo) {
            riesgoInfo.style.display = "none";
            return;
        }

        riesgoInfo.style.display = "block";
        riesgoInfo.innerHTML = `Riesgo de enfermedad seleccionado: <strong>${riesgo}</strong>`;
    }

    // Guardar triaje
    async function guardarTriaje(e) {
        e.preventDefault();

        const formData = {
            idPaciente: parseInt(document.getElementById("idPaciente").value),
            temperatura: parseFloat(document.getElementById("temperatura").value),
            presionArterial: parseInt(document.getElementById("presionArterial").value),
            saturacion: parseInt(document.getElementById("saturacion").value),
            frecuenciaCardiaca: parseInt(document.getElementById("frecuenciaCardiaca").value),
            frecuenciaRespiratoria: parseInt(document.getElementById("frecuenciaRespiratoria").value),
            peso: parseFloat(document.getElementById("pesoTriaje").value),
            talla: parseFloat(document.getElementById("tallaTriaje").value),
            perimAbdominal: parseFloat(document.getElementById("perimAbdominal").value),
            superficieCorporal: parseFloat(document.getElementById("superficieCorporal").value),
            imc: parseFloat(document.getElementById("imc").value),
            clasificacionImc: document.getElementById("clasificacionImc").value,
            riesgoEnfermedad: document.getElementById("riesgoEnfermedad").value,
            estadoTriage: document.getElementById("estadoTriage").value,
            observaciones: document.getElementById("observaciones").value
        };

        try {
            const res = await fetch(`${API_BASE_URL}/triaje/register`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            });

            const result = await res.json();

            if (result.success) {
                alert("Triaje registrado correctamente");
                window.history.back();
            } else {
                alert(result.message || "Error al registrar el triaje");
            }
        } catch (error) {
            console.error("Error:", error);
            alert("Error al guardar el triaje");
        }
    }

    // Cancelar
    function cancelarTriaje() {
        if (confirm("¿Estás seguro de que deseas cancelar?")) {
            window.history.back();
        }
    }

