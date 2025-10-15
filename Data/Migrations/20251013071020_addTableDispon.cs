using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGMG.Data.Migrations
{
    /// <inheritdoc />
    public partial class addTableDispon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisponibilidadesSemanales",
                columns: table => new
                {
                    IdDisponibilidad = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdMedico = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaInicioSemana = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFinSemana = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CitasActuales = table.Column<int>(type: "INTEGER", nullable: false),
                    CitasMaximas = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisponibilidadesSemanales", x => x.IdDisponibilidad);
                    table.ForeignKey(
                        name: "FK_DisponibilidadesSemanales_Medicos_IdMedico",
                        column: x => x.IdMedico,
                        principalTable: "Medicos",
                        principalColumn: "IdMedico",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisponibilidadesSemanales_IdMedico",
                table: "DisponibilidadesSemanales",
                column: "IdMedico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisponibilidadesSemanales");
        }
    }
}
