using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexMigración.Migrations
{
    public partial class CamposDeIndiceAgregados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpedienteId = table.Column<string>(nullable: true),
                    FoliadoInicio = table.Column<int>(nullable: false),
                    FoliadoFin = table.Column<int>(nullable: false),
                    TotalHojas = table.Column<int>(nullable: false),
                    Estado = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    NombreArchivo = table.Column<string>(nullable: true),
                    ContenidoArchivo = table.Column<string>(nullable: true),
                    Volumen = table.Column<string>(nullable: true),
                    Libro = table.Column<string>(nullable: true),
                    Folio = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expedientes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificador = table.Column<string>(nullable: true),
                    NombreCliente = table.Column<string>(nullable: true),
                    TipoCaso = table.Column<string>(nullable: true),
                    FechaCreacion = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expedientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Indices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Otorgante = table.Column<string>(nullable: true),
                    Operacion = table.Column<string>(nullable: true),
                    NumeroEscritura = table.Column<string>(nullable: true),
                    Volumen = table.Column<string>(nullable: true),
                    Libro = table.Column<string>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Folio = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Protocolos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpedienteId = table.Column<string>(nullable: true),
                    NumeroEscritura = table.Column<string>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Extracto = table.Column<string>(nullable: true),
                    TextoCompleto = table.Column<string>(nullable: true),
                    Firmado = table.Column<bool>(nullable: false),
                    Volumen = table.Column<string>(nullable: true),
                    Libro = table.Column<string>(nullable: true),
                    Folio = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protocolos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(nullable: true),
                    Contrasena = table.Column<string>(nullable: true),
                    Rol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anexos");

            migrationBuilder.DropTable(
                name: "Expedientes");

            migrationBuilder.DropTable(
                name: "Indices");

            migrationBuilder.DropTable(
                name: "Protocolos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
