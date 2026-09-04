using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SistemaAlquilerAutos.Datos;

#nullable disable

namespace SistemaAlquilerAutos.Migrations;

[DbContext(typeof(ContextoAlquiler))]
[Migration("20260904000000_CrearBaseInicial")]
public class CrearBaseInicial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase().Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Clientes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Cedula = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Nombres = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Apellidos = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Telefono = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Correo = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Direccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Clientes", x => x.Id))
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Vehiculos",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Placa = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Marca = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Modelo = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Anio = table.Column<int>(type: "int", nullable: false),
                Color = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                PrecioDiario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Vehiculos", x => x.Id))
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Contratos",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                ClienteId = table.Column<int>(type: "int", nullable: false),
                VehiculoId = table.Column<int>(type: "int", nullable: false),
                FechaInicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                FechaFin = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                TarifaDiaria = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Contratos", x => x.Id);
                table.ForeignKey("FK_Contratos_Clientes_ClienteId", x => x.ClienteId, "Clientes", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_Contratos_Vehiculos_VehiculoId", x => x.VehiculoId, "Vehiculos", "Id", onDelete: ReferentialAction.Restrict);
            }).Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Pagos",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                ContratoId = table.Column<int>(type: "int", nullable: false),
                FechaPago = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Monto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                Metodo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                NumeroReferencia = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Pagos", x => x.Id);
                table.ForeignKey("FK_Pagos_Contratos_ContratoId", x => x.ContratoId, "Contratos", "Id", onDelete: ReferentialAction.Cascade);
            }).Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex("IX_Clientes_Cedula", "Clientes", "Cedula", unique: true);
        migrationBuilder.CreateIndex("IX_Clientes_Correo", "Clientes", "Correo", unique: true);
        migrationBuilder.CreateIndex("IX_Vehiculos_Placa", "Vehiculos", "Placa", unique: true);
        migrationBuilder.CreateIndex("IX_Contratos_ClienteId", "Contratos", "ClienteId");
        migrationBuilder.CreateIndex("IX_Contratos_VehiculoId", "Contratos", "VehiculoId");
        migrationBuilder.CreateIndex("IX_Pagos_ContratoId", "Pagos", "ContratoId");

        migrationBuilder.InsertData("Clientes",
            new[] { "Id", "Cedula", "Nombres", "Apellidos", "Telefono", "Correo", "Direccion", "Activo" },
            new object[,]
            {
                { 1, "1712345678", "María Elena", "Cevallos", "0991234567", "maria.cevallos@correo.com", "Av. Amazonas, Quito", true },
                { 2, "1723456789", "Daniel", "Montalvo", "0987654321", "daniel.montalvo@correo.com", "La Carolina, Quito", true }
            });

        migrationBuilder.InsertData("Vehiculos",
            new[] { "Id", "Placa", "Marca", "Modelo", "Anio", "Color", "PrecioDiario", "Estado", "Activo" },
            new object[,]
            {
                { 1, "PBA-1234", "Chevrolet", "Tracker", 2023, "Blanco", 48.00m, "Disponible", true },
                { 2, "PBC-5678", "Kia", "Soluto", 2022, "Gris", 38.00m, "Disponible", true },
                { 3, "PBD-9012", "Toyota", "Corolla Cross", 2024, "Negro", 62.00m, "Disponible", true }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Pagos");
        migrationBuilder.DropTable(name: "Contratos");
        migrationBuilder.DropTable(name: "Clientes");
        migrationBuilder.DropTable(name: "Vehiculos");
    }
}
