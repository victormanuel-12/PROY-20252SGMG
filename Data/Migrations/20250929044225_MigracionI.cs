using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGMG.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracionI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisponibilidadesMedicos_Medicos_MedicoIdMedico",
                table: "DisponibilidadesMedicos");

            migrationBuilder.DropForeignKey(
                name: "FK_DomiciliosPacientes_Pacientes_PacienteIdPaciente",
                table: "DomiciliosPacientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Enfermerias_PersonalTecnicos_PersonalIdPersonal",
                table: "Enfermerias");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoriasClinicas_Pacientes_PacienteIdPaciente",
                table: "HistoriasClinicas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Citas_CitaIdCita",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Triages_Pacientes_PacienteIdPaciente",
                table: "Triages");

            migrationBuilder.DropIndex(
                name: "IX_Triages_PacienteIdPaciente",
                table: "Triages");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_CitaIdCita",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_HistoriasClinicas_PacienteIdPaciente",
                table: "HistoriasClinicas");

            migrationBuilder.DropIndex(
                name: "IX_Enfermerias_PersonalIdPersonal",
                table: "Enfermerias");

            migrationBuilder.DropIndex(
                name: "IX_DomiciliosPacientes_PacienteIdPaciente",
                table: "DomiciliosPacientes");

            migrationBuilder.DropIndex(
                name: "IX_DisponibilidadesMedicos_MedicoIdMedico",
                table: "DisponibilidadesMedicos");

            migrationBuilder.DropColumn(
                name: "PacienteIdPaciente",
                table: "Triages");

            migrationBuilder.DropColumn(
                name: "CitaIdCita",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "PacienteIdPaciente",
                table: "HistoriasClinicas");

            migrationBuilder.DropColumn(
                name: "PersonalIdPersonal",
                table: "Enfermerias");

            migrationBuilder.DropColumn(
                name: "PacienteIdPaciente",
                table: "DomiciliosPacientes");

            migrationBuilder.DropColumn(
                name: "MedicoIdMedico",
                table: "DisponibilidadesMedicos");

            migrationBuilder.CreateIndex(
                name: "IX_Triages_IdPaciente",
                table: "Triages",
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdCita",
                table: "Pagos",
                column: "IdCita");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriasClinicas_IdPaciente",
                table: "HistoriasClinicas",
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_Enfermerias_IdPersonal",
                table: "Enfermerias",
                column: "IdPersonal");

            migrationBuilder.CreateIndex(
                name: "IX_DomiciliosPacientes_IdPaciente",
                table: "DomiciliosPacientes",
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_DisponibilidadesMedicos_IdMedico",
                table: "DisponibilidadesMedicos",
                column: "IdMedico");

            migrationBuilder.AddForeignKey(
                name: "FK_DisponibilidadesMedicos_Medicos_IdMedico",
                table: "DisponibilidadesMedicos",
                column: "IdMedico",
                principalTable: "Medicos",
                principalColumn: "IdMedico",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DomiciliosPacientes_Pacientes_IdPaciente",
                table: "DomiciliosPacientes",
                column: "IdPaciente",
                principalTable: "Pacientes",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enfermerias_PersonalTecnicos_IdPersonal",
                table: "Enfermerias",
                column: "IdPersonal",
                principalTable: "PersonalTecnicos",
                principalColumn: "IdPersonal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriasClinicas_Pacientes_IdPaciente",
                table: "HistoriasClinicas",
                column: "IdPaciente",
                principalTable: "Pacientes",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Citas_IdCita",
                table: "Pagos",
                column: "IdCita",
                principalTable: "Citas",
                principalColumn: "IdCita",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Triages_Pacientes_IdPaciente",
                table: "Triages",
                column: "IdPaciente",
                principalTable: "Pacientes",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisponibilidadesMedicos_Medicos_IdMedico",
                table: "DisponibilidadesMedicos");

            migrationBuilder.DropForeignKey(
                name: "FK_DomiciliosPacientes_Pacientes_IdPaciente",
                table: "DomiciliosPacientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Enfermerias_PersonalTecnicos_IdPersonal",
                table: "Enfermerias");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoriasClinicas_Pacientes_IdPaciente",
                table: "HistoriasClinicas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Citas_IdCita",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Triages_Pacientes_IdPaciente",
                table: "Triages");

            migrationBuilder.DropIndex(
                name: "IX_Triages_IdPaciente",
                table: "Triages");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_IdCita",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_HistoriasClinicas_IdPaciente",
                table: "HistoriasClinicas");

            migrationBuilder.DropIndex(
                name: "IX_Enfermerias_IdPersonal",
                table: "Enfermerias");

            migrationBuilder.DropIndex(
                name: "IX_DomiciliosPacientes_IdPaciente",
                table: "DomiciliosPacientes");

            migrationBuilder.DropIndex(
                name: "IX_DisponibilidadesMedicos_IdMedico",
                table: "DisponibilidadesMedicos");

            migrationBuilder.AddColumn<int>(
                name: "PacienteIdPaciente",
                table: "Triages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CitaIdCita",
                table: "Pagos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PacienteIdPaciente",
                table: "HistoriasClinicas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PersonalIdPersonal",
                table: "Enfermerias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PacienteIdPaciente",
                table: "DomiciliosPacientes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MedicoIdMedico",
                table: "DisponibilidadesMedicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Triages_PacienteIdPaciente",
                table: "Triages",
                column: "PacienteIdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_CitaIdCita",
                table: "Pagos",
                column: "CitaIdCita");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriasClinicas_PacienteIdPaciente",
                table: "HistoriasClinicas",
                column: "PacienteIdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_Enfermerias_PersonalIdPersonal",
                table: "Enfermerias",
                column: "PersonalIdPersonal");

            migrationBuilder.CreateIndex(
                name: "IX_DomiciliosPacientes_PacienteIdPaciente",
                table: "DomiciliosPacientes",
                column: "PacienteIdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_DisponibilidadesMedicos_MedicoIdMedico",
                table: "DisponibilidadesMedicos",
                column: "MedicoIdMedico");

            migrationBuilder.AddForeignKey(
                name: "FK_DisponibilidadesMedicos_Medicos_MedicoIdMedico",
                table: "DisponibilidadesMedicos",
                column: "MedicoIdMedico",
                principalTable: "Medicos",
                principalColumn: "IdMedico",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DomiciliosPacientes_Pacientes_PacienteIdPaciente",
                table: "DomiciliosPacientes",
                column: "PacienteIdPaciente",
                principalTable: "Pacientes",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enfermerias_PersonalTecnicos_PersonalIdPersonal",
                table: "Enfermerias",
                column: "PersonalIdPersonal",
                principalTable: "PersonalTecnicos",
                principalColumn: "IdPersonal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriasClinicas_Pacientes_PacienteIdPaciente",
                table: "HistoriasClinicas",
                column: "PacienteIdPaciente",
                principalTable: "Pacientes",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Citas_CitaIdCita",
                table: "Pagos",
                column: "CitaIdCita",
                principalTable: "Citas",
                principalColumn: "IdCita",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Triages_Pacientes_PacienteIdPaciente",
                table: "Triages",
                column: "PacienteIdPaciente",
                principalTable: "Pacientes",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
