const API_BASE_URL = "http://localhost:5122";

document.addEventListener('DOMContentLoaded', () => {
  const buscarBtn = document.getElementById('buscarMedicosBtn');
  const limpiarBtn = document.getElementById('limpiarFiltrosBtn');
  if (buscarBtn) buscarBtn.addEventListener('click', onBuscarMedicos);
  if (limpiarBtn) limpiarBtn.addEventListener('click', onLimpiarFiltros);
});

async function onBuscarMedicos(e) {
  e && e.preventDefault && e.preventDefault();
  const numeroDni = document.getElementById('fNumeroDni')?.value || '';
  const consultorio = document.getElementById('fConsultorio')?.value || '';
  const estado = document.getElementById('fEstado')?.value || '';
  const fechaInicio = document.getElementById('fFechaInicio')?.value || '';
  const fechaFin = document.getElementById('fFechaFin')?.value || '';
  const turno = document.getElementById('fTurno')?.value || '';

  const params = new URLSearchParams();
  if (numeroDni) params.append('numeroDni', numeroDni);
  if (consultorio) params.append('idConsultorio', consultorio);
  if (estado) params.append('estado', estado);
  if (fechaInicio) params.append('fechaInicio', fechaInicio);
  if (fechaFin) params.append('fechaFin', fechaFin);
  if (turno) params.append('turno', turno);

  try {
    const res = await fetch(`${API_BASE_URL}/medicos/filters?${params.toString()}`);
    const result = await res.json();
    if (!result.success || !result.data) {
      renderMedicosTable([]);
      return;
    }
    renderMedicosTable(result.data);
  } catch (err) {
    console.error(err);
    renderMedicosTable([]);
  }
}

function onLimpiarFiltros() {
  ['fNumeroDni','fConsultorio','fEstado','fFechaInicio','fFechaFin','fTurno'].forEach(id => {
    const el = document.getElementById(id); if (el) el.value = '';
  });
  renderMedicosTable([]);
}

function renderMedicosTable(medicos) {
  const tbody = document.getElementById('medicosTableBody');
  if (!tbody) return;
  if (!medicos || medicos.length === 0) {
    tbody.innerHTML = `<tr><td colspan="4" class="no-data">No hay médicos.</td></tr>`;
    return;
  }

  tbody.innerHTML = medicos.map(m => `
    <tr>
      <td>${m.numeroDni || m.NumeroDni || ''}</td>
      <td>${(m.Nombre || m.nombre || '') + ' ' + (m.ApellidoPaterno || m.apellidoPaterno || '')}</td>
      <td>${m.ConsultorioAsignado?.Nombre || m.consultorioAsignado?.nombre || ''}</td>
      <td><button class="btn btn-success" onclick="verHorarios(${m.idMedico || m.IdMedico})">Ver horarios</button></td>
    </tr>
  `).join('');
}

function verHorarios(idMedico) {
  alert('Implementar vista de horarios para medico ID: ' + idMedico);
}
