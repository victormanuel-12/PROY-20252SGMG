using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGMG.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicos",
                columns: table => new
                {
                    IdMedico = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroDni = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "TEXT", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "TEXT", nullable: false),
                    Sexo = table.Column<string>(type: "TEXT", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", nullable: false),
                    CorreoElectronico = table.Column<string>(type: "TEXT", nullable: false),
                    EstadoLaboral = table.Column<string>(type: "TEXT", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Turno = table.Column<string>(type: "TEXT", nullable: false),
                    AreaServicio = table.Column<string>(type: "TEXT", nullable: false),
                    CargoMedico = table.Column<string>(type: "TEXT", nullable: false),
                    NumeroColegiatura = table.Column<string>(type: "TEXT", nullable: false),
                    TipoMedico = table.Column<string>(type: "TEXT", nullable: false),
                    ConsultorioAsignado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicos", x => x.IdMedico);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    IdPaciente = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroDocumento = table.Column<string>(type: "TEXT", nullable: false),
                    TipoDocumento = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "TEXT", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "TEXT", nullable: false),
                    Sexo = table.Column<string>(type: "TEXT", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.IdPaciente);
                });

            migrationBuilder.CreateTable(
                name: "PersonalTecnicos",
                columns: table => new
                {
                    IdPersonal = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroDni = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "TEXT", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "TEXT", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Sexo = table.Column<string>(type: "TEXT", nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    EstadoLaboral = table.Column<string>(type: "TEXT", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Turno = table.Column<string>(type: "TEXT", nullable: false),
                    AreaServicio = table.Column<string>(type: "TEXT", nullable: false),
                    Cargo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalTecnicos", x => x.IdPersonal);
                });

            migrationBuilder.CreateTable(
                name: "DisponibilidadesMedicos",
                columns: table => new
                {
                    IdDisponibilidad = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdMedico = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false),
                    MedicoIdMedico = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisponibilidadesMedicos", x => x.IdDisponibilidad);
                    table.ForeignKey(
                        name: "FK_DisponibilidadesMedicos_Medicos_MedicoIdMedico",
                        column: x => x.MedicoIdMedico,
                        principalTable: "Medicos",
                        principalColumn: "IdMedico",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DomiciliosPacientes",
                columns: table => new
                {
                    IdDomicilio = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdPaciente = table.Column<int>(type: "INTEGER", nullable: false),
                    Departamento = table.Column<string>(type: "TEXT", nullable: false),
                    Provincia = table.Column<string>(type: "TEXT", nullable: false),
                    Distrito = table.Column<string>(type: "TEXT", nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", nullable: false),
                    Referencia = table.Column<string>(type: "TEXT", nullable: false),
                    PacienteIdPaciente = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomiciliosPacientes", x => x.IdDomicilio);
                    table.ForeignKey(
                        name: "FK_DomiciliosPacientes_Pacientes_PacienteIdPaciente",
                        column: x => x.PacienteIdPaciente,
                        principalTable: "Pacientes",
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoriasClinicas",
                columns: table => new
                {
                    IdHistoria = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdPaciente = table.Column<int>(type: "INTEGER", nullable: false),
                    CodigoHistoria = table.Column<string>(type: "TEXT", nullable: false),
                    TipoSeguro = table.Column<string>(type: "TEXT", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstadoCivil = table.Column<string>(type: "TEXT", nullable: false),
                    TipoSangre = table.Column<string>(type: "TEXT", nullable: false),
                    PacienteIdPaciente = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriasClinicas", x => x.IdHistoria);
                    table.ForeignKey(
                        name: "FK_HistoriasClinicas_Pacientes_PacienteIdPaciente",
                        column: x => x.PacienteIdPaciente,
                        principalTable: "Pacientes",
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Triages",
                columns: table => new
                {
                    IdTriage = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdPaciente = table.Column<int>(type: "INTEGER", nullable: false),
                    Temperatura = table.Column<decimal>(type: "TEXT", nullable: false),
                    PresionArterial = table.Column<int>(type: "INTEGER", nullable: false),
                    Saturacion = table.Column<int>(type: "INTEGER", nullable: false),
                    FrecuenciaCardiaca = table.Column<int>(type: "INTEGER", nullable: false),
                    FrecuenciaRespiratoria = table.Column<int>(type: "INTEGER", nullable: false),
                    Peso = table.Column<decimal>(type: "TEXT", nullable: false),
                    Talla = table.Column<decimal>(type: "TEXT", nullable: false),
                    PerimetroAbdominal = table.Column<decimal>(type: "TEXT", nullable: false),
                    SuperficieCorporal = table.Column<decimal>(type: "TEXT", nullable: false),
                    Imc = table.Column<decimal>(type: "TEXT", nullable: false),
                    ClasificacionImc = table.Column<string>(type: "TEXT", nullable: false),
                    RiesgoEnfermedad = table.Column<string>(type: "TEXT", nullable: false),
                    FechaTriage = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HoraTriage = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: false),
                    EstadoTriage = table.Column<string>(type: "TEXT", nullable: false),
                    PacienteIdPaciente = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triages", x => x.IdTriage);
                    table.ForeignKey(
                        name: "FK_Triages_Pacientes_PacienteIdPaciente",
                        column: x => x.PacienteIdPaciente,
                        principalTable: "Pacientes",
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enfermerias",
                columns: table => new
                {
                    IdEnfermeria = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdPersonal = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroColegiaturaEnfermeria = table.Column<string>(type: "TEXT", nullable: false),
                    NivelProfesional = table.Column<string>(type: "TEXT", nullable: false),
                    PersonalIdPersonal = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enfermerias", x => x.IdEnfermeria);
                    table.ForeignKey(
                        name: "FK_Enfermerias_PersonalTecnicos_PersonalIdPersonal",
                        column: x => x.PersonalIdPersonal,
                        principalTable: "PersonalTecnicos",
                        principalColumn: "IdPersonal",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    IdCita = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Especialidad = table.Column<string>(type: "TEXT", nullable: false),
                    FechaCita = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HoraCita = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Consultorio = table.Column<string>(type: "TEXT", nullable: false),
                    EstadoCita = table.Column<string>(type: "TEXT", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PacienteIdPaciente = table.Column<int>(type: "INTEGER", nullable: false),
                    MedicoIdMedico = table.Column<int>(type: "INTEGER", nullable: false),
                    TriageIdTriage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.IdCita);
                    table.ForeignKey(
                        name: "FK_Citas_Medicos_MedicoIdMedico",
                        column: x => x.MedicoIdMedico,
                        principalTable: "Medicos",
                        principalColumn: "IdMedico",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Citas_Pacientes_PacienteIdPaciente",
                        column: x => x.PacienteIdPaciente,
                        principalTable: "Pacientes",
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Citas_Triages_TriageIdTriage",
                        column: x => x.TriageIdTriage,
                        principalTable: "Triages",
                        principalColumn: "IdTriage",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    IdPago = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCita = table.Column<int>(type: "INTEGER", nullable: false),
                    CodigoServicio = table.Column<string>(type: "TEXT", nullable: false),
                    DescripcionServicio = table.Column<string>(type: "TEXT", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "TEXT", nullable: false),
                    Subtotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    Igv = table.Column<decimal>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false),
                    EstadoPago = table.Column<string>(type: "TEXT", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CitaIdCita = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.IdPago);
                    table.ForeignKey(
                        name: "FK_Pagos_Citas_CitaIdCita",
                        column: x => x.CitaIdCita,
                        principalTable: "Citas",
                        principalColumn: "IdCita",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoIdMedico",
                table: "Citas",
                column: "MedicoIdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_PacienteIdPaciente",
                table: "Citas",
                column: "PacienteIdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_TriageIdTriage",
                table: "Citas",
                column: "TriageIdTriage");

            migrationBuilder.CreateIndex(
                name: "IX_DisponibilidadesMedicos_MedicoIdMedico",
                table: "DisponibilidadesMedicos",
                column: "MedicoIdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_DomiciliosPacientes_PacienteIdPaciente",
                table: "DomiciliosPacientes",
                column: "PacienteIdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_Enfermerias_PersonalIdPersonal",
                table: "Enfermerias",
                column: "PersonalIdPersonal");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriasClinicas_PacienteIdPaciente",
                table: "HistoriasClinicas",
                column: "PacienteIdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_CitaIdCita",
                table: "Pagos",
                column: "CitaIdCita");

            migrationBuilder.CreateIndex(
                name: "IX_Triages_PacienteIdPaciente",
                table: "Triages",
                column: "PacienteIdPaciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisponibilidadesMedicos");

            migrationBuilder.DropTable(
                name: "DomiciliosPacientes");

            migrationBuilder.DropTable(
                name: "Enfermerias");

            migrationBuilder.DropTable(
                name: "HistoriasClinicas");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "PersonalTecnicos");

            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "Medicos");

            migrationBuilder.DropTable(
                name: "Triages");

            migrationBuilder.DropTable(
                name: "Pacientes");
        }
    }
}
