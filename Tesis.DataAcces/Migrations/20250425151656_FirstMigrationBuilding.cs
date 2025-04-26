using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tesis.DataAcces.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigrationBuilding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Objetivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Evaluacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objetivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Procesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Evaluacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procesos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Indicadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MetaCumplir = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    MetaCumplirValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsMetaCumplirPorcentage = table.Column<bool>(type: "bit", nullable: false),
                    MetaReal = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    MetaRealValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsMetaRealPorcentage = table.Column<bool>(type: "bit", nullable: false),
                    Evaluacion = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    ProcesoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indicadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Indicadores_Procesos_ProcesoId",
                        column: x => x.ProcesoId,
                        principalTable: "Procesos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ObjetivoProcesoModel",
                columns: table => new
                {
                    ObjetivoId = table.Column<int>(type: "int", nullable: false),
                    ProcesoId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjetivoProcesoModel", x => new { x.ObjetivoId, x.ProcesoId });
                    table.ForeignKey(
                        name: "FK_ObjetivoProcesoModel_Objetivos_ObjetivoId",
                        column: x => x.ObjetivoId,
                        principalTable: "Objetivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjetivoProcesoModel_Procesos_ProcesoId",
                        column: x => x.ProcesoId,
                        principalTable: "Procesos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjetivoIndicadorModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjetivoId = table.Column<int>(type: "int", nullable: false),
                    IndicadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjetivoIndicadorModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjetivoIndicadorModel_Indicadores_IndicadorId",
                        column: x => x.IndicadorId,
                        principalTable: "Indicadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjetivoIndicadorModel_Objetivos_ObjetivoId",
                        column: x => x.ObjetivoId,
                        principalTable: "Objetivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Indicadores_ProcesoId",
                table: "Indicadores",
                column: "ProcesoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjetivoIndicadorModel_IndicadorId",
                table: "ObjetivoIndicadorModel",
                column: "IndicadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjetivoIndicadorModel_ObjetivoId",
                table: "ObjetivoIndicadorModel",
                column: "ObjetivoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjetivoProcesoModel_ProcesoId",
                table: "ObjetivoProcesoModel",
                column: "ProcesoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjetivoIndicadorModel");

            migrationBuilder.DropTable(
                name: "ObjetivoProcesoModel");

            migrationBuilder.DropTable(
                name: "Indicadores");

            migrationBuilder.DropTable(
                name: "Objetivos");

            migrationBuilder.DropTable(
                name: "Procesos");
        }
    }
}
