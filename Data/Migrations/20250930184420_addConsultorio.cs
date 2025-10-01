using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGMG.Data.Migrations
{
    /// <inheritdoc />
    public partial class addConsultorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsultorioAsignado",
                table: "Medicos");

            migrationBuilder.AddColumn<int>(
                name: "IdConsultorio",
                table: "Medicos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdConsultorio",
                table: "Enfermerias",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Consultorios",
                columns: table => new
                {
                    IdConsultorio = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: true),
                    estado = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultorios", x => x.IdConsultorio);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_IdConsultorio",
                table: "Medicos",
                column: "IdConsultorio");

            migrationBuilder.CreateIndex(
                name: "IX_Enfermerias_IdConsultorio",
                table: "Enfermerias",
                column: "IdConsultorio");

            migrationBuilder.AddForeignKey(
                name: "FK_Enfermerias_Consultorios_IdConsultorio",
                table: "Enfermerias",
                column: "IdConsultorio",
                principalTable: "Consultorios",
                principalColumn: "IdConsultorio");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicos_Consultorios_IdConsultorio",
                table: "Medicos",
                column: "IdConsultorio",
                principalTable: "Consultorios",
                principalColumn: "IdConsultorio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enfermerias_Consultorios_IdConsultorio",
                table: "Enfermerias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicos_Consultorios_IdConsultorio",
                table: "Medicos");

            migrationBuilder.DropTable(
                name: "Consultorios");

            migrationBuilder.DropIndex(
                name: "IX_Medicos_IdConsultorio",
                table: "Medicos");

            migrationBuilder.DropIndex(
                name: "IX_Enfermerias_IdConsultorio",
                table: "Enfermerias");

            migrationBuilder.DropColumn(
                name: "IdConsultorio",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "IdConsultorio",
                table: "Enfermerias");

            migrationBuilder.AddColumn<string>(
                name: "ConsultorioAsignado",
                table: "Medicos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
