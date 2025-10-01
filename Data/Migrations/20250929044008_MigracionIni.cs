using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGMG.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracionIni : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Medicos_MedicoIdMedico",
                table: "Citas");

            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Pacientes_PacienteIdPaciente",
                table: "Citas");

            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Triages_TriageIdTriage",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_MedicoIdMedico",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_PacienteIdPaciente",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_TriageIdTriage",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "MedicoIdMedico",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "PacienteIdPaciente",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "TriageIdTriage",
                table: "Citas");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_IdMedico",
                table: "Citas",
                column: "IdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_IdPaciente",
                table: "Citas",
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_IdTriage",
                table: "Citas",
                column: "IdTriage");

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Medicos_IdMedico",
                table: "Citas",
                column: "IdMedico",
                principalTable: "Medicos",
                principalColumn: "IdMedico",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Pacientes_IdPaciente",
                table: "Citas",
                column: "IdPaciente",
                principalTable: "Pacientes",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Triages_IdTriage",
                table: "Citas",
                column: "IdTriage",
                principalTable: "Triages",
                principalColumn: "IdTriage",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Medicos_IdMedico",
                table: "Citas");

            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Pacientes_IdPaciente",
                table: "Citas");

            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Triages_IdTriage",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_IdMedico",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_IdPaciente",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_IdTriage",
                table: "Citas");

            migrationBuilder.AddColumn<int>(
                name: "MedicoIdMedico",
                table: "Citas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PacienteIdPaciente",
                table: "Citas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TriageIdTriage",
                table: "Citas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Medicos_MedicoIdMedico",
                table: "Citas",
                column: "MedicoIdMedico",
                principalTable: "Medicos",
                principalColumn: "IdMedico",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Pacientes_PacienteIdPaciente",
                table: "Citas",
                column: "PacienteIdPaciente",
                principalTable: "Pacientes",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Triages_TriageIdTriage",
                table: "Citas",
                column: "TriageIdTriage",
                principalTable: "Triages",
                principalColumn: "IdTriage",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
