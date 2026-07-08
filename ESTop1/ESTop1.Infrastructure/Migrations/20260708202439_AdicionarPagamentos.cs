using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESTop1.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarPagamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Plano = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    MetodoPagamento = table.Column<string>(type: "TEXT", nullable: false),
                    IdExterno = table.Column<string>(type: "TEXT", nullable: true),
                    CheckoutUrl = table.Column<string>(type: "TEXT", nullable: true),
                    PixQrCode = table.Column<string>(type: "TEXT", nullable: true),
                    PixQrCodeBase64 = table.Column<string>(type: "TEXT", nullable: true),
                    AssinaturaId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PagoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiraEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_IdExterno",
                table: "Pagamentos",
                column: "IdExterno");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_UsuarioId_Status",
                table: "Pagamentos",
                columns: new[] { "UsuarioId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagamentos");
        }
    }
}
